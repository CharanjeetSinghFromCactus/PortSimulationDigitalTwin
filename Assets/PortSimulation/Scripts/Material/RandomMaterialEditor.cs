#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public class RandomMaterialEditor : MonoBehaviour
{
    public Material[] materials;

    [ContextMenu("Apply Random Materials")]
    void ApplyRandom()
    {
        foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rend.sharedMaterial = materials[Random.Range(0, materials.Length)];
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
}
#endif