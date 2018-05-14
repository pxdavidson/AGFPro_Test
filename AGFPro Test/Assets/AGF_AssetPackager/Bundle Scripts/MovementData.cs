using UnityEngine;
using System.Collections;

// small class consisting of all variables defining a character's movement.
[System.Serializable]
public class MovementData{
	
	// all values are in m/s.
	public float rotationSpeed = 2.0f;
	public float movementSpeed = 2.0f;
	public float gravity = -20.0f;
	
	// jump
	public float jumpHeight = 2.0f;
}
