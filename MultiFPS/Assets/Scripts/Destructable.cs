using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	public float structurePoints = 100f;
	float currentStructurePoints;

	// Use this for initialization
	void Start () {
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
		Destroy(gameObject);
	}
}
