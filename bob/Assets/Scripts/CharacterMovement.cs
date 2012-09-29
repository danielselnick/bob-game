using UnityEngine;
using System.Collections;


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
	
	
	// Use this for initialization
	void Start () {
		
		moveDirection = new Vector3();
		playerTransform = this.GetComponent<Transform>();
		timePassed = 0;

	}
	
	// Update is called once per frame
	void Update () {
		
		if(controller.isGrounded)
		{
			
			moveDirection.x = Input.GetAxis("Horizontal");
			if(moveDirection.x > 0)
			{
				playerTransform.localScale = new Vector3(1.0f,1,1);
				
				timePassed+=Time.fixedDeltaTime;
				
			}
			else if(moveDirection.x < 0)
			{		
				playerTransform.localScale = new Vector3(-1.0f,1,1);
			}
			else
			{
				timePassed = 0;			
			}
			moveDirection.y = 0;
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if(Input.GetButton("Jump"))
			{
				moveDirection.y = jumpSpeed;
			}		
		}
		else
		{

			moveDirection.x = Input.GetAxis("Horizontal");
			moveDirection.x *= speed;
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move (moveDirection*Time.deltaTime);
	}
}
