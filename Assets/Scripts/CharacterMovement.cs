using UnityEngine;
using System.Collections.Generic;

public class CharacterMovement : MonoBehaviour 
{
	public float JumpAcceleration = 200f;
	public float OnGroundAcceleration = 30f;
	public float InAirAcceleration = 5f;
	
	public float MaximumVelocityMagnitude = 10f;
	public float JumpCoolDown = 2f;
	public Camera PlayerCamera;
	
	private List<GameObject> _collisionObjects;
	private float _timeSinceLastJump;
	
	// Use this for initialization
	void Start () 
	{
		_collisionObjects = new List<GameObject>();
	}
	
	void OnCollisionEnter(Collision collisionInfo)
	{
		if(!_collisionObjects.Exists(x => x == collisionInfo.gameObject))
		{
			_collisionObjects.Add(collisionInfo.gameObject);	
		}
	}
	
	void OnCollisionExit(Collision collisionInfo)
	{
		_collisionObjects.Remove(collisionInfo.gameObject);
	}

	void HandleHorizontalMovement ()
	{
		if(rigidbody.velocity.magnitude > MaximumVelocityMagnitude)
		{
			return;
		}
		
		var horizontalInput = Input.GetAxis("Horizontal");
		float acceleration;
		// Modifier for if we're in the air.
		if(_collisionObjects.Count > 0)
		{
			acceleration = OnGroundAcceleration;
		}
		else
		{
			acceleration = InAirAcceleration;
		}
		var force = new Vector3(horizontalInput * acceleration, 0, 0);
		rigidbody.AddForce(force);
	}
	
	// Update is called once per frame
	void Update ()
	{
		HandleJumpAction();
		HandleHorizontalMovement();
		PlayerCamera.transform.position 
			= new Vector3(transform.position.x, transform.position.y, PlayerCamera.transform.position.z);
	}
	
	private void HandleJumpAction()
	{
		_timeSinceLastJump += Time.fixedDeltaTime;
		if(_timeSinceLastJump < JumpCoolDown)
		{
			return;
		}
		// Jump Up
		if ((_collisionObjects.Count > 0)
			&& (Input.GetKeyDown(KeyCode.UpArrow)
				|| Input.GetKeyDown(KeyCode.W)))
		{
			rigidbody.AddForce(Vector3.up * JumpAcceleration);
		}
		// Force down
		if(Input.GetKeyDown(KeyCode.DownArrow)
			|| Input.GetKeyDown(KeyCode.S))
		{
			rigidbody.AddForce(Vector3.down * JumpAcceleration);
		}
	}
}