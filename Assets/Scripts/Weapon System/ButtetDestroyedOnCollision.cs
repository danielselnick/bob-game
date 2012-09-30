using UnityEngine;
using System.Collections;

public class ButtetDestroyedOnCollision : MonoBehaviour
{
	void OnCollisionEnter(Collision collision) 
	{
        Destroy(gameObject);
    }
	
	void OnTriggerEnter(Collider other)
	{
		Destroy(gameObject);
	}
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

