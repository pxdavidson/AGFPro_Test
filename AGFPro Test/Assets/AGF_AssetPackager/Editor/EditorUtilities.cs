using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorUtilities : MonoBehaviour {
	
	public static Texture2D GenerateRenderTexture( GameObject target, Camera newCamera, string destinationPath, string loadPath, Bounds bounds, TextureFormat format ){
		// generate a render texture. to do this, we must first instantiate the block.
		GameObject newTile = (GameObject)Instantiate(target);
		
		Vector3 lookAt = bounds.center;
		
		Main.SetDefaultFrameCameraRotation( newCamera, lookAt );
		Main.FrameCameraToLocalBounds( newCamera, newTile.transform, lookAt, bounds );
		
		// render out the texture.
		RenderTexture renderTexture = new RenderTexture(128, 128, 24);
		newCamera.targetTexture = renderTexture;
		newCamera.Render();
		
		RenderTexture.active = renderTexture;
		Texture2D saveTexture = new Texture2D(renderTexture.width, renderTexture.height, format, false);
		saveTexture.ReadPixels(new Rect(0,0,renderTexture.width,renderTexture.height), 0, 0);
		saveTexture.Apply();
		
		byte[] bytes = saveTexture.EncodeToPNG();
		DestroyImmediate(saveTexture);
		
		RenderTexture.active = null;
		newCamera.targetTexture = null;
		DestroyImmediate(renderTexture);
		
		// before creating the file, we need to check to see if it exists. if so, make sure it is not read-only.
		if ( File.Exists(destinationPath) ){
			FileInfo tileTexture = new FileInfo(destinationPath);
			tileTexture.IsReadOnly = false;
		}
		
		// write out to the file.
		File.WriteAllBytes(destinationPath, bytes);
		
		// import the asset. make sure that no mip maps are generated.
		print ( "Texture Importer: " + loadPath );
		AssetDatabase.ImportAsset(loadPath);
		
		TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(loadPath);
		importer.mipmapEnabled = false;
		AssetDatabase.ImportAsset(loadPath, ImportAssetOptions.ForceUpdate);
		
		// destroy the block.
		DestroyImmediate(newTile.gameObject);
		
		// save the texture to the tile.
		return (Texture2D)AssetDatabase.LoadAssetAtPath(loadPath,typeof(Texture2D));
	}
}
