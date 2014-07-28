using UnityEngine;
using UnityEditor;

public class GameObjectEnhancements : MonoBehaviour
{
    //[MenuItem("GameObject/Create Empty Duplicate #&d")]
    //static void createEmptyDuplicate()
    //{
    //    GameObject go = new GameObject("GameObject");

    //    if (Selection.activeTransform != null)
    //    {
    //        go.transform.parent = Selection.activeTransform.parent;
    //        Vector3 parentPosition = new Vector3(Selection.activeTransform.position.x, Selection.activeTransform.position.y, Selection.activeTransform.position.z);
    //        go.transform.position = parentPosition;
    //    }
    //}

    [MenuItem("GameObject/Create Empty Child #&c")]
    static void createEmptyChild()
    {
        GameObject go = new GameObject("GameObject");

        if (Selection.activeTransform != null)
        {
            go.transform.parent = Selection.activeTransform;
            Vector3 parentPosition = new Vector3(Selection.activeTransform.position.x, Selection.activeTransform.position.y, Selection.activeTransform.position.z);
            go.transform.position = parentPosition;
        }
    }

}
