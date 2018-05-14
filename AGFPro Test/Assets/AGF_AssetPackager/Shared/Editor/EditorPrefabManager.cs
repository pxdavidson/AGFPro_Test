using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorPrefabManager : MonoBehaviour {
	
	private const string npcDirectoryName = "NPCs";
	private const string artilleryDirectoryName = "Artillery";
	private const string pickupDirectoryName = "Pickups";
	private const string pointLightDirectoryName = "Light_Point";
	private const string shadowPointLightDirectoryName = "Light_Point_Shadow";
	private const string effectDirectoryName = "Effects";
	private const string weaponDirectoryName = "Weapons";
	
	private const string pickupTexturePath = "Assets/Source_Assets/Warehouse/Pickups/GUITextures/Pick_Up.png";
	
	public static bool ConfigurePrefab( GameObject prefab, string directoryName, string prefabName, bool hasGibs ){
		bool generateRenderTexture = true;

		if (prefab.GetComponent<PickupProperties> ()) {
			SetupPickupPrefab (prefab, directoryName, prefabName);
		}else {
			switch (directoryName) {
			case npcDirectoryName:
				SetupNPCPrefab (prefab, directoryName);
				generateRenderTexture = false;
				break;
			case artilleryDirectoryName:
				SetupArtilleryPrefab (prefab, directoryName, prefabName);
				break;
			case pickupDirectoryName:
				SetupPickupPrefab (prefab, directoryName, prefabName);
				break;
			case pointLightDirectoryName:
				SetupPointLightPrefab (prefab, directoryName);
				break;
			case shadowPointLightDirectoryName:
				SetupPointLightPrefab (prefab, directoryName);
				break;
			case effectDirectoryName:
				SetupEffectPrefab (prefab, directoryName);
				generateRenderTexture = false;
				break;
			case weaponDirectoryName:
				SetupWeaponPrefab (prefab, directoryName);
				generateRenderTexture = false;
				break;
			default:
				SetupPrefab (prefab, directoryName, hasGibs);
				break;
			}
		}
		return generateRenderTexture;
	}

	private static void SetupPickupPrefab( GameObject prefab, string directoryName, string prefabName ){
//		string[] pointString = prefabName.Split (new char[]{'_'});
//		int pointValue = 5;
//		int.TryParse (pointString[1], out pointValue);
		
		Main.AddComponentRecursively( prefab.transform, "PickupProperties" );
//		prefab.GetComponent<PickupProperties>().yOffset = 0.5f;
//		Main.AddSphereColliderRecursively( prefab.transform, 2 );
		
		// if this is a pickup, we need to attach the proper gui texture file.
//		prefab.GetComponent<PickupProperties>().pickupTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(pickupTexturePath,typeof(Texture2D));
//		prefab.GetComponent<PickupProperties>().pointValue = pointValue;
		
		SetupPrefab ( prefab, directoryName );
		
		// pickups are always indestructible.
		prefab.GetComponent<TileProperties>().isIndestructible = true;	
	}
	
	private static void SetupPointLightPrefab( GameObject prefab, string directoryName ){
		Main.AddComponentRecursively( prefab.transform, "LightProperties" );
		
		prefab.GetComponent<TileProperties>().useYOffset = true;
		prefab.GetComponent<TileProperties>().yOffset = 3.0f;
//		prefab.GetComponent<TileProperties>().placeableInBuildMode = false;
		prefab.GetComponent<TileProperties>().keepUpright = true;
		
		SetupPrefab ( prefab, directoryName );
		
		// Lights are always indestructible.
		prefab.GetComponent<TileProperties>().isIndestructible = true;
	}
	
	private static void SetupEffectPrefab( GameObject prefab, string directoryName ){
		Main.AddComponentRecursively( prefab.transform, "EffectProperties" );
		
		prefab.GetComponent<TileProperties>().scalable = false;
		
		SetupPrefab( prefab, directoryName );
		
		// Effects are always indestructible.
		prefab.GetComponent<TileProperties>().isIndestructible = true;
	}
	
	private static void SetupArtilleryPrefab( GameObject prefab, string directoryName, string prefabName ){
		Main.AddComponentRecursively( prefab.transform, "CannonProperties" );
		
		// search for the projectile for this prefab.
		prefab.GetComponent<CannonProperties>().projectile = (Transform)AssetDatabase.LoadAssetAtPath("Assets/Source_Assets/Warehouse/Artillery/Resources/" + prefabName + "/Projectile.prefab", typeof(Transform));
		
		// TEMP
		if ( prefabName == "Cannon" ){
			prefab.GetComponent<CannonProperties>().fireAnimationOffsetPosition = new Vector3(0,1.15f,2.0f);
		} else if ( prefabName == "Ballista" ){
			prefab.GetComponent<CannonProperties>().fireAnimationOffsetPosition = new Vector3(0,1.15f,3.0f);
		}
		
		SetupPrefab ( prefab, directoryName );
	}
	
	private static void SetupPrefab( GameObject prefab, string directoryName, bool hasGibs = false ){
		// make all necessary adjustments to the prefab.
		Main.AddComponentRecursively( prefab.transform, "TileProperties" );
		
		prefab.GetComponent<TileProperties>().category = directoryName;
		
		// Main.AddComponentRecursively( prefab.transform, "Rigidbody" );
		
		// remove excess components in the prefab.
//		RemoveExcessPrefabComponentsRecursively( prefab.transform );
		
		Main.SetMeshColliderConvexRecursively( prefab.transform, true );
		// Main.SetTriggerRecursively( prefab.transform, true );
		
		// set indestructibility based on the presence of a gibs folder.
		prefab.GetComponent<TileProperties>().isIndestructible = !hasGibs;	
		
		// determine the size of the component, and add it to the tile properties.
		Bounds bounds = new Bounds ();
		Main.GetBoundsRecursively( prefab.transform,ref bounds );
		
		Vector3 size = bounds.extents * 2;
		prefab.GetComponent<TileProperties>().size = size;
		prefab.GetComponent<TileProperties>().tileBounds = bounds;
		prefab.GetComponent<TileProperties>().tileID = directoryName + "/" + prefab.name;
	}
	
	private static void SetupNPCPrefab( GameObject prefab, string directoryName ){
		// only add the main scripts to the highest level object.
		if ( prefab.GetComponent<NPCProperties>() == null ){
			prefab.AddComponent<NPCProperties>();
		}
		
		prefab.GetComponent<TileProperties>().category = directoryName;
		prefab.GetComponent<TileProperties>().isIndestructible = true;	
		prefab.GetComponent<TileProperties>().scalable = false;
		prefab.GetComponent<TileProperties>().keepUpright = true;
		prefab.GetComponent<TileProperties>().placeableInBuildMode = false;
		
		prefab.GetComponent<Collider>().isTrigger = true;
		
		// set the bounds.
		Bounds bounds = new Bounds ();
			Main.GetBoundsRecursively( prefab.transform,ref bounds ); //new Bounds(prefab.transform.position, Vector3.zero);
		
		
		Vector3 size = bounds.extents * 2;
		prefab.GetComponent<TileProperties>().size = size;
		prefab.GetComponent<TileProperties>().tileBounds = bounds;
		
		// find all children with renderers, and add the NPCRendererChild script to them.
		List<Transform> npcChildren = new List<Transform>();
		Main.GetTransformsWithComponentRecursively( prefab.transform, "Renderer", ref npcChildren );
		
		foreach ( Transform t in npcChildren ){
			if ( t.gameObject.GetComponent<NPCRendererChild>() == null ){
				t.gameObject.AddComponent<NPCRendererChild>();
			}
		}
	}
	
	private static void SetupWeaponPrefab( GameObject prefab, string directoryName ){
		// only add the main scripts to the highest level object.
		if ( prefab.GetComponent<TileProperties>() == null ){
			prefab.AddComponent<WeaponProperties>();
		}
		
//		if ( prefab.GetComponent<HighlightableObject>() == null ){
//			prefab.AddComponent<HighlightableObject>();
//		}
		
		// set the bounds.
		Bounds bounds = new Bounds ();
		Main.GetBoundsRecursively( prefab.transform,ref bounds );
		
		Vector3 size = bounds.extents * 2;
		prefab.GetComponent<TileProperties>().size = size;
		prefab.GetComponent<TileProperties>().tileBounds = bounds;
		
		prefab.GetComponent<TileProperties>().category = directoryName;
		prefab.GetComponent<TileProperties>().isIndestructible = false;	
		prefab.GetComponent<TileProperties>().scalable = false;
		prefab.GetComponent<TileProperties>().keepUpright = true;
		prefab.GetComponent<TileProperties>().placeableInBuildMode = false;
		prefab.GetComponent<TileProperties>().tileID = directoryName + "/" + prefab.name;
	}
	
	private static void RemoveExcessPrefabComponentsRecursively( Transform t ){
		foreach ( Transform child in t ){
			if ( child == t ){
				continue;
			}
			
			RemoveExcessPrefabComponentsRecursively( child );
		}
		
		if ( t.GetComponent<Renderer>() == null || t.GetComponent<Collider>() == null ){
			DestroyImmediate( t.GetComponent<Rigidbody>(), true );
			DestroyImmediate( t.GetComponent<Collider>(), true );
		}
	}
	
	public static void ConfigureGibPrefab( GameObject prefab, string directoryName, string bundleName ){
		Main.AddComponentRecursively( prefab.transform, "GibProperties" );
		prefab.GetComponent<GibProperties>().category = directoryName;
		prefab.GetComponent<GibProperties>().bundle = bundleName;
		
		Main.SetMeshColliderConvexRecursively( prefab.transform, true );
		Main.AddComponentRecursively( prefab.transform, "Rigidbody" );
	}
}
