AGF Asset Packager v. 0.1
(c) Heavy Water 2013
Last Updated: 9/12/2013

Terrain Textures:
a) Must following the following naming convention for each pair of textures.
	"*_c" or "*_Diffuse" = colormap
	"*_n" or "*_Normal" = normalmap
	where * must be the same string for both the colormap and normalmap.
	ex: Beach_c.png, Beach_n.png; Grass_c.bmp, Grass_n.bmp.
b) The normalmaps must be marked as a "Normal map" texture in unity's import settings.
c) Note: The tool will automatically ask if you want to remove alpha channels and apply the normal map import settings on package build.
d) Must have their textures set to "Read/Write Enabled" in the Texture importer.