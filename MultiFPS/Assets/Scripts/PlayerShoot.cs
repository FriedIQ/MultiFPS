﻿using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PhotonView))]
// ReSharper disable once CheckNamespace
public class PlayerShoot : MonoBehaviour {

	public float FireRate = 0.5f;

	private float CoolDown;
    private const float Range = 550.0f;
    private const float Damage = 25.0f;

    FXManager fxManager;
    PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.Log("Unable to find PhotonView");
        }

        fxManager = GameObject.FindObjectOfType<FXManager>();
        if (fxManager == null)
        {
            Debug.Log( "Unable to find FXManager" );
        }
    }

    // Use this for initialization
    // ReSharper disable once UnusedMember.Local
	void Start () {
	    
	}
	
	// Update is called once per frame
    // ReSharper disable once UnusedMember.Local
	void Update () {
		CoolDown -= Time.deltaTime;

		if( Input.GetButton( "Fire1" ) )
		{
			Fire();
		}
	}

	void Fire()
	{
		if(CoolDown > 0)
		{
			return;
		}

		var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

		Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20, Color.red);

		RaycastHit hitInfo = new RaycastHit();

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

            DoWeaponEffect("SniperBulletEffect", Camera.main.transform.position, hitInfo.point);
		}
		else
		{
			Debug.Log( "Hit Nothing" );

            Vector3 endPoint = Camera.main.transform.position + (Camera.main.transform.forward * Range);
            DoWeaponEffect("SniperBulletEffect", Camera.main.transform.position, endPoint);
		}

		CoolDown = FireRate;
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

    void DoWeaponEffect(string effect, Vector3 startPos, Vector3 endPos)
    {
        fxManager.PhotonView.RPC(effect, PhotonTargets.All, startPos, endPos);
    }
}
