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
            if (photonView != null && photonView.instantiationId != 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
}
