using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PhotonView))]
public class NetworkManager : MonoBehaviour
{
    public bool OfflineMode = false;
    public GameObject MenuCamera;
    public float RespawnTimer = 0;

    private PhotonView _photonView;
    private List<string> _chatMessages;
    private const int ChatBufferSize = 5;

    // Use this for initialization
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        PhotonNetwork.player.name = PlayerPrefs.GetString("PlayerName", "Unknown Player");
        _chatMessages = new List<string>();
    }

    private void Update()
    {
        if (!(RespawnTimer > 0)) return;

        RespawnTimer -= Time.deltaTime;

        if (!(RespawnTimer <= 0)) return;

        RespawnTimer = 0;
    }

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("0.0.1");
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetString("PlayerName", PhotonNetwork.player.name);
    }

    private void OnJoinedLobby()
    {
        //Debug.Log ( "OnJoinedLobby" );
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnPhotonRandomJoinFailed()
    {
        //Debug.Log ( "OnPhotonRandonJoinFailed" );
        PhotonNetwork.CreateRoom(null);
    }

    private void OnJoinedRoom()
    {
        Debug.Log ( "OnJoinedRoom" );
        SpawnPlayer();
    }

    public void AddChatMessage(string text)
    {
        if (_photonView != null)
        {
            _photonView.RPC("AddChatMessageRpc", PhotonTargets.AllBuffered, text);
        }
        else
        {
            Debug.Log("photonView is null");
        }
    }

    [RPC]
    private void AddChatMessageRpc(string text)
    {
        if (_chatMessages.Count >= ChatBufferSize)
        {
            _chatMessages.RemoveAt(0);
        }
        _chatMessages.Add(text);
    }

    private void SpawnPlayer()
    {
        var spawnPoints = FindObjectsOfType<SpawnPoint>();

        var point = Random.Range(1, spawnPoints.Length);
        MenuCamera.SetActive(false);

        PhotonNetwork.Instantiate("First Person Controller", spawnPoints[point].transform.position,
            spawnPoints[point].transform.rotation, 0);

        AddChatMessage("Player " + PhotonNetwork.player.name + " has joined the game.");

        Screen.lockCursor = true;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginVertical();
        GUILayout.Label(PhotonNetwork.connectionState.ToString());
        GUILayout.Label("IsMasterClient: " + PhotonNetwork.isMasterClient.ToString());
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();

        // We are NOT connected nor are we currently ATTEMPTING to connect.
        if (!PhotonNetwork.connected && !PhotonNetwork.connecting)
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Single Player"))
            {
                PhotonNetwork.offlineMode = true;
                OnJoinedLobby();
            }
            if (GUILayout.Button("Multi Player"))
            {
                Connect();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        // We are FULLY connected.
        if (PhotonNetwork.connected && !PhotonNetwork.connecting)
        {

                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                foreach (var m in _chatMessages)
                {
                    GUILayout.Label(m);
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
        }

        // Display a countdown if the respawn timer is active
        if (RespawnTimer > 0)
        {
            Screen.lockCursor = false;
            
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label( "Respawning in: " + Mathf.RoundToInt(RespawnTimer) + " seconds." );

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}