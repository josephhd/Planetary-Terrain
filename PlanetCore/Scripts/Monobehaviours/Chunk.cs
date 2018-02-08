using UnityEngine;

public class Chunk : MonoBehaviour {

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public MeshFilter MeshFilter {
        get {

            if (meshFilter == null) {
                meshFilter = GetComponent<MeshFilter>();
            }

            return meshFilter;
        }
    }
    public MeshCollider MeshCollider {
        get {

            if (meshCollider == null) {
                meshCollider = GetComponent<MeshCollider>();
            }

            return meshCollider;
        }
    }

    public Vector3 position;
}
