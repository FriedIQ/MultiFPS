using UnityEngine;
using System.Collections;

/*
 * 	This component is only enabled for the LOCAL player.
 * 
 */
 
public class PlayerMovement : MonoBehaviour {

	float speed = 10f;
	float jumpSpeed = 5.5f;

	Vector3 direction = Vector3.zero;	// WASD forward/back, left/right direction stored in direction
	float verticalVelocity = -0.01f;

	float colliderFullHeight = 2.0f;
	float colliderJumpHeight = 1.3f;

	CharacterController controller;
	Animator anim;
	//CapsuleCollider capsuleCollider;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		anim = GetComponent<Animator>();
		//capsuleCollider = (CapsuleCollider)collider;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// WASD forward/back, left/right direction stored in direction
		direction = transform.rotation * new Vector3( Input.GetAxis("Horizontal"), 0, Input.GetAxis ("Vertical"));

		if( direction.magnitude > 1.0f )
		{
			direction = direction.normalized;
		}

		anim.SetFloat( "Speed", direction.magnitude );

		if( controller.isGrounded && Input.GetButtonDown( "Jump" ) )
		{
			verticalVelocity = jumpSpeed;
			Debug.Log (verticalVelocity);
		}
	}

	// Fixed update is called once per physic loop.
	// Do all MOVEMENT and other physics stuff here.
	void FixedUpdate () 
	{
		Vector3 distance = direction * speed * Time.deltaTime;

		if( controller.isGrounded && verticalVelocity < 0 )
		{
			anim.SetBool( "Jumping", false );
			verticalVelocity = Physics.gravity.y * Time.deltaTime;
		}
		else
		{
			if( Mathf.Abs(verticalVelocity) > jumpSpeed * 0.75f )
			{
				anim.SetBool( "Jumping", true );
			}

			verticalVelocity += Physics.gravity.y * Time.deltaTime;
		}

		distance.y = verticalVelocity * Time.deltaTime;

		controller.Move(distance);
	}
}
