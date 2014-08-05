using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

    public float SelfDestructTime = 1.0f;

	// Use this for initialization
    //void Start () {
	
    //}
	
	// Update is called once per frame
	void Update () {
        SelfDestructTime -= Time.deltaTime;

        if (SelfDestructTime <= 0)
        {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView == null)
            {
                Destroy(gameObject);
                return;
            }

            if (photonView.instantiationId != 0 && photonView.isMine)
            {
                Debug.Log("PhotonNetwork.Destroy called from " + photonView.instantiationId);
                PhotonNetwork.Destroy(gameObject);
            }
        }
	}
}
