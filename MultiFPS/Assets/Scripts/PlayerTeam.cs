using UnityEngine;

public class PlayerTeam : MonoBehaviour {
    public int TeamId { get; private set; }

    [RPC]
    private void SetTeam(int teamId)
    {
        TeamId = teamId;

        var skin = transform.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skin == null)
        {
            Debug.Log("SkinnedMeshRenderer not found");
            return;
        }

        switch (teamId)
        {
            case 1:
                skin.material.color = Color.blue;
                break;
            case 2:
                skin.material.color = Color.red;
                break;
            default:
                skin.material.color = Color.white;
                break;
        }
    }
}
