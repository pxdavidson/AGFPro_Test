// C# Example
// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ExportAssetBundles {
	private static string[] textureExtensions = new string[]{ ".psd", ".tiff", ".jpg", ".tga", ".png", ".gif", ".bmp", ".iff", ".pict", ".tif"};
	private static string[] audioExtensions = new string[]{ ".mp3", ".ogg", ".wav", ".aiff", ".aif", ".mod", ".it", ".s3m", ".xm"};

	[MenuItem("Assets/Build AGF Asset Pack/Warehouse")]
	static void ExportWarehouseAssetPack(){
		ExportWarehouseBundle ();
	}

	[MenuItem("Assets/Build AGF Asset Pack/Terrain")]
	static void ExportTerrainAssetPack(){
		ExportTerrainBundle ();
	}

	[MenuItem("Assets/Build AGF Asset Pack/Vegetation")]
	static void ExportVegetationAssetPack(){
		ExportVegetationBundle ();
	}

	[MenuItem("Assets/Build AGF Asset Pack/Skybox")]
	static void ExportSkyboxAssetPack(){
		ExportSkyboxBundle ();
	}

	[MenuItem("Assets/Build AGF Asset Pack/Audio")]
	static void ExportAudioAssetPack(){
		ExportAudioBundle ();
	}

