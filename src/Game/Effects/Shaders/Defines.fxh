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

// Common definitions for targeting various platforms.

#if OPENGL
	#define _vs(r)  : register(vs, r)
	#define _ps(r)  : register(ps, r)
	#define _cb(r)
	#define VS_MODEL vs_3_0
	#define PS_MODEL ps_3_0

	#define BEGIN_PARAMETERS
	#define END_PARAMETERS

	#ifndef NOT_STANDARD_TEXTURE
		#define SAMPLE(texture, texCoord) tex2D(texture, texCoord) 

	    sampler2D Texture : register(s0);
	#endif
#else
	#define _vs(r)
	#define _ps(r)
	#define _cb(r)
	#define VS_MODEL vs_4_0_level_9_3
	#define PS_MODEL ps_4_0_level_9_3

	#define BEGIN_PARAMETERS    cbuffer Parameters : register(b0) {
	#define END_PARAMETERS      };

	#ifndef NOT_STANDARD_TEXTURE
		#define SAMPLE(texture, texCoord) texture.Sample(texture##Sampler, texCoord)

		Texture2D<float4> Texture : register(t0);
		sampler TextureSampler : register(s0);
	#endif
#endif