using System.ComponentModel;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PhotonView))]
// ReSharper disable once CheckNamespace
public class PlayerShoot : MonoBehaviour {

	private float _coolDown;

    FXManager _fxManager;
    PhotonView _photonView;
    WeaponData _weaponData;
    private PlayerTeam _playerTeam;

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        if (_photonView == null)
        {
            Debug.Log("Unable to find PhotonView");
        }

        _fxManager = FindObjectOfType<FXManager>();
        if (_fxManager == null)
        {
            Debug.Log( "Unable to find FXManager" );
        }

        _playerTeam = GetComponent<PlayerTeam>();
        if (_playerTeam == null)
        {
            Debug.Log("Unable to find PlayerTeam");
        }
    }

    // Use this for initialization
    // ReSharper disable once UnusedMember.Local
	void Start () {
        _weaponData = gameObject.GetComponentInChildren<WeaponData>();
        if (_weaponData == null)
        {
            Debug.Log("Unable to find WeaponData");
        }
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

		var hitInfo = new RaycastHit();

		if( GetClosestRaycastHit( ray, out hitInfo ) )
		{
			//Debug.Log( "Hit: " + hitInfo.collider.name );
			var trans = hitInfo.transform;

			var target = trans.GetComponent<Destructable>();
			while( target == null && trans.parent)
			{
				trans = trans.parent;
				target = trans.GetComponent<Destructable>();
			}

			if(target != null)
			{
                var team = trans.GetComponent<PlayerTeam>();
                while (team == null && trans.parent)
                {
                    trans = trans.parent;
                    team = trans.GetComponent<PlayerTeam>();
                }

			    if (team != null && (team.TeamId == 0 || team.TeamId != _playerTeam.TeamId))
			    {
                    Debug.Log("Shot team " + team.TeamId + " object. You are team " + _playerTeam.TeamId);
                    target.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, _weaponData.Damage);
			    }
			    else
			    {
                    Debug.Log("PlayerTeam was NULL");
                    target.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, _weaponData.Damage);
			    }
                
			}

            DoWeaponEffect("SniperBulletEffect", _weaponData.Muzzle.transform.position, hitInfo.point);
		}
		else
		{
			//Debug.Log( "Hit Nothing" );

            var endPoint = Camera.main.transform.position + (Camera.main.transform.forward * _weaponData.Range);
            DoWeaponEffect("SniperBulletEffect", _weaponData.Muzzle.transform.position, endPoint);
		}

        _coolDown = _weaponData.FireRate;
	}

	bool GetClosestRaycastHit( Ray ray, out RaycastHit hitInfo )
	{
        var hits = Physics.RaycastAll(ray, _weaponData.Range);
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
        _fxManager.PhotonView.RPC(effect, PhotonTargets.All, startPos, endPos);
    }
}
