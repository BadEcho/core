//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

#include "Defines.fxh"

BEGIN_PARAMETERS
    float4x4 MatrixTransform _vs(c0) _cb(c0);
    float Alpha _vs(c4) _cb(c4);
END_PARAMETERS
    
struct VSOutput
{
    float4 Position : SV_Position;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

VSOutput SpriteVertexShader(    float4 position : POSITION0, 
                                float4 color : COLOR0, 
                                float2 texCoord : TEXCOORD0)
{
    VSOutput output;
    
    output.Position = mul(position, MatrixTransform);
    output.Color = color;
    output.Color.a *= Alpha;
    output.Color.rgb *= Alpha;    
    output.TexCoord = texCoord;
    
    return output;
}

float4 SpritePixelShader(VSOutput input) : SV_Target0
{
    return SAMPLE(Texture, input.TexCoord) * input.Color;
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile VS_MODEL SpriteVertexShader();
        PixelShader = compile PS_MODEL SpritePixelShader();
    }
};