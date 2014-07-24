using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		meshRenderer.enabled = false;
	}
}
