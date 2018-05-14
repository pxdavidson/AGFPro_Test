using UnityEngine;
using System.Collections;

public class PickupProperties : TileProperties {
	
	// Pickups function similarly to all other tiles, except for their additional in-game functions:
	// 1. When colliding with the player, they play a sound, add to a score, and are deleted.
	// 2. While in game, they constantly rotate.
	public Texture2D pickupTexture;
	public float lifeTimer = -1.0f;
	public int pointValue;
}
