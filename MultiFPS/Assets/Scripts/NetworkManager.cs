using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkManager : MonoBehaviour {

	public bool OfflineMode = false;
	public Camera MenuCamera;

    private PhotonView photonView;
    private List<string> chatMessages;
    private int chatBufferSize = 5;

	// Use this for initialization
	void Start () {
        photonView = GetComponent<PhotonView>();
        PhotonNetwork.player.name = PlayerPrefs.GetString( "PlayerName", "Unknown Player" );
        chatMessages = new List<string>();
	}

	void Connect()
	{
		PhotonNetwork.ConnectUsingSettings( "0.0.1" );
	}

    void OnDestroy()
    {
        PlayerPrefs.SetString( "PlayerName", PhotonNetwork.player.name );
    }

	void OnJoinedLobby()
	{
		//Debug.Log ( "OnJoinedLobby" );
		PhotonNetwork.JoinRandomRoom(); 
	}

	void OnPhotonRandomJoinFailed()
	{
		//Debug.Log ( "OnPhotonRandonJoinFailed" );
		PhotonNetwork.CreateRoom( null );
	}

	void OnJoinedRoom()
	{
		//Debug.Log ( "OnJoinedRoom" );

		SpawnPlayer();
	}

    public void AddChatMessage(string text)
    {
        if (photonView != null)
        {
            photonView.RPC("AddChatMessageRpc", PhotonTargets.AllBuffered, text);
        }
        else
        {
            Debug.Log("photonView is null");
        }
    }

    [RPC]
    void AddChatMessageRpc(string text)
    {
        if (chatMessages.Count >= chatBufferSize)
        {
            chatMessages.RemoveAt(0);
        }
        chatMessages.Add(text);
    }

	void SpawnPlayer()
	{
		var spawnPoints = FindObjectsOfType<SpawnPoint>();

	    var point = UnityEngine.Random.Range(1, spawnPoints.Length);
        PhotonNetwork.Instantiate("First Person Controller", spawnPoints[point].transform.position, spawnPoints[point].transform.rotation, 0);
	    MenuCamera.gameObject.SetActive(false);

        AddChatMessage("Player " + PhotonNetwork.player.name + " has joined the game.");
	}

	void OnGUI()
	{
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginVertical();
		GUILayout.Label( PhotonNetwork.connectionState.ToString() );
        GUILayout.Label("IsMasterClient: " + PhotonNetwork.isMasterClient.ToString());
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (!PhotonNetwork.connected && !PhotonNetwork.connecting)
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            PhotonNetwork.player.name = GUILayout.TextField( PhotonNetwork.player.name );
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Single Player"))
            {
                PhotonNetwork.offlineMode = true;
                OnJoinedLobby();
            }
            if( GUILayout.Button( "Multi Player" ) )
            {
                Connect();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        if (PhotonNetwork.connected && !PhotonNetwork.connecting)
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            foreach (var m in chatMessages)
            {
                GUILayout.Label(m);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
	}
}
