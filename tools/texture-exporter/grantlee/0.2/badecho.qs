var exportData = function(root)
{
    var doc = {
        atlas: exportAtlas(root),
        meta: {
            version: "1.0",
            smartupdate: root.smartUpdateKey
        },
    };
    return JSON.stringify(doc, null, "\t");
}
exportData.filterName = "exportData";
Library.addFilter("exportData");

var exportAtlas = function(root)
{
    var texture = root.texture;

    var exportNode = {
        texturePath: texture.fullName,
        normalMap: (texture.normalMapFileName === "") ? undefined : texture.normalMapFileName,
        format: root.settings.outputFormat,
        size: {
            width: texture.size.width,
            height: texture.size.height
        },
        scale: root.variantParams.scale,
        regions: exportRegions(texture.allSprites)
    };
    
    return exportNode;
}

var exportRegions = function(sprites)
{
    var result = [];

    for (var i = 0; i < sprites.length; i++)
    {
        var sprite = sprites[i];
        var exportNode = {
            id: sprite.trimmedName,
            source: { 
                x: sprite.frameRect.x,
                 y: sprite.frameRect.y, 
                 width: sprite.frameRect.width, 
                 height: sprite.frameRect.height
                 }
        };
        
        if (sprite.scale9Enabled) 
        {
            exportNode.scaleNine = {
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