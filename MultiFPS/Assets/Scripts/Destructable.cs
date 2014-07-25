using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	public float structurePoints = 100f;

    PhotonView photonView;
    float currentStructurePoints;

	// Use this for initialization
	void Start () {
        photonView = GetComponent<PhotonView>();
		currentStructurePoints = structurePoints;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
            Destroy(gameObject);
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("Called from Master: [" + photonView.instantiationId + "]");
                PhotonNetwork.Destroy(gameObject);
            }
        }
	}
}
