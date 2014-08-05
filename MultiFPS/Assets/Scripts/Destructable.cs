using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Destructable : MonoBehaviour {

	public float StructurePoints = 100f;

    PhotonView _photonView;
    float _currentStructurePoints;

	// Use this for initialization
	void Start () {
        _photonView = GetComponent<PhotonView>();
		_currentStructurePoints = StructurePoints;
	}

    //void OnGUI()
    //{
    //    if( photonView.isMine && gameObject.tag == "Player" )
    //    {
    //        if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 40), "Respawn"))
    //        {
    //            BeDestroyed();
    //        }
    //    }
    //}

	[RPC]
	public void TakeDamage( float damage )
	{
		_currentStructurePoints -= damage;

	    if (!(_currentStructurePoints <= 0)) return;

	    BeDestroyed();
	}

	void BeDestroyed()
	{
        if (_photonView.instantiationId == 0)
        {
            Debug.Log("BeDestroyed() called from " + _photonView.instantiationId);
            Destroy(gameObject);
        }
        else
        {
            if (_photonView.isMine)
            {
                //Debug.Log("Called from Master: [" + photonView.instantiationId + "]");
                if (gameObject.tag == "Player")
                {
                    var networkManager = FindObjectOfType<NetworkManager>();
                    networkManager.MenuCamera.SetActive(true);
                    networkManager.RespawnTimer = 3.0f;
                    networkManager.GetComponent<PhotonView>().RPC("AddChatMessageRpc", PhotonTargets.AllBuffered, "Player " + PhotonNetwork.player.name + " has been destroyed!");
                }

                Debug.Log("BeDestroyed() called from " + _photonView.instantiationId);
                PhotonNetwork.Destroy(gameObject);
            }
        }
	}
}
