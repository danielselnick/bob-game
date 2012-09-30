using UnityEngine;
using System.Collections.Generic;


public class CharacterMovement : MonoBehaviour {
	
	public Camera playerCam;
	
	public float gravity;
	
	public float jumpSpeed;
	
	public float speed;
	
	const int FORWARD = 1;
	
	const int BACKWARD = 0;
	
	private Rigidbody playerRigidbody;
	
	private Transform playerTransform;
	
	private float timePassed;
	
	private Vector3 moveDirection = Vector3.zero;
	
	private Vector3 playerNormals;
	
	private float jumpTime = 0.7f;
	
	private float jumpTimer;
	
	private bool isGrounded;
	
	private List<GameObject> CollisionObjects;
	
	//public Transform leftArm;
	
	//private Vector3 mouseVector;
	
	//public Animation bobAnimation;
	
	// Use this for initialization
	void Start () 
	{	
		//mouseVector = new Vector3();
		moveDirection = new Vector3();
		playerTransform = GetComponent<Transform>();
		playerRigidbody = GetComponent<Rigidbody>();
		CollisionObjects =  new List<GameObject>();
		timePassed = 0;
	}
	
	private bool canJump()
	{
		if(Input.GetKeyDown("space") && jumpTimer >= jumpTime && CollisionObjects.Count != 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{	
		
		//mouseVector = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);
		
		//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
   		//RaycastHit hit;

       // if (Physics.Raycast(ray, out hit, 100))
		//{
            //Debug.DrawLine(ray.origin, hit.point);
		//	leftArm.LookAt(hit.point);
		//}
        
    
		
		//if(Input.GetMouseButton(0))
		//{
			
		//}
		
		
		//Debug.Log(Vector3.Dot(leftArm.rotation.eulerAngles,mouseVector));
		
		
		if(canJump())
		{
			playerRigidbody.AddForce(Vector3.up * jumpSpeed);
		}	
		
		//Smooth speed
		if(CollisionObjects.Count == 0)
		{
			moveDirection.x = Mathf.Lerp(moveDirection.x,0,timePassed*15);	
			playerRigidbody.AddForce(moveDirection);
		}
		
		else
		{
			moveDirection.x = Input.GetAxis("Horizontal")*speed;
			playerRigidbody.AddForce(moveDirection);
		}
		
		jumpTimer+= Time.fixedDeltaTime;
		
		if(playerCam == null)
		{
			Debug.LogError("Hey, Listen! Drag the Camera object in the Hierarchy View onto the slot for the camera!");
		}
		else
		{
			playerCam.transform.position  = new Vector3(transform.position.x,transform.position.y,playerCam.transform.position.z);
		}
		timePassed+=Time.deltaTime;
	}
	
	void OnCollisionEnter(Collision collisionInfo)
	{
		if(collisionInfo.collider.gameObject.layer == 8)
		{
			if(collisionInfo.contacts[0].normal.y > 0.5f)
			{
				CollisionObjects.Add(collisionInfo.transform.gameObject);
			}
		}
	}
	void OnCollisionExit(Collision collisionInfo)
	{
		if(CollisionObjects.Contains(collisionInfo.collider.gameObject))
		{
			CollisionObjects.Remove(collisionInfo.collider.gameObject);
		}
	}
}



//FLIPPING CODE
//		if(moveDirection.x > 0)
//		{
//			playerTransform.localScale = new Vector3(1.0f,1,1);
//			timePassed+=Time.fixedDeltaTime;	
//		}
//		else if(moveDirection.x < 0)
//		{		
//			playerTransform.localScale = new Vector3(-1.0f,1,1);
//		}
//		else
//		{
//			timePassed = 0;			
//		}
//		moveDirection = transform.TransformDirection(moveDirection);
//		moveDirection *= speed;