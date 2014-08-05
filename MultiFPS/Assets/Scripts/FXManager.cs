using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class FXManager : MonoBehaviour 
{

    public PhotonView PhotonView;
    public GameObject MuzzleEffectPrefab;
    public GameObject BulletHolePrefab;

	// Use this for initialization
    void Start()
    {
        PhotonView = GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
    //void Update () {
	
    //}

    [RPC]
    void AssaultRifleMuzzleEffect(Vector3 startPos, Vector3 endPos)
    {
        if (MuzzleEffectPrefab == null)
        {
            Debug.Log("MuzzleEffectPrefab was NULL");
            return;
        }

        var effect = (GameObject)Instantiate(MuzzleEffectPrefab, (startPos), Camera.main.transform.rotation);
        var lineRenderer = effect.transform.Find("BulletEffect").GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, (startPos));
        lineRenderer.SetPosition(1, endPos);
    }

    [RPC]
    void AussaultRifleHitEffect(Vector3 startPos, Vector3 normal)
    {
        if (BulletHolePrefab == null)
        {
            Debug.Log("BulletHolePrefab was NULL");
            return;
        }

        Instantiate(BulletHolePrefab, (startPos), Quaternion.FromToRotation(Vector3.up, normal));
    }
}
