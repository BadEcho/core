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

#include "font-atlas.h"
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>

using namespace System;
using namespace msdfgen;
using namespace msdf_atlas;
using namespace msclr::interop;
using namespace BadEcho::MsdfGenerator;

void FontAtlas::Generate(FontConfiguration configuration)
{
	FreetypeHandle* freeType = initializeFreetype();

	if (freeType == nullptr)
		return;

	marshal_context^ context = gcnew marshal_context();
	const char* fontPath = context->marshal_as<const char*>(configuration.fontPath);

	if (FontHandle* font = loadFont(freeType, fontPath))
	{
		Charset charset;
		bool charsetLoaded;	

		if (String::IsNullOrEmpty(configuration.charsetPath))
		{
			charset = Charset::ASCII;
			charsetLoaded = true;
		}
		else
		{
			const char* charsetPath = context->marshal_as<const char*>(configuration.charsetPath);
			charsetLoaded = charset.load(charsetPath, false);
		}

		if (charsetLoaded)
			Generate(font, charset, configuration);

		destroyFont(font);
	}

	delete context;
	deinitializeFreetype(freeType);
}

void FontAtlas::Generate(FontHandle* font, const Charset& charset, FontConfiguration configuration)
{
	std::vector<GlyphGeometry> glyphs;
	FontGeometry fontGeometry(&glyphs);

	if (!fontGeometry.loadCharset(font, 1, charset, true, true))
		return;

	int glyphCount = glyphs.size();

	for (GlyphGeometry &glyph : glyphs)
	{
		glyph.edgeColoring(&edgeColoringInkTrap, 3.0, 0);
	}

	TightAtlasPacker atlasPacker;

	atlasPacker.setDimensionsConstraint(TightAtlasPacker::DimensionsConstraint::MULTIPLE_OF_FOUR_SQUARE);
	atlasPacker.setScale(configuration.resolution);
	atlasPacker.setPixelRange(configuration.range);
	atlasPacker.setUnitRange(0);
	atlasPacker.setMiterLimit(0);
	atlasPacker.setPadding(0);
	atlasPacker.pack(glyphs.data(), glyphCount);

	int width = 0, height = 0;

	atlasPacker.getDimensions(width, height);

	double scale = atlasPacker.getScale();
	double range = atlasPacker.getPixelRange();

	ImmediateAtlasGenerator<float, 4, mtsdfGenerator, BitmapAtlasStorage<byte, 4>>
		generator(width, height);

	GeneratorAttributes attributes;
	generator.setAttributes(attributes);
	generator.setThreadCount(4);
	generator.generate(glyphs.data(), glyphCount);

	marshal_context^ context = gcnew marshal_context();
	const char* outputPath = context->marshal_as<const char*>(configuration.outputPath);
	const char* jsonPath = context->marshal_as<const char*>(configuration.jsonPath);

	savePng(generator.atlasStorage(), outputPath);
	exportJSON(&fontGeometry, 1, scale, range, width, height, ImageType::MTSDF, YDirection::TOP_DOWN, jsonPath, true);

	delete context;
}