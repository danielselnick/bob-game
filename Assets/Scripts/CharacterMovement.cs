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
	public Vector3 PlayerCameraOffset = new Vector3(0, 3, 3);
	
	private List<GameObject> _collisionObjects;
	private float _timeSinceLastJump;
	private float _rotationY;
	
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

	// Update is called once per frame
	void Update ()
	{
		HandleJumpAction();
		HandleHorizontalMovement();
		PlayerCamera.transform.position = new Vector3(transform.position.x + PlayerCameraOffset.x,
													  transform.position.y + PlayerCameraOffset.y,
													  transform.position.z + PlayerCameraOffset.z);
		if(rigidbody.velocity.magnitude > 1)
		{
			if(animation != null)
			{
				if(!animation.isPlaying)
				{
					animation.Play(AnimationPlayMode.Mix);
				}
			}
		}
		else
		{
			if(animation != null)
			{
				animation.Stop();
			}
		}
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
		
		if(horizontalInput > 0.01)
		{
			_rotationY = 90;
		}
		else if(horizontalInput < -0.01)
		{
			_rotationY = 270;
		}
		rigidbody.MoveRotation(Quaternion.Euler(0, _rotationY, 0));
		var force = new Vector3(horizontalInput * acceleration, 0, 0);
		rigidbody.AddForce(force);
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