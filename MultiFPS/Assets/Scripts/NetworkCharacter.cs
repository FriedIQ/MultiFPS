using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class NetworkCharacter : Photon.MonoBehaviour 
{
    private const float _lerpFactor = 8.0f;

    private Vector3 _realPosition = Vector3.zero;
    private Quaternion _realRotation = Quaternion.identity;

    private Animator _animator;
    private bool _spawning = true;

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

            //Debug.Log("Instantiating Local Player: " + photonView.viewID + "[" + photonView.instantiationId + "]");
        }
        else
        {
            //Debug.Log("Instantiating Remote Player: " + photonView.viewID + "[" + photonView.instantiationId + "]");
        }

        _animator = GetComponent<Animator>();

        // Debug.Log("IsNonMasterClientInRoom: " + PhotonNetwork.isNonMasterClientInRoom);
    }

	// Use this for initialization
    //void Start () 
    //{

    //}
	
	// Update is called once per frame
    // ReSharper disable once UnusedMember.Local
	void Update () 
	{
		if(photonView.isMine)
		{
			// Do nothing
		}
		else
		{
            transform.position = Vector3.Lerp(transform.position, _realPosition, Time.deltaTime * _lerpFactor);
            transform.rotation = Quaternion.Lerp(transform.rotation, _realRotation, Time.deltaTime * _lerpFactor);
		}
	}

	public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		if(stream.isWriting)
		{
			// This is the LOCAL player. We need to send our actual position to the server.

			stream.SendNext ( transform.position );
			stream.SendNext ( transform.rotation );

			stream.SendNext ( _animator.GetFloat("Speed") );
			stream.SendNext ( _animator.GetBool( "Jumping" ) );
		}
		else
		{
			// This is a NETWORK player. We need to recieve their position and update the LOCAL version of that player.

            //transform.position = realPosition;
            //transform.rotation = realRotation;

			_realPosition = (Vector3)stream.ReceiveNext();
			_realRotation = (Quaternion)stream.ReceiveNext();

			_animator.SetFloat( "Speed", (float)stream.ReceiveNext() );
			_animator.SetBool( "Jumping", (bool)stream.ReceiveNext() );

		    if (!_spawning) return;
		    transform.position = _realPosition;
		    transform.rotation = _realRotation;
		    _spawning = false;
		}
	}
}
