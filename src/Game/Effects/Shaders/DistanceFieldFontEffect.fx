//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

#define NOT_STANDARD_TEXTURE

#include "Defines.fxh"

BEGIN_PARAMETERS
    float4x4 WorldViewProjection _vs(c0) _cb(c0);
    float2 AtlasSize _ps(c0) _cb(c4);
    float DistanceRange _ps(c1)  _cb(c5);
END_PARAMETERS

texture Texture : register(t0);

sampler AtlasSampler : register(s0) = sampler_state
{
    Texture = (Texture);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    MipFilter = LINEAR;
};

struct VSInput
{
    float4 Position : POSITION0;
    float4 FillColor : COLOR0;
    float4 StrokeColor : COLOR1;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
    float4 FillColor : COLOR0;
    float4 StrokeColor : COLOR1;
    float2 TexCoord : TEXCOORD0;
};

// Returns the channel value that is in the middle of a data set consisting of the provided color's RGB values.
float Median(float3 color)
{
    return max(min(color.r, color.g), min(max(color.r, color.g), color.b));
}

// Normalizes the provided vector, safely.
float2 SafeNormalize(float2 v)
{
    float vLength = length(v);

    vLength = (vLength > 0.0) ? 1.0 / vLength : 0.0;

    return v * vLength;
}

// Calculates the opacity to apply with respect to screen-space x- and y-coordinates given the specified encoded signed distance.
float GetOpacityFromDistance(float signedDistance, float2 Jdx, float2 Jdy)
{   // A signed distance field is a texture that stores distance values rather than colors. A negative distance means we're inside the shape, zero
    // means we're on the border, and positive values indicate that we're outside the shape. The main job of this shader is to approximate pixel coverage based
    // on our distance from the edge. If a shape's edge hits a pixel's center exactly, then we would have a distance of 0, with 50% coverage.
    // The inverse of the pixel coverage can be used as an opacity value that can be multiplied against our color.

    // The distance from the center of a pixel to one of its corners is the square root of 2 divided by 2 (the length of a diagonal is the length of a
    // side (which is 1) multiplied by the square root of 2, dividing by 2 gives us the length from the center), or 0.7071. This is our maximum distance from pixel center.
    const float distanceLimit = sqrt(2.0f) / 2.0f;  // If we use this value to define the min and max range in a smoothstep function, along with the signed distance value as
    const float thickness = 1.0f / DistanceRange;   // the value to be interpolated, this will return the appropriate amount of pixel coverage.
													// This only works, however, if the size of the geometry we're rendering is the same size as the distance field's
													// rectangle in the source texture. This is not the case with our signed distance fonts.

	// Multiplying our distance limit by the partial derivative of non-normalized texture pixel coordinates (pixelCoord) will provide us with uniform scaling.
    // In order to account for non-uniform scaling and perspective, however, we need to scale our distance limit based on how a vector normal to the outline curve
    // is transformed (like with ellipses). We do this by using a normalized gradient vector (normalized to avoid approximation errors when near the edge of the shape)
    // which we multiply by a Jacobian matrix containing our coordinate's first-order partial derivatives.
    float2 gradientDistance = SafeNormalize(float2(ddx(signedDistance), ddy(signedDistance)));
    float2 gradient = float2(gradientDistance.x * Jdx.x + gradientDistance.y * Jdy.x, gradientDistance.x * Jdx.y + gradientDistance.y * Jdy.y);
    float scaledDistanceLimit = min(thickness * distanceLimit * length(gradient), 0.5f);

    return smoothstep(-scaledDistanceLimit, scaledDistanceLimit, signedDistance);
}

// A fairly standard vertex shader; most of the work is done in the pixel shader.
VSOutput DistanceVertexShader(in VSInput input)
{
    VSOutput output;

    output.Position = mul(input.Position, WorldViewProjection);
    output.FillColor = input.FillColor;
    output.StrokeColor = input.StrokeColor;
    output.TexCoord = input.TexCoord;

    return output;
}

// A pixel shader to use for normal- and large-sized text.
float4 DistancePixelShader(VSOutput input) : COLOR
{   // Divide the distance field range by the atlas size to give us a distance range applicable to texels.
    float2 texelDistanceRange = DistanceRange / AtlasSize;
    float3 sample = tex2D(AtlasSampler, input.TexCoord).rgb;
        
    // Vertex color values go from 0.0 to 1.0. The center of a pixel is located at (0, 0), however the edges are located at +/- 0.5.
    // We subtract 0.5 to align with the pixels.
    float signedDistance = Median(sample) - 0.5f;

    // Calculate the relation between the texel distance range and the changes across the input coordinates, and apply this to our
    // signed distance. This converts the distance to screen pixels.
    signedDistance *= dot(texelDistanceRange, 0.5f / fwidth(input.TexCoord));

    // Align the distance value back to a normal vertex color value range by adding 0.5 back to it.
    float opacity = clamp(signedDistance + 0.5f, 0.0f, 1.0f);

    return input.FillColor * opacity;
}

