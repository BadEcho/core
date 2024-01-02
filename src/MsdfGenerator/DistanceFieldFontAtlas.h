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

#pragma once

#include <msdf-atlas-gen/msdf-atlas-gen.h>

namespace BadEcho::MsdfGenerator {

	/// <summary>
	/// Represents configuration settings for a multi-channel signed distance field font atlas to generate.
	/// </summary>
	public value struct FontConfiguration sealed
	{
		/// <summary>
		/// The path to the font file (.ttf/.otf) to create an atlas for.
		/// </summary>
		System::String^ FontPath;
		/// <summary>
		/// The path to the file containing the character set to include in the atlas; defaults to ASCII if unset.
		/// </summary>
		System::String^ CharsetPath;		
		/// <summary>
		/// The path to the JSON file to write the atlas's layout data to.
		/// </summary>
		System::String^ JsonPath;
		/// <summary>
		/// The path to the image file to write the atlas to.
		/// </summary>
		System::String^ OutputPath;
		/// <summary>
		/// The size of the glyphs in the atlas, in pixels-per-em.
		/// </summary>
		unsigned int Resolution;
		/// <summary>
		/// The distance field range in output pixels, which affects how far the distance field extends beyond the glyphs.
		/// </summary>
		unsigned int Range;
	};

	/// <summary>
	/// Provides multi-channel signed distance field font atlas generation methods.
	/// </summary>
	public ref class DistanceFieldFontAtlas abstract sealed
	{
	public:
		/// <summary>
		/// Generates a MSDF atlas using the specified settings.
		/// </summary>
		/// <param name="configuration">The configuration settings for the font atlas to generate.</param>
		static void Generate(FontConfiguration configuration);

	private:
		static void Generate(msdfgen::FontHandle* font, const msdf_atlas::Charset& charset, FontConfiguration configuration);
	};
}