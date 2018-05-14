using UnityEngine;
using System.Collections;

public class AssetPackagerSettings : MonoBehaviour {
	public Camera scriptCamera;
	public Transform tileTextureLighting;
	public bool colliderConvex = false;
	public bool addRigidbodys = false;
	public bool GetColliderConvex(){
		return colliderConvex;
	}
}