[MenuItem("Axis Game Factory/Build Asset Pack/Warehouse")]
    static void ExportWarehouseBundle () {
		Object folder = Selection.activeObject;
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		
        // Bring up save panel
        string saveLocation = EditorUtility.SaveFilePanel ("Build Warehouse Pack", "", folder.name + "_w", "unity3d");
        if (saveLocation.Length != 0) {
            // Build the resource file from the active selection.
			string[] saveSplit = saveLocation.Split( new char[]{'/'} );
			string bundleName = saveSplit[saveSplit.Length-1];
			
			// find all maya, fbx and prefab files within the selection.
			List<Object> tiles = new List<Object>();
			List<Object> gibs = new List<Object>();
			for ( int i = 0; i < selection.Length; i++ ){
				string path = AssetDatabase.GetAssetPath( selection[i] );
				if ( path.EndsWith(".ma") || path.EndsWith(".mb") || path.EndsWith(".fbx") || path.EndsWith(".obj") || path.EndsWith(".prefab") ){	
					// split the files into two lists, one for gibs specifically and one for tiles.
					if ( path.Contains("/Gibs/") ) gibs.Add ( selection[i] ); 
					else tiles.Add ( selection[i] );
				}
			}
			
			// create prefabs for each of the selected objects.
			List<string> prefabPaths = new List<string>();
			foreach ( Object obj in tiles ){
				bool success = EditorGUIManager.MakePrefabFromSelection( obj, bundleName, ref prefabPaths );
				if ( success == false ) return;
			}
			
			// create prefabs for each of the selected gibs. link them to their owner objects appropriately.
			foreach( Object obj in gibs ){
				bool success= EditorGUIManager.MakeGibPrefabFromSelection( obj, bundleName, ref prefabPaths );
				if ( success == false ) return;
			}
			
			// Create an array of names for the assets placed into the bundle.
			Object[] prefabs = new Object[prefabPaths.Count];
			for ( int i = 0; i < prefabs.Length; i++ ){
				prefabs[i] = AssetDatabase.LoadAssetAtPath( prefabPaths[i], typeof(Object) );
			}
			
			string[] objectNames = new string[prefabs.Length];
			for ( int i = 0; i < prefabs.Length; i++ ){
				string prefabPath = AssetDatabase.GetAssetPath( prefabs[i] );
				string[] split = prefabPath.Split ( new char[]{'/'} );
				objectNames[i] = split[split.Length-2] + "/" + split[split.Length-1];
			}

			AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

			string[] assetPaths = new string[prefabPaths.Count];
			for (int i = 0; i < prefabPaths.Count; i++) {
				assetPaths [i] = prefabPaths [i];
			}

			buildMap [0].assetBundleName = bundleName;
			buildMap [0].assetNames = assetPaths;

			// Remove the file name from the save path
			saveLocation = saveLocation.Remove (saveLocation.LastIndexOf("/"));
			BuildPipeline.BuildAssetBundles (saveLocation, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

//			BuildPipeline.BuildAssetBundleExplicitAssetNames( prefabs, objectNames, saveLocation, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows );
            // Selection.objects = prefabs;
			
			// make sure the new bundle shows up in the project window.
			AssetDatabase.Refresh();
        }
    }
	
	[MenuItem("Axis Game Factory/Build Asset Pack/Terrain")]
    static void ExportTerrainBundle () {
		Object folder = Selection.activeObject;
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		
        // Bring up save panel
        string saveLocation = EditorUtility.SaveFilePanel ("Build Terrain Pack", "", folder.name + "_t", "unity3d");
        if (saveLocation.Length != 0) {
			
			// ask if the tool should automatically configure texture import settings.
			bool result = EditorUtility.DisplayDialog( "Reimport textures with recommended settings?", "This operation will change the import settings of the selected textures so that alpha channels are removed and normal maps are marked appropriately.", "Reimport", "Skip" );
			
			if ( result == true ){
				for ( int i = 0; i < selection.Length; i++ ){
					string path = AssetDatabase.GetAssetPath( selection[i] );
					for ( int j = 0; j < textureExtensions.Length; j++ ){
						if ( path.EndsWith(textureExtensions[j]) ){
							
							// found a texture. reimport it.
							TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
							if ( path.Contains("_n") || path.Contains("_Normal" ) || path.Contains("_nmp" ) ){
								importer.normalmap = true;
//								importer.textureFormat = TextureImporterFormat.
							} else {
								importer.textureFormat = TextureImporterFormat.DXT1;	
							}
							AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
							
						}
					}
				}
			}
			
            // Build the resource file from the active selection.
			BuildTextureAssetBundle( selection, saveLocation );
        }
    }
	
	[MenuItem("Axis Game Factory/Build Asset Pack/Vegetation")]
    static void ExportVegetationBundle () {
		Object folder = Selection.activeObject;
        Object[] selectionArray = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

		List<Object> selection = new List<Object>();

		for (int i = 0; i < selectionArray.Length; i++) {
			string path = AssetDatabase.GetAssetPath( selectionArray[i] );
			for (int j = 0; j < textureExtensions.Length; j++) {
				if (path.EndsWith (textureExtensions [j])) {
					selection.Add (selectionArray [i]);
				}
			}
		}

        // Bring up save panel
        string saveLocation = EditorUtility.SaveFilePanel ("Build Vegetation Pack", "", folder.name + "_v", "unity3d");
        if (saveLocation.Length != 0) {
            // Build the resource file from the active selection.
			BuildTextureAssetBundle( selection.ToArray(), saveLocation );
        }
    }
	
	[MenuItem("Axis Game Factory/Build Asset Pack/Skybox")]
    static void ExportSkyboxBundle () {
		Object folder = Selection.activeObject;
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		
        // Bring up save panel
        string saveLocation = EditorUtility.SaveFilePanel ("Build Skybox Pack", "", folder.name + "_s", "unity3d");
        if (saveLocation.Length != 0) {
            
			// find all material files within the selection. 
			List<Object> materials = new List<Object>();
			for ( int i = 0; i < selection.Length; i++ ){
				string path = AssetDatabase.GetAssetPath( selection[i] );
				if ( path.EndsWith(".mat") ){	
					materials.Add ( selection[i] );
				}
			}
		
			// for each material, create a texture from one of the cubemap faces.
			if ( Directory.Exists( Application.dataPath + "/AGF_Intermediate/Skybox_Previews" ) == false ){
				Directory.CreateDirectory( Application.dataPath + "/AGF_Intermediate/Skybox_Previews" );
			}
				
			List<string> texturesPaths = new List<string>();
			for ( int i = 0; i < materials.Count; i++ ){
				Material mat = (Material)materials[i];
				if (mat.GetTexture ("_Tex")) {
					Cubemap cubemap = (Cubemap)mat.GetTexture ("_Tex");
					Texture2D cubemapTopFace = new Texture2D (cubemap.width, cubemap.height, TextureFormat.RGB24, false);
					cubemapTopFace.SetPixels (cubemap.GetPixels (CubemapFace.PositiveZ));
				
					// rotate the image.
					cubemapTopFace = Main.RotateTexture (cubemapTopFace, 270.0f);
					byte[] bytes = cubemapTopFace.EncodeToPNG ();
				
					File.WriteAllBytes (Application.dataPath + "/AGF_Intermediate/Skybox_Previews/" + mat.name + ".png", bytes);
					texturesPaths.Add ("Assets/AGF_Intermediate/Skybox_Previews/" + mat.name + ".png");
				} else {
					Debug.LogWarning (mat.name + " did not have the correct shader. Skipping. Try using AGF/Skybox Cubed Rotate");
				}
			}
			
			AssetDatabase.Refresh();
			
			// convert the list into an array.
			Object[] objects = new Object[materials.Count + texturesPaths.Count];
			for ( int i = 0; i < materials.Count; i++ ){
				objects[i] = materials[i];
			}
			for ( int i = materials.Count; i < objects.Length; i++ ){
				objects[i] = AssetDatabase.LoadAssetAtPath( texturesPaths[i - materials.Count], typeof(Texture2D) );
			}
			
			// Create an array of names for the assets placed into the bundle.
			string[] objectNames = new string[objects.Length];
			for ( int i = 0; i < objectNames.Length; i++ ){
				string prefabPath = AssetDatabase.GetAssetPath( objects[i] );
				
				Debug.Log( prefabPath );

				objectNames [i] = prefabPath;
//				string[] split = prefabPath.Split ( new char[]{'/'} );
//				objectNames[i] = split[split.Length-1];
			}

			AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

			string[] assetPaths = new string[texturesPaths.Count];
			for (int i = 0; i < texturesPaths.Count; i++) {
				assetPaths [i] = texturesPaths [i];
			}


			string[] saveSplit = saveLocation.Split( new char[]{'/'} );
			string bundleName = saveSplit[saveSplit.Length-1];

			buildMap [0].assetBundleName = bundleName;
			buildMap [0].assetNames = objectNames;

			// Remove the file name from the save path
			saveLocation = saveLocation.Remove (saveLocation.LastIndexOf("/"));
			BuildPipeline.BuildAssetBundles (saveLocation, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

//			BuildPipeline.BuildAssetBundleExplicitAssetNames( objects, objectNames, saveLocation, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows );
	        // Selection.objects = objects;
			
			// make sure the new bundle shows up in the project window.
			AssetDatabase.Refresh();
        }
    }
	
	private static void BuildTextureAssetBundle( Object[] selection, string saveLocation ){
		// find all texture files within the selection. 
		List<Object> textures = new List<Object>();
		for ( int i = 0; i < selection.Length; i++ ){
			string path = AssetDatabase.GetAssetPath( selection[i] );
			for ( int j = 0; j < textureExtensions.Length; j++ ){
				if ( path.EndsWith(textureExtensions[j]) ){
					textures.Add ( selection[i] );
				}
			}
		}
		
		// convert the list into an array.
		Object[] objects = new Object[textures.Count];
		for ( int i = 0; i < objects.Length; i++ ){
			objects[i] = textures[i];
		}
		
		// Create an array of names for the assets placed into the bundle.
		string[] objectNames = new string[objects.Length];
		for ( int i = 0; i < objectNames.Length; i++ ){
			string prefabPath = AssetDatabase.GetAssetPath( objects[i] );
			
			Debug.Log( prefabPath );
			objectNames [i] = prefabPath;
//			string[] split = prefabPath.Split ( new char[]{'/'} );
//			objectNames[i] = split[split.Length-1];
		}

		AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

		string[] saveSplit = saveLocation.Split( new char[]{'/'} );
		string bundleName = saveSplit[saveSplit.Length-1];

		buildMap [0].assetBundleName = bundleName;
		buildMap [0].assetNames = objectNames;

		// Remove the file name from the save path
		saveLocation = saveLocation.Remove (saveLocation.LastIndexOf("/"));
		BuildPipeline.BuildAssetBundles (saveLocation, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
		
//		BuildPipeline.BuildAssetBundleExplicitAssetNames( objects, objectNames, saveLocation, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows );
        // Selection.objects = objects;
		
		// make sure the new bundle shows up in the project window.
		AssetDatabase.Refresh();
	}

	[MenuItem("Axis Game Factory/Build Asset Pack/Audio")]
	static void ExportAudioBundle () {
		Object folder = Selection.activeObject;
		Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

		// Bring up save panel
		string saveLocation = EditorUtility.SaveFilePanel ("Build Audio Pack", "", folder.name + "_a", "unity3d");
		if (saveLocation.Length != 0) {
			// find all texture files within the selection. 
			List<Object> audios = new List<Object>();
			for ( int i = 0; i < selection.Length; i++ ){
				string path = AssetDatabase.GetAssetPath( selection[i] );
				for ( int j = 0; j < audioExtensions.Length; j++ ){
					if ( path.EndsWith(audioExtensions[j]) ){
						audios.Add ( selection[i] );
					}
				}
			}

			// convert the list into an array.
			Object[] objects = new Object[audios.Count];
			for ( int i = 0; i < objects.Length; i++ ){
				objects[i] = audios[i];
			}

			// Create an array of names for the assets placed into the bundle.
			string[] objectNames = new string[objects.Length];
			for ( int i = 0; i < objectNames.Length; i++ ){
				string prefabPath = AssetDatabase.GetAssetPath( objects[i] );

				Debug.Log( prefabPath );
				objectNames [i] = prefabPath;
				//			string[] split = prefabPath.Split ( new char[]{'/'} );
				//			objectNames[i] = split[split.Length-1];
			}

			AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

			string[] saveSplit = saveLocation.Split( new char[]{'/'} );
			string bundleName = saveSplit[saveSplit.Length-1];

			buildMap [0].assetBundleName = bundleName;
			buildMap [0].assetNames = objectNames;

			// Remove the file name from the save path
			saveLocation = saveLocation.Remove (saveLocation.LastIndexOf("/"));
			BuildPipeline.BuildAssetBundles (saveLocation, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

			//		BuildPipeline.BuildAssetBundleExplicitAssetNames( objects, objectNames, saveLocation, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows );
			// Selection.objects = objects;

			// make sure the new bundle shows up in the project window.
			AssetDatabase.Refresh();
		}
	}


}
