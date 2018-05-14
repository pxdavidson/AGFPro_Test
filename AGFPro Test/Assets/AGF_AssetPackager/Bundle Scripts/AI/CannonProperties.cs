using UnityEngine;
using System.Collections;

public class CannonProperties : TileProperties {

	public Transform projectile;
	
	// cooldown timer
	public float delayTimerDuration = 5.0f;
	public Vector3 fireAnimationOffsetPosition = new Vector3(0,1.15f,2.5f);
	public float fireAnimationOffsetTimer = 0.0f;
	public float maximumFireDistance = 30.0f;
}
