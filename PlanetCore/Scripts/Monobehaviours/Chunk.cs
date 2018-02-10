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

    private Vector3 _position;
    public Vector3 position {
        get { return _position; }
        set { _position = value; transform.position = value; }
    }
}
