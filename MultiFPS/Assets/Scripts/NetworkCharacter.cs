using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class NetworkCharacter : Photon.MonoBehaviour {

    private float lerpFactor = 8.0f;
    private float fraction;

    private Vector3 realPosition = Vector3.zero;
    private Quaternion realRotation = Quaternion.identity;

    private Animator animator;
    private bool spawning = true;

    public void Awake()
    {
        if (photonView.isMine)
        {
            name = "Local Player";

            // Enable components on the local objects
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<PlayerShoot>().enabled = true;
            GetComponent<MouseLook>().enabled = true;

            // Enable the local camera
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = true;
            ((Behaviour)GetComponentInChildren<Camera>().GetComponent("FlareLayer")).enabled = true;
            GetComponentInChildren<Camera>().GetComponent<GUILayer>().enabled = true;

            Debug.Log("Instantiating Local Player: " + photonView.viewID + "[" + photonView.instantiationId + "]");
        }
        else
        {
            Debug.Log("Instantiating Remote Player: " + photonView.viewID + "[" + photonView.instantiationId + "]");
        }

        Debug.Log("IsNonMasterClientInRoom: " + PhotonNetwork.isNonMasterClientInRoom);
    }

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
            // fraction = fraction + Time.deltaTime * lerpFactor;
            transform.position = Vector3.Lerp(transform.position, realPosition, Time.deltaTime * lerpFactor);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, Time.deltaTime * lerpFactor);
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

            //transform.position = realPosition;
            //transform.rotation = realRotation;

			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();

			animator.SetFloat( "Speed", (float)stream.ReceiveNext() );
			animator.SetBool( "Jumping", (bool)stream.ReceiveNext() );

            fraction = 0.0f;

            if (spawning)
            {
                transform.position = realPosition;
                transform.rotation = realRotation;
                spawning = false;
            }
		}
	}
}
