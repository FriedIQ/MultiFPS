using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class Destructable : MonoBehaviour {

	public float structurePoints = 100f;

    PhotonView photonView;
    float currentStructurePoints;

	// Use this for initialization
	void Start () {
        photonView = GetComponent<PhotonView>();
		currentStructurePoints = structurePoints;
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
		currentStructurePoints -= damage;
		if( currentStructurePoints <= 0 )
		{
			BeDestroyed();
		}
	}

	void BeDestroyed()
	{
        if (photonView.instantiationId == 0)
        {
            //Debug.Log("BeDestroyed() called from " + photonView.instantiationId);
            Destroy(gameObject);
        }
        else
        {
            if (photonView.isMine)
            {
                //Debug.Log("Called from Master: [" + photonView.instantiationId + "]");
                if (gameObject.tag == "Player")
                {
                    var networkManager = GameObject.FindObjectOfType<NetworkManager>();
                    networkManager.MenuCamera.SetActive(true);
                    networkManager.RespawnTimer = 3.0f;
                    networkManager.GetComponent<PhotonView>().RPC("AddChatMessageRpc", PhotonTargets.AllBuffered, "Player " + PhotonNetwork.player.name + " has been destroyed!");
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }
	}
}
