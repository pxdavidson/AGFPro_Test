using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileProperties : MonoBehaviour {
	public int tileLimit = -1;
	public bool useYOffset = false;
	
	public Vector3 size;
	[HideInInspector] public Vector3 storedScale = new Vector3(1,1,1); // scale attribute. custom blocks may be saved higher or lower than 1,1,1. Default blocks will always be 1,1,1.
	public bool scalable = true;
	public bool keepUpright = false;
	public bool placeableInBuildMode = true;
	
	public float yOffset = 0.0f;
	public string category;
	public bool isIndestructible = false;
	public Texture2D tileTexture;
	public Bounds tileBounds;
	
	public Vector3 m_InitialCenterOffset;
	public Vector3 m_CenterOffset;
	
	public string tileID;
	
	public Vector3 GetSize(){
		return size;	
	}
	
	public Bounds GetLocalBounds(){
		if ( transform.parent ){
			return GetParent ().GetComponent<TileProperties>().GetLocalBounds();
		} else {
			return this.tileBounds;	
		}
	}
	
	public Transform GetParent(){
		if ( transform.parent ) {
			return transform.parent.GetComponent<TileProperties>().GetParent ();	
		} else {
			return transform;	
		}
	}
}
 