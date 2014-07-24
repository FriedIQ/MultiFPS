using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public bool offlineMode = false;
	//public Camera standByCamera;

	// Use this for initialization
	void Start () {
		Connect();
	}

	void Connect()
	{
		if( offlineMode )
		{
			PhotonNetwork.offlineMode = offlineMode;
			OnJoinedLobby ();
		}
		else
		{
			PhotonNetwork.ConnectUsingSettings( "0.0.1" );
		}
	}

	void OnJoinedLobby()
	{
		Debug.Log ( "OnJoinedLobby" );
		PhotonNetwork.JoinRandomRoom(); 
	}

	void OnPhotonRandomJoinFailed()
	{
		Debug.Log ( "OnPhotonRandonJoinFailed" );
		PhotonNetwork.CreateRoom( null );
	}

	void OnJoinedRoom()
	{
		Debug.Log ( "OnJoinedRoom" );

		SpawnPlayer();
	}

	void SpawnPlayer()
	{
		SpawnPoint spawnPoint = GameObject.FindObjectOfType<SpawnPoint>();
		GameObject localPlayer = (GameObject)PhotonNetwork.Instantiate( "First Person Controller", spawnPoint.transform.position, spawnPoint.transform.rotation, 0 );

		// Enable components on the local objects
		localPlayer.GetComponent<PlayerMovement>().enabled = true;
		localPlayer.GetComponent<PlayerShoot>().enabled = true;
		localPlayer.GetComponent<MouseLook>().enabled = true;
		//localPlayer.GetComponent<FPSInputController>().enabled = true;

		// Enable the local camera
		localPlayer.GetComponentInChildren<Camera>().enabled = true;
		localPlayer.GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = true;
		((Behaviour)localPlayer.GetComponentInChildren<Camera>().GetComponent( "FlareLayer" )).enabled = true;
		localPlayer.GetComponentInChildren<Camera>().GetComponent<GUILayer>().enabled = true;
		//localPlayer.GetComponentInChildren<Camera>().GetComponent<MouseLook>().enabled = true;
	}

	void OnGUI()
	{
		GUILayout.Label( PhotonNetwork.connectionState.ToString() );
	}
}
