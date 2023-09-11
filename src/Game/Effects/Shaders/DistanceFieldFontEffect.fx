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
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

// A fairly standard vertex shader; most of the work is done in the pixel shader.
VSOutput DistanceVertexShader(in VSInput input)
{
    VSOutput output;

    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;

    return output;
}

float4 DistancePixelShader(VSOutput input) : COLOR
{   // Divide the distance field range by the atlas size to give us a distance range applicable to texels.
    float2 texelDistanceRange = DistanceRange / AtlasSize;
    float3 sample = tex2D(AtlasSampler, input.TexCoord).rgb;

    // We take the middle value in the data set and shift its range to -0.5 to 0.5, centering it at zero.
    float signedDistance = max(min(sample.r, sample.g), min(max(sample.r, sample.g), sample.b)) - 0.5f;

    // Calculate the relation between the texel distance range and the changes across the input coordinates, and apply this to our
    // signed distance.
    signedDistance = signedDistance * dot(texelDistanceRange, 0.5f / fwidth(input.TexCoord));

    float opacity = clamp(signedDistance + 0.5f, 0.0f, 1.0f);

    return input.Color * opacity;
}

technique DistanceFieldFont
{
    pass
    {
        VertexShader = compile VS_MODEL DistanceVertexShader();
        PixelShader = compile PS_MODEL DistancePixelShader();
    }
}