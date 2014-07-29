using UnityEngine;
using System.Collections;

public class WeaponData : MonoBehaviour {

    public Transform Muzzle;
    public float FireRate = 0.5f;
    public float Range = 550.0f;
    public float Damage = 25.0f;

	// Use this for initialization
	void Start () {
        Muzzle = transform.Find("Muzzle");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
