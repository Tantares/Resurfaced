# R E S U R F A C E D
## v1.1

Stockalike PBR shaders for KSP.
Add to GameData.

This is the distribution for players, for modders, the shaders can be found at:
https://github.com/Tantares/Resurfaced

Licence: GPL

# Example Usage for Modders.
How to apply the new shader-material to a part.

Create a file to store your material for the part(s), for example `material.cfg` would look like this:
```
SHABBY_MATERIAL_DEF
{
	name = mek_mok_m1

	shader = Resurfaced/Standard

	TEXTURE
	{
		_MainTex  = Tantares/parts/core_mek_mok/a1
		_MetalMap = Tantares/parts/core_mek_mok/me1
		_BumpMap  = Tantares/parts/core_mek_mok/n1
	}

	COLOR
	{
		_Color = #fff
	}

	FLOAT
	{
		_Metalness = 1.0
		_Smoothness  = 1.0
		_MetalAlbedoMultiplier = 1.5
	}
}
```

The line `_MetalAlbedoMultiplier = 1.5` makes the metal parts a little bit brighter. This is usually required as the default metal colour for KSP parts is quite dark, this multiplier brings it up to more typical PBR levels.


Then apply this in the part config:

```
SHABBY_MATERIAL_REPLACE
{
    materialDef = mek_mok_m1
    targetTransform = mesh1
}
```

This will apply the `mek_mok_m1` material to the mesh with the name `mesh1` on this part.
The `targetTransform` line isn't required, if left out it will apply to all meshes on the part.
Multiple parts can share the same material.