using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerShoot : MonoBehaviour {

	public float fireRate = 0.5f;
	float coolDown = 0.0f;
	float range = 550.0f;
	float damage = 25.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		coolDown -= Time.deltaTime;

		if( Input.GetButton( "Fire1" ) )
		{
			Fire();
		}
	}

	void Fire()
	{
		if(coolDown > 0)
		{
			return;
		}

		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

		Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20, Color.red);

		RaycastHit hitInfo;

		if( GetClosestRaycastHit( ray, out hitInfo ) )
		{
			Debug.Log( "Hit: " + hitInfo.collider.name );
			var transform = hitInfo.transform;

			var target = transform.GetComponent<Destructable>();
			while( target == null && transform.parent)
			{
				transform = transform.parent;
				target = transform.GetComponent<Destructable>();
			}

			if(target != null)
			{
				//target.TakeDamage(damage);
				target.GetComponent<PhotonView>().RPC( "TakeDamage", PhotonTargets.All, damage );
			}
		}
		else
		{
			Debug.Log( "Hit Nothing" );
		}


		coolDown = fireRate;
	}

	bool GetClosestRaycastHit( Ray ray, out RaycastHit hitInfo )
	{
		RaycastHit[] hits = Physics.RaycastAll( ray, range );
		if( hits.Count() == 0 )
		{
			hitInfo = new RaycastHit();
			return false;
		}
		else
		{
			hitInfo = hits.Where(h => h.collider.name != "Local Player").OrderBy(h => h.distance).First();
			return true;
		}
	}
}
