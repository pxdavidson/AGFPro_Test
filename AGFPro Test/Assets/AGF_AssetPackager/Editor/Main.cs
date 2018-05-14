using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		string[] args = System.Environment.GetCommandLineArgs();	
		print ("Current command line arguments:");
		for ( int i = 0; i < args.Length; i++ ){
			print ( args[i] );	
		}
		print ("/end command line args");
	}
	
	public static void SetLayerRecursively( Transform t, int layer ){
		foreach (Transform child in t){
			if ( t.transform == child ) {
				continue;
			}
			SetLayerRecursively( child, layer );
		}
		
		t.gameObject.layer = layer;
	}
	
	public static void GetBoundsRecursively( Transform t, ref Bounds bounds ){
		foreach (Transform child in t){
			if ( t.transform == child ) {
				continue;
			}
			GetBoundsRecursively( child, ref bounds );
		}
		if ( t.GetComponent<Renderer>() ){
			bounds.Encapsulate(t.GetComponent<Renderer>().bounds);
		}
	}
	
	public static void GetMeshBoundsRecursively( Transform t, ref Bounds bounds ){
		foreach (Transform child in t){
			if ( t.transform == child ) {
				continue;
			}
			GetBoundsRecursively( child, ref bounds );
		}
		if ( t.GetComponent<Renderer>() ){
			bounds.Encapsulate(t.GetComponent<MeshFilter>().mesh.bounds);
		}
	}
	
	public static void AddComponentRecursively( Transform t, string componentType ){
		foreach ( Transform child in t ) {
			if ( child == t.transform ){
				continue;	
			}
			
			AddComponentRecursively( child, componentType );
		}

		if (componentType == "PickupProperties" && !t.GetComponent<PickupProperties> ())
			t.gameObject.AddComponent<PickupProperties> ();
		else if (componentType == "LightProperties" && !t.GetComponent<LightProperties> ())
			t.gameObject.AddComponent<LightProperties> ();
		else if (componentType == "EffectProperties" && !t.GetComponent<EffectProperties> ())
			t.gameObject.AddComponent<EffectProperties> ();
		else if (componentType == "CannonProperties" && !t.GetComponent<CannonProperties> ())
			t.gameObject.AddComponent<CannonProperties> ();
		else if (componentType == "TileProperties" && !t.GetComponent<TileProperties> ())
			t.gameObject.AddComponent<TileProperties> ();
		else if (componentType == "GibProperties" && !t.GetComponent<GibProperties> ())
			t.gameObject.AddComponent<GibProperties> ();
		else if (componentType == "Rigidbody" && !t.GetComponent<Rigidbody> ())
			t.gameObject.AddComponent<Rigidbody> ();
//			UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(t.gameObject, "Assets/AGF_AssetPackager/Editor/Main.cs (69,4)", componentType);

	}
	
	public static void AddSphereColliderRecursively( Transform t, float radiusScale ){
		foreach ( Transform child in t ) {
			if ( child == t.transform ){
				continue;	
			}
			
			AddSphereColliderRecursively( child, radiusScale );
		}
		
		if ( t.gameObject.GetComponent<SphereCollider>() == null ){
			t.gameObject.AddComponent<SphereCollider>();
			t.gameObject.GetComponent<SphereCollider>().radius *= radiusScale;
		}
	}
	
	public static void SetMeshColliderConvexRecursively( Transform t, bool convex ){
		foreach ( Transform child in t ){
			if ( child == t.transform ){
				continue;	
			}
			
			SetMeshColliderConvexRecursively( child, convex );
		}
		// UNCOMMENT BELOW TO MAKE COLLIDERS CONVEX //
		if ( t.gameObject.GetComponent<MeshCollider>() != null ){
			bool colliderConvex=GameObject.Find("AssetPackagerSettings").GetComponent<AssetPackagerSettings>().GetColliderConvex();
			if (colliderConvex==true){
			t.gameObject.GetComponent<MeshCollider>().convex = convex;
			}
		}
	}
	
	public static void SetTriggerRecursively( Transform t, bool isTrigger ){
		foreach( Transform child in t ){
			if ( child == t.transform ){
				continue;	
			}
			
			SetTriggerRecursively( child, isTrigger );
		}
		
		if ( t.GetComponent<Collider>() ){
			t.GetComponent<Collider>().isTrigger = isTrigger;	
		}
	}
	
	public static void GetTransformsWithComponentRecursively( Transform t, string componentType, ref List<Transform> result ){
		foreach( Transform child in t ){
			if ( child == t.transform ){
				continue;	
			}
			
			GetTransformsWithComponentRecursively( child, componentType, ref result );
		}
		
		if ( t.GetComponent(componentType) != null ){
			result.Add (t);	
		}
	}
	
	public static string TrimEndFromString( string s, string trimString ){
		string result = "";
		if ( s.EndsWith(trimString) ){
			result = s.Substring (0, s.LastIndexOf(trimString) );
		} else {
			result = s;	
		}
		return result;
	}
	
	public static void SetDefaultFrameCameraRotation( Camera cam, Vector3 lookAt ){
		// this step is optional, and only serves to appropriately place the camera in preparation for a photo with rotation 45,45.
		cam.transform.position = lookAt + Vector3.one;
		//Quaternion.AngleAxis(-45, new Vector3(1,0,0)) * new Vector3(0,0,1);
		//cam.transform.position = Quaternion.AngleAxis(45, new Vector3(0,0,1)) * cam.transform.position;
	}
	
	public static bool FrameCameraToLocalBounds( Camera cam, Transform target, Vector3 lookAt, Bounds bounds ){
		// begin by looking at the target, then positioning the camera 100 units backward from it.
		cam.transform.LookAt(lookAt);
		cam.transform.position = lookAt - ( cam.transform.forward * 100.0f );
		
		// grab all the bounding box points.
		Vector3[] boundingBoxPoints = new Vector3[8];
		boundingBoxPoints[0] = target.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, 1, 1)));
		boundingBoxPoints[1] = target.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, 1, 1)));
		boundingBoxPoints[2] = target.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, 1, -1)));
		boundingBoxPoints[3] = target.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, 1, -1)));
		boundingBoxPoints[4] = target.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, -1, 1)));
		boundingBoxPoints[5] = target.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, -1, 1)));
		boundingBoxPoints[6] = target.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, -1, -1)));
		boundingBoxPoints[7] = target.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, -1, -1)));
		
		// -- determine if the height or width is greater. -- //
		int highestYid = -1;
		float hiY = 0.0f;
		for ( int i = 0; i < boundingBoxPoints.Length; i++ ){
			Vector3 viewPoint = cam.WorldToViewportPoint(boundingBoxPoints[i]);
			if ( viewPoint.y >= hiY || i == 0){
				highestYid = i;
				hiY = viewPoint.y;
			}
		}
		
		int lowestYid = -1;
		float lowY = 1.0f;
		for ( int i = 0; i < boundingBoxPoints.Length; i++ ){
			Vector3 viewPoint = cam.WorldToViewportPoint(boundingBoxPoints[i]);
			if ( viewPoint.y <= lowY || i == 0){
				lowestYid = i;
				lowY = viewPoint.y;
			}
		}
		
		float height = hiY - lowY;
		
		int highestXid = -1;
		float hiX = 0.0f;
		for ( int i = 0; i < boundingBoxPoints.Length; i++ ){
			Vector3 viewPoint = cam.WorldToViewportPoint(boundingBoxPoints[i]);
			if ( viewPoint.x >= hiX || i == 0){
				highestXid = i;
				hiX = viewPoint.x;
			}
		}
		
		int lowestXid = -1;
		float lowX = 1.0f;
		for ( int i = 0; i < boundingBoxPoints.Length; i++ ){
			Vector3 viewPoint = cam.WorldToViewportPoint(boundingBoxPoints[i]);
			if ( viewPoint.x <= lowX || i == 0){
				lowestXid = i;
				lowX = viewPoint.x;
			}
		}
		
		float width = hiX - lowX;
		
		// now, we know which value is higher. move the camera closer until the IDs associated with this value are offscreen.
		Vector3 direction = cam.transform.position - lookAt;
		direction.Normalize();
		direction *= 0.5f;
		
		int inc = 0;
		if ( width > height ){
			while ( cam.WorldToViewportPoint(boundingBoxPoints[highestXid]).x < 1.0f &&
				cam.WorldToViewportPoint(boundingBoxPoints[lowestXid]).x > 0.0f ){
				cam.transform.position = cam.transform.position - direction;
				inc++;
				if ( inc > 200 ){
					Debug.LogError("ERROR! Unable to generate image for " + target.name );
					return false;
				}
			}
		} else {
			while ( cam.WorldToViewportPoint(boundingBoxPoints[highestYid]).y < 1.0f &&
				cam.WorldToViewportPoint(boundingBoxPoints[lowestYid]).y > 0.0f ){
				cam.transform.position = cam.transform.position - direction;
				inc++;
				if ( inc > 200 ){
					Debug.LogError("ERROR! Unable to generate image for " + target.name );
					return false;
				}
			}
		}
		
		// move back several steps.
		cam.transform.position = cam.transform.position + ( direction * inc/80 );
		return true;
	}
	
	// Texture Rotation 
	public static Texture2D RotateTexture(Texture2D tex, float angle){
        Texture2D rotImage = new Texture2D(tex.width, tex.height);
        int x,y;
        float x1, y1, x2,y2;
 
        int w = tex.width;
        int h = tex.height;
        float x0 = Rot_x (angle, -w/2.0f, -h/2.0f) + w/2.0f;
        float y0 = Rot_y (angle, -w/2.0f, -h/2.0f) + h/2.0f;
 
        float dx_x = Rot_x (angle, 1.0f, 0.0f);
        float dx_y = Rot_y (angle, 1.0f, 0.0f);
        float dy_x = Rot_x (angle, 0.0f, 1.0f);
        float dy_y = Rot_y (angle, 0.0f, 1.0f);
       
       
        x1 = x0;
        y1 = y0;
 
        for (x = 0; x < tex.width; x++) {
            x2 = x1;
            y2 = y1;
            for ( y = 0; y < tex.height; y++) {
            //rotImage.SetPixel (x1, y1, Color.clear);          
 
            x2 += dx_x;//rot_x(angle, x1, y1);
            y2 += dx_y;//rot_y(angle, x1, y1);
            rotImage.SetPixel ( (int)Mathf.Floor(x), (int)Mathf.Floor(y), GetPixel(tex,x2, y2));
            }
 
            x1 += dy_x;
            y1 += dy_y;
           
        }
 
        rotImage.Apply();
       	return rotImage;
    }
     
    private static Color GetPixel(Texture2D tex, float x, float y)
    {
        Color pix;
        int x1 = (int) Mathf.Floor(x);
        int y1 = (int) Mathf.Floor(y);
 
        if(x1 > tex.width || x1 < 0 ||
           y1 > tex.height || y1 < 0) {
            pix = Color.clear;
        } else {
            pix = tex.GetPixel(x1,y1);
        }
       
        return pix;
    }
 
    private static float Rot_x (float angle, float x, float y) {
        float cos = Mathf.Cos(angle/180.0f*Mathf.PI);
        float sin = Mathf.Sin(angle/180.0f*Mathf.PI);
        return (x * cos + y * (-sin));
    }
    private static float Rot_y (float angle, float x, float y) {
        float cos = Mathf.Cos(angle/180.0f*Mathf.PI);
        float sin = Mathf.Sin(angle/180.0f*Mathf.PI);
        return (x * sin + y * cos);
    }
}
