var exportAtlas = function(root)
{
    var texture = root.texture;

    var atlas = {
        texturePath: texture.fullName,
        normalMap: (texture.normalMapFileName === "") ? undefined : texture.normalMapFileName,
        format: root.settings.outputFormat,
        scale: root.variantParams.scale,
        regions: exportRegions(texture.allSprites),        
        meta: {
            version: "1.0",
            smartupdate: root.smartUpdateKey
        },
    };

    return JSON.stringify(atlas, null, "\t");
}
exportAtlas.filterName = "exportAtlas";
Library.addFilter("exportAtlas");

var exportRegions = function(sprites)
{
    var result = [];

    for (var i = 0; i < sprites.length; i++)
    {
        var sprite = sprites[i];
        var exportNode = {
            name: sprite.trimmedName,
            sourceArea: { 
                x: sprite.frameRect.x,
                y: sprite.frameRect.y, 
                width: sprite.frameRect.width, 
                height: sprite.frameRect.height
            }            
        };
        
        if (sprite.scale9Enabled) 
        {
            exportNode.nineSliceArea = {
                x: sprite.scale9Borders.x,
                y: sprite.scale9Borders.y,
                width: sprite.scale9Borders.width,
                height: sprite.scale9Borders.height
            };
        }

        result.push(exportNode);
    }

    return result;
}