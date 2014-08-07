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

        // Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20, Color.red);

        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        var endPoint = Camera.main.transform.position + (Camera.main.transform.forward * _weaponData.Range);
        var hitInfo = new RaycastHit();

        if (GetClosestRaycastHit(ray, out hitInfo))
        {
            endPoint = hitInfo.point;

            DoBulletEffect("AussaultRifleHitEffect", hitInfo.point, hitInfo.normal);
        }
        else
        {
            Debug.Log("Hit Nothing");
        }

        DoWeaponEffect("AssaultRifleMuzzleEffect", _weaponData.Muzzle.transform.position, endPoint);


        Debug.Log("(" + hitInfo.point.x + ", " + hitInfo.point.y + ", " + hitInfo.point.z + ")");
        // Debug.Log("Hit: " + hitInfo.collider.transform.name);


        //var direction = (endPoint - _weaponData.Muzzle.transform.position).normalized;
        //var lookRotation = Quaternion.LookRotation(direction);
        //var bullet = PhotonNetwork.Instantiate("Bullet", Camera.main.transform.position + (Camera.main.transform.forward * 2), Camera.main.transform.rotation, 0); //_weaponData.Muzzle.transform.position

        //bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 20000);

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

    void DoBulletEffect(string effect, Vector3 hit, Vector3 normal)
    {
        _fxManager.PhotonView.RPC(effect, PhotonTargets.All, hit, normal);
    }
}