// A pixel shader to use for normal- and large-sized outlined text.
float4 StrokedDistancePixelShader(VSOutput input) : COLOR
{   // Refer to DistancePixelShader for documentation on the code common to both shaders.
    float2 texelDistanceRange = DistanceRange / AtlasSize;
    float medianSample = Median(tex2D(AtlasSampler, input.TexCoord).rgb);
    float signedDistance = medianSample - 0.5f;

    float distanceInputRelation = dot(texelDistanceRange, 0.5f / fwidth(input.TexCoord));

    signedDistance *= distanceInputRelation;

    const float strokeThickness = 0.1875f;
    // We want to fill in the stroke based on proximity to the edge, as opposed to the center of the glyph, and we only want the outline itself to be so thick.
    // We align the sampled texture data such that, unlike the fill color, positive values will result in an outline along the edge.
    float strokeDistance = -(abs(medianSample - 0.25f - strokeThickness) - strokeThickness);

    // We now convert the signed stroke distance to screen pixels.
    strokeDistance *= distanceInputRelation;
    
    float opacity = clamp(signedDistance + 0.5f, 0.0f, 1.0f);
    float strokeOpacity = clamp(strokeDistance + 0.5f, 0.0f, 1.0f);

    // Blend the colors together with some linear interpolation.
    return lerp(input.StrokeColor, input.FillColor, opacity) * max(opacity, strokeOpacity);
}

// A pixel shader to use for small- and normal-sized text.
// This will render small-sized text more accurately without artifacts. As the text size increases however, the text may appear 'phatter' than it should.
// In that case, use the standard DistancePixelShader.
float4 SmallDistancePixelShader(VSOutput input) : COLOR
{   // A downside of SDF fonts is that small font sizes will experience a degradation in quality when working from the same, large
    // resolution font atlas that all other sizes are working from. Because creating a separate font atlas specifically for small font sizes
    // defeats the purpose of MSDF altogether, a special shader needs to be provided for when the font size is small enough.

    // This shader takes inspiration from the concepts discussed by James M. Lan Verth in his article on the topic:
    // http://www.essentialmath.com/blog/?p=151&cpage=1
    // As well as the implementation of said concepts by the Cinder-SdfText project (https://github.com/chaoticbob/Cinder-SdfText).
    float2 pixelCoord = input.TexCoord * AtlasSize;    
    float2 Jdx = ddx(pixelCoord);
    float2 Jdy = ddy(pixelCoord);
    float3 sample = tex2D(AtlasSampler, input.TexCoord).rgb;
    float signedDistance = Median(sample) - 0.5f;
    
    float opacity = GetOpacityFromDistance(signedDistance, Jdx, Jdy);
    float4 color;

    color.a = pow(abs(input.FillColor.a * opacity), 1.0f / 2.2f);   // Correct for gamma, 2.2 is a valid gamma for most LCD monitors.
    color.rgb = input.FillColor.rgb * color.a;

    return color;
}

// A pixel shader to use for small- and normal-sized outlined text.
// This will render small-sized outline text more accurately without artifacts; however, as text continues to decrease in size, eventually it becomes
// unfeasible to try outline text due to the ever-shrinking amount of fill pixels that can be replaced.
float4 StrokedSmallDistancePixelShader(VSOutput input) : COLOR
{   // Refer to SmallDistancePixelShader and StrokedDistancePixelShader for documentation on code common to both shaders.
    float2 pixelCoord = input.TexCoord * AtlasSize;
    float2 Jdx = ddx(pixelCoord);
    float2 Jdy = ddy(pixelCoord);
    float medianSample = Median(tex2D(AtlasSampler, input.TexCoord).rgb);
    float signedDistance = medianSample - 0.5f;

	const float strokeThickness = 0.1875f;
    float strokeDistance = -(abs(medianSample - 0.25f - strokeThickness) - strokeThickness);

    float opacity = GetOpacityFromDistance(signedDistance, Jdx, Jdy);
    float strokeOpacity = GetOpacityFromDistance(strokeDistance, Jdx, Jdy);

    return lerp(input.StrokeColor, input.FillColor, opacity) * max(opacity, strokeOpacity);
}

technique DistanceFieldFont
{
    pass
    {
        VertexShader = compile VS_MODEL DistanceVertexShader();
        PixelShader = compile PS_MODEL DistancePixelShader();
    }
}

technique SmallDistanceFieldFont
{
    pass
    {
        VertexShader = compile VS_MODEL DistanceVertexShader();
        PixelShader = compile PS_MODEL SmallDistancePixelShader();
    }
}

technique StrokedDistanceFieldFont
{
    pass
    {
        VertexShader = compile VS_MODEL DistanceVertexShader();
        PixelShader = compile PS_MODEL StrokedDistancePixelShader();
    }
}

technique StrokedSmallDistanceFieldFont
{
    pass
    {
        VertexShader = compile VS_MODEL DistanceVertexShader();
        PixelShader = compile PS_MODEL StrokedSmallDistancePixelShader();
    }    
}