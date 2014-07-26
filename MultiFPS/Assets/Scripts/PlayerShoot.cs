using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PhotonView))]
// ReSharper disable once CheckNamespace
public class PlayerShoot : MonoBehaviour {

	public float FireRate = 0.5f;

	float _coolDown;
    private const float Range = 550.0f;
    private const float Damage = 25.0f;

    // Use this for initialization
    // ReSharper disable once UnusedMember.Local
	void Start () {
	
	}
	
	// Update is called once per frame
    // ReSharper disable once UnusedMember.Local
	void Update () {
		_coolDown -= Time.deltaTime;

		if( Input.GetButton( "Fire1" ) )
		{
			Fire();
		}
	}

	void Fire()
	{
		if(_coolDown > 0)
		{
			return;
		}

		var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

		Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20, Color.red);

		RaycastHit hitInfo;

		if( GetClosestRaycastHit( ray, out hitInfo ) )
		{
			Debug.Log( "Hit: " + hitInfo.collider.name );
			var trans = hitInfo.transform;

			var target = trans.GetComponent<Destructable>();
			while( target == null && trans.parent)
			{
				trans = trans.parent;
				target = trans.GetComponent<Destructable>();
			}

			if(target != null)
			{
				//target.TakeDamage(damage);
				target.GetComponent<PhotonView>().RPC( "TakeDamage", PhotonTargets.AllBuffered, Damage );
			}
		}
		else
		{
			Debug.Log( "Hit Nothing" );
		}


		_coolDown = FireRate;
	}

	bool GetClosestRaycastHit( Ray ray, out RaycastHit hitInfo )
	{
	    var hits = Physics.RaycastAll( ray, Range );
	    if( hits.Count() == 0 )
		{
			hitInfo = new RaycastHit();
			return false;
		}
	    
        hitInfo = hits.Where(h => h.collider.name != "Local Player").OrderBy(h => h.distance).First();
	    return true;
	}
}
