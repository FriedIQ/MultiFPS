using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class FXManager : MonoBehaviour 
{

    public PhotonView PhotonView;
    public GameObject SniperShotPrefab;
    public float WeaponEffectOffsetX = 0f;
    public float WeaponEffectOffsetY = 0f;
    public float WeaponEffectOffsetZ = 0f;

	// Use this for initialization
    void Start()
    {
        PhotonView = GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
    //void Update () {
	
    //}

    [RPC]
    void SniperBulletEffect(Vector3 startPos, Vector3 endPos)
    {
        Vector3 offset = new Vector3(WeaponEffectOffsetX, WeaponEffectOffsetY, WeaponEffectOffsetZ);
        GameObject effect = (GameObject)Instantiate(SniperShotPrefab, (startPos + offset), Camera.main.transform.rotation);
        LineRenderer lineRenderer = effect.transform.Find("BulletEffect").GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, (startPos + offset));
        lineRenderer.SetPosition(1, endPos);
    }
}
