using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class Bullet : Photon.MonoBehaviour
{
    private float speed;
    private float range;
    // private float force = 5000;

    private Rigidbody _rigidBody;
    private PhotonView _photonView;
    FXManager _fxManager;


	// Use this for initialization
	void Start () {
        _rigidBody = GetComponent<Rigidbody>();
        if (_rigidBody == null)
        {
            Debug.Log("[Bullet] Unable to find Rigidbody");
        }

        _photonView = GetComponent<PhotonView>();
        if (_photonView == null)
        {
            Debug.Log("[Bullet] Unable to find PhotonView");
        }

        _fxManager = FindObjectOfType<FXManager>();
        if (_fxManager == null)
        {
            Debug.Log("[Bullet] Unable to find FXManager");
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        if (_photonView != null && _photonView.instantiationId != 0 && _photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        DoBulletEffect(contact.point, contact.normal);

        Debug.Log("Hit " + contact.otherCollider.name + "(" + contact.point.x + ", " + contact.point.y + ", " + contact.point.z + ")");
    }

	void OnTriggerEnter(Collider other)
	{
		Debug.Log("Hit " + other.name);
	}

    void DoBulletEffect(Vector3 hit, Vector3 normal)
    {
        _fxManager.PhotonView.RPC("AussaultRifleHitEffect", PhotonTargets.All, hit, normal);
    }
	
	// Update is called once per frame
    //void FixedUpdate () {
    //    gameObject.transform.position += gameObject.transform.forward * speed * Time.deltaTime;
    //    range -= speed * Time.deltaTime;

    //    if (range <= 0f)
    //    {
    //        GameObject.Destroy(gameObject);
    //    }
    //}
}
