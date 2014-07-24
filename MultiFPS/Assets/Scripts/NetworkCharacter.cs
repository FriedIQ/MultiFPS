using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	// float lastUpdateTime

	Animator animator;

	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(photonView.isMine)
		{
			// Do nothing
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
		}
	}

	public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		if(stream.isWriting)
		{
			// This is the LOCAL player. We need to send our actual position to the server.
			stream.SendNext ( transform.position );
			stream.SendNext ( transform.rotation );

			stream.SendNext ( animator.GetFloat("Speed") );
			stream.SendNext ( animator.GetBool( "Jumping" ) );
		}
		else
		{
			// This is a NETWORK player. We need to recieve their position and update the LOCAL version of that player.
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();

			animator.SetFloat( "Speed", (float)stream.ReceiveNext() );
			animator.SetBool( "Jumping", (bool)stream.ReceiveNext() );
		}
	}
}
