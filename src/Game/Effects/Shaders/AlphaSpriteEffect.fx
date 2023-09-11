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

#include "Defines.fxh"

BEGIN_PARAMETERS
    float4x4 MatrixTransform _vs(c0) _cb(c0);
    float Alpha _vs(c4) _cb(c4);
END_PARAMETERS
    
struct VSOutput
{
    float4 position : SV_Position;
    float4 color    : COLOR0;
    float2 texCoord : TEXCOORD0;
};

VSOutput SpriteVertexShader(    float4 position : POSITION0, 
                                float4 color : COLOR0, 
                                float2 texCoord : TEXCOORD0)
{
    VSOutput output;
    
    output.position = mul(position, MatrixTransform);
    output.color = color;
    output.color.a = Alpha;
    output.color.rgb *= Alpha;    
    output.texCoord = texCoord;
    
    return output;
}

float4 SpritePixelShader(VSOutput input) : SV_Target0
{
    return SAMPLE(Texture, input.texCoord) * input.color;
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile VS_MODEL SpriteVertexShader();
        PixelShader = compile PS_MODEL SpritePixelShader();
    }
};