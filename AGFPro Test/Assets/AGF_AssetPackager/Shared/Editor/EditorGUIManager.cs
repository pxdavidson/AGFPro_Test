using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorGUIManager : MonoBehaviour {
	private static string warehousePrefabDirectory = "/AGF_Intermediate/Warehouse/";
	
	public static bool MakePrefabFromSelection( Object obj, string bundleName, ref List<string> prefabList ){
		// ensure that all assets have been imported properly.
		AssetDatabase.Refresh();
		
		string assetPath = AssetDatabase.GetAssetPath(obj);
		string[] assetPathSplit = assetPath.Split( new char[]{'/'} );
		
		// save the source and target files strings.
		string sourceFile = assetPathSplit[assetPathSplit.Length-1];
		string prefabFile = sourceFile;
		
		if ( prefabFile.EndsWith(".ma") ){
			prefabFile = Main.TrimEndFromString( prefabFile, ".ma" ) + ".prefab";
		} else if ( prefabFile.EndsWith(".mb") ){
			prefabFile = Main.TrimEndFromString( prefabFile, ".mb" ) + ".prefab";
		} else if ( prefabFile.EndsWith(".obj") ){
			prefabFile = Main.TrimEndFromString( prefabFile, ".obj" ) + ".prefab";
		} else if ( prefabFile.EndsWith(".fbx") ){
			prefabFile = Main.TrimEndFromString( prefabFile, ".fbx" ) + ".prefab"; 
		}
		
		// determine the current directory.
		string currentDirectory = "";
		for ( int i = 0; i < assetPathSplit.Length-1; i++ ){
			currentDirectory += assetPathSplit[i] + "/";	
		}
		
		// grab the directory name.
		string currentDirectoryName = assetPathSplit[assetPathSplit.Length-2];
			
		// prepare the path strings.
		string targetDirectoryAbsPath = Application.dataPath + warehousePrefabDirectory + currentDirectoryName + "/";
		string targetDirectoryRelPath = "Assets" + warehousePrefabDirectory + currentDirectoryName + "/";
		
		// create a new directory at the destination, if necessary.
		if ( Directory.Exists(targetDirectoryAbsPath) == false ){
			Directory.CreateDirectory(targetDirectoryAbsPath);
		}
		
		// if the old prefab exists, delete it.
		AssetDatabase.DeleteAsset( targetDirectoryRelPath + prefabFile );
		
		// create the new prefab.
		GameObject mayaFile = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath,typeof(GameObject));
		if ( mayaFile == null ){
			// an error occurred.
			EditorUtility.DisplayDialog("Error!", string.Format ("Could not read maya file:\n{0}", assetPath), "Ok");
			Debug.LogError(string.Format("Error! Could not read maya file: {0}", assetPath));	
			return false;
		}
		
		print (string.Format ("Now generating: {0}", targetDirectoryRelPath + prefabFile));
		GameObject prefab = PrefabUtility.CreatePrefab(targetDirectoryRelPath + prefabFile, mayaFile, ReplacePrefabOptions.Default);
		
		// check to see if a gibs folder exists within the directory. this will determine if the object is destructible or not.
		
		bool hasGibs = Directory.Exists ( currentDirectory + "Gibs/" );
		
		// configure the prefab as appropriate. generate the render texture as well.
		bool result = SetupPrefab( prefab, targetDirectoryAbsPath, targetDirectoryRelPath, currentDirectoryName, hasGibs ); //ref newCategory, ref newGibCategory, 
		if ( result == false ){
			// an error occurred.
			return false;
		}
		
		prefabList.Add ( targetDirectoryRelPath + prefabFile );
		return true;	
	} 
	
	public static bool MakeGibPrefabFromSelection( Object obj, string bundleName, ref List<string> prefabList ){
		// ensure that all assets have been imported properly.
		AssetDatabase.Refresh();
		
		string assetPath = AssetDatabase.GetAssetPath(obj);
		string[] assetPathSplit = assetPath.Split( new char[]{'/'} );
		
		// save the source and target files strings.
		string sourceFile = assetPathSplit[assetPathSplit.Length-1];
		string prefabFile = sourceFile;
		
		if ( prefabFile.EndsWith(".ma") ){
			prefabFile = Main.TrimEndFromString( prefabFile, ".ma" ) + ".prefab";
		} else if ( prefabFile.EndsWith(".mb") ){
			prefabFile = Main.TrimEndFromString( prefabFile, ".mb" ) + ".prefab";
		} else if ( prefabFile.EndsWith(".obj") ){
			prefabFile = Main.TrimEndFromString( prefabFile, ".obj" ) + ".prefab";
		} else if ( prefabFile.EndsWith(".fbx") ){
			prefabFile = Main.TrimEndFromString( prefabFile, ".fbx" ) + ".prefab"; 
		}
		
		// prepare the path strings.
		string currentDirectoryName = "";
		string targetDirectoryRelPath = "";
		
		if ( assetPathSplit[assetPathSplit.Length-2] == "Gibs" ){
			currentDirectoryName = assetPathSplit[assetPathSplit.Length-3];
			targetDirectoryRelPath = "Assets" + warehousePrefabDirectory + currentDirectoryName + "/Gibs/";
		} else if ( assetPathSplit[assetPathSplit.Length-2] == "Extra" ) {
			currentDirectoryName = assetPathSplit[assetPathSplit.Length-4];
			targetDirectoryRelPath = "Assets" + warehousePrefabDirectory + currentDirectoryName + "/Gibs/Extra/";
		}
			
		string targetDirectoryAbsPath = Application.dataPath + warehousePrefabDirectory + currentDirectoryName + "/";
		 
		// create a new base directory at the destination, if necessary.
		if ( Directory.Exists(targetDirectoryAbsPath) == false ){
			Directory.CreateDirectory(targetDirectoryAbsPath);
		}
		
		// create a new gib directory at the destination, if necessary.
		if ( Directory.Exists(targetDirectoryAbsPath + "Gibs/" ) == false ){
			Directory.CreateDirectory(targetDirectoryAbsPath + "Gibs/");
		}
		
		if ( assetPathSplit[assetPathSplit.Length-2] == "Extra" ){
			// create a new extra directory at the destination, if necessary.
			if ( Directory.Exists(targetDirectoryAbsPath + "Gibs/Extra/" ) == false ){
				Directory.CreateDirectory(targetDirectoryAbsPath + "Gibs/Extra/");
			}
		}
		
		// if the old prefab exists, delete it.
		AssetDatabase.DeleteAsset( targetDirectoryRelPath + prefabFile );
		
		// create the new prefab.
		GameObject mayaFile = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath,typeof(GameObject));
		if ( mayaFile == null ){
			// an error occurred.
			EditorUtility.DisplayDialog("Error!", string.Format ("Could not read maya file:\n{0}", assetPath), "Ok");
			Debug.LogError(string.Format("Error! Could not read maya file: {0}", assetPath));	
			return false;
		}
		
		print (string.Format ("Now generating: {0}", targetDirectoryRelPath + prefabFile));
		GameObject prefab = PrefabUtility.CreatePrefab(targetDirectoryRelPath + prefabFile, mayaFile, ReplacePrefabOptions.Default);
		
		// if this is a gib, add the gib properties to it.
		if ( assetPathSplit[assetPathSplit.Length-2] == "Gibs" ){
			EditorPrefabManager.ConfigureGibPrefab( prefab, currentDirectoryName, bundleName );
		}
		
		// the gib settings needs to know to which category it belongs.
		if ( prefab.GetComponent<GibSettings>() != null ){
			prefab.GetComponent<GibSettings>().category = currentDirectoryName;
			prefab.GetComponent<GibSettings>().bundle = bundleName;
		}
		
		prefabList.Add ( targetDirectoryRelPath + prefabFile );
		
		return true;
	}
	
	static bool SetupPrefab( GameObject prefab, string targetDirectoryAbsPath, string targetDirectoryRelPath, string directoryName, bool hasGibs ){
		GameObject abs = GameObject.Find ("AssetPackagerSettings");
		if ( abs == null ){
			EditorUtility.DisplayDialog( "Error!", "Unable to complete operation. Please make sure the AssetPackagerSettings prefab has been placed within your scene.", "Ok" );
			Debug.LogError ("Error! AssetPackagerSettings prefab missing from scene.");
			return false;
		}
		AssetPackagerSettings assetPackagerSettings = abs.GetComponent<AssetPackagerSettings>();
		
		// prepare the render texture process. instantiate lights and camera.
		Camera newCamera = (Camera)Instantiate( (Camera)assetPackagerSettings.scriptCamera );
		Transform lighting = (Transform)Instantiate( (Transform)assetPackagerSettings.tileTextureLighting );
		
		string[] split = AssetDatabase.GetAssetPath( prefab ).Split ( new char[]{'/'});
		string prefabName = Main.TrimEndFromString(split[split.Length-1], ".prefab");
		
		if ( prefab == null ){
			EditorUtility.DisplayDialog( "Error!", "While attempting to convert " + prefabName + ", the prefab was either null or otherwise unreadable.", "Ok" );
			Debug.LogError ("Error! Unable to setup prefab.");
			return false;
		}
		
		bool generateRenderTexture = EditorPrefabManager.ConfigurePrefab( prefab, directoryName, prefabName, hasGibs );
		
		string destinationPath = targetDirectoryAbsPath + prefab.GetComponent<TileProperties>().name + ".png";
		string loadPath = targetDirectoryRelPath + prefab.GetComponent<TileProperties>().name + ".png";
//		Vector3 size = prefab.GetComponent<TileProperties>().GetWorldSize();
		Bounds bounds = prefab.GetComponent<TileProperties>().GetLocalBounds();
		
		// check to see if the render texture already exists and if we should not re-generate them.
		if ( generateRenderTexture == false ){
			prefab.GetComponent<TileProperties>().tileTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(loadPath,typeof(Texture2D));
			if ( prefab.GetComponent<TileProperties>().tileTexture == null ){
				generateRenderTexture = true;
			}
		}
		
		if ( generateRenderTexture ){
			prefab.GetComponent<TileProperties>().tileTexture = EditorUtilities.GenerateRenderTexture( prefab, newCamera, destinationPath, loadPath, bounds, TextureFormat.RGB24 );
		}
		
		// clean up the render texture creation process. destroy the lights and camera.
		DestroyImmediate(newCamera.gameObject);
		DestroyImmediate(lighting.gameObject);
		
		return true;
	}
	
}