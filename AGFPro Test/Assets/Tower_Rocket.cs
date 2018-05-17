using System;
using UnityEngine;

public class Tower_Rocket : MonoBehaviour
{

    // Variables
    Rigidbody rigidBody;
    [SerializeField] float rcsThrust = 30f;
    [SerializeField] float mainThrust = 100f;

    // Use this for initialization
    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        RocketControl();
	}

    private void RocketControl()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidBody.AddForce(Vector3.up * mainThrust);
        }
    }
}
