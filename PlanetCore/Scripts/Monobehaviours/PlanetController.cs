using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Threading;
using VectorDoubles;

public class PlanetController : MonoBehaviour {
    public GameObject chunkPrefab;
    public PlanetSettings planetSettings;
    public NoiseSettings noiseSettings;

    private List<Node> nodes = new List<Node>();

    //Prevent out of bounds values from being used
    private void OnValidate() {
        if (planetSettings.resolution < 3)
            planetSettings.resolution = 3;

        if (planetSettings.radius < 1)
            planetSettings.radius = 1;

        if (planetSettings.minSubLevel > planetSettings.maxSubLevel)
            planetSettings.minSubLevel = planetSettings.maxSubLevel;
    }

    void Start() {
        InitializePlanet();

        StartCoroutine(UpdateChunks());
    }

    //Iterate through nodes and perform actions as necessary
    IEnumerator UpdateChunks () {
        while (true) {
            Vector3 cameraPos = Camera.main.transform.position;

            //Loop through the list of nodes
            for (int i = 0; i < nodes.Count; i++) {
                Node n = nodes[i];

                //Determine if the node should split, merge, or get create a mesh
                if (n.generating) {
                    if (n.meshGen.Complete) {
                        CreateMesh(n);
                    }
                } else if (n.children.Length == 0 && n.subLevel < planetSettings.maxSubLevel) {
                    float distance = (cameraPos - n.obj.position).sqrMagnitude;

                    if (distance <= n.splitDist || n.subLevel < planetSettings.minSubLevel) {
                        SplitNode(n);
                    }
                } else if (n.children.Length > 0) {
                    float distance = (cameraPos - n.obj.position).sqrMagnitude;

                    if (distance > n.splitDist && n.subLevel + 1 > planetSettings.minSubLevel) {
                        MergeNode(n);
                    }
                }

                //Only destroy children/meshes if meshes are currently generated
                switch (n.nodeState) {
                    case NodeState.Splitting:
                        if (!n.children[0].generating && !n.children[1].generating && !n.children[2].generating && !n.children[3].generating)
                            DestroyMeshes(n);
                        break;

                    case NodeState.Merging:
                        if (!n.generating && n.children.Length > 0)
                            DestroyChildren(n);
                        break;

                    default:
                        break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    //Initializes the root nodes for the planet
    private void InitializePlanet () {
        GameObject[] gos = new GameObject[6];

        Node[] Nodes = new Node[6];


        for (int i = 0; i < 6; i++) {
            gos[i] = Instantiate(chunkPrefab, transform.position, Quaternion.identity, transform) as GameObject;

            Nodes[i] = new Node {
                obj = gos[i].GetComponent<Chunk>(),
                nodeState = NodeState.Idle,
                subLevel = 0,
                scale = 1,
                children = new Node[0],
                splitDist = planetSettings.radius * 5f
            };

            nodes.Add(Nodes[i]);
        }

        Nodes[0].rotation = TransformMatrices.None;
        Nodes[1].rotation = TransformMatrices.Down;
        Nodes[2].rotation = TransformMatrices.Right;
        Nodes[3].rotation = TransformMatrices.Left;
        Nodes[4].rotation = TransformMatrices.Forward;
        Nodes[5].rotation = TransformMatrices.Backward;
    }

    //Splits this node into 4 children nodes
    private void SplitNode (Node node) {
        node.nodeState = NodeState.Splitting;

        double offset = node.scale * 0.25;
        byte c_subLevel = (byte)(node.subLevel + 1);
        double c_scale = node.scale * 0.5;

        GameObject[] gos = new GameObject[4];

        Vector2Double[] uvs = new Vector2Double[] {
            new Vector2Double(node.uv.x - offset, node.uv.y + offset),
            new Vector2Double(node.uv.x + offset, node.uv.y + offset),
            new Vector2Double(node.uv.x + offset, node.uv.y - offset),
            new Vector2Double(node.uv.x - offset, node.uv.y - offset)
        };

        //Create and set Nodes
        node.children = new Node[4];

        for (int i = 0; i < 4; i++) {
            gos[i] = Instantiate(chunkPrefab, transform.position, Quaternion.identity, transform) as GameObject;

            node.children[i] = new Node {
                nodeState = NodeState.Idle,
                obj = gos[i].GetComponent<Chunk>(),
                splitDist = 25f * c_scale * c_scale * planetSettings.radius * planetSettings.radius,
                subLevel = c_subLevel,
                scale = c_scale,
                rotation = node.rotation,
                children = new Node[0],
                uv = uvs[i]
            };

            nodes.Add(node.children[i]);

            CreateMeshData(node.children[i]);
        }
    }

    //Merges this node, destroying its children
    private void MergeNode (Node node) {
        node.nodeState = NodeState.Merging;

        CreateMeshData(node);
    }

    //Creates this node's mesh, and assigns it to the chunk GameObject
    private void CreateMesh (Node node) {
        Mesh mesh = new Mesh();
        MeshGenerator meshGen = node.meshGen;

        mesh.SetVertices(meshGen.meshData.vertices);
        mesh.SetNormals(meshGen.meshData.normals);
        mesh.SetTriangles(meshGen.meshData.triangles, 0);
        mesh.SetUVs(0, meshGen.meshData.uvs);

        node.obj.MeshFilter.sharedMesh = mesh;
        node.obj.MeshCollider.sharedMesh = mesh;

        //if there's recently been an origin update, then this check ensures that all position data is correct 
        if (meshGen.meshData.planetCenter == planetSettings.center) {
            node.obj.position = new Vector3((float)meshGen.meshData.centerPoint.x, (float)meshGen.meshData.centerPoint.y, (float)meshGen.meshData.centerPoint.z);
        } else {
            node.obj.position = new Vector3((float)meshGen.meshData.centerPoint.x, (float)meshGen.meshData.centerPoint.y, (float)meshGen.meshData.centerPoint.z) - (meshGen.meshData.planetCenter - planetSettings.center);
        }

        meshGen.meshData = null;
        meshGen = null;
        node.generating = false;
    }

    //Calls for the creation of this node's MeshData
    private void CreateMeshData (Node node) {
        node.generating = true;
        node.meshGen = new MeshGenerator(planetSettings, noiseSettings, node);

        ThreadPool.QueueUserWorkItem(node.meshGen.GenerateMesh);
    }

    //Destroys all meshes connected to the node's chunk
    private void DestroyMeshes (Node node) {
        DestroyImmediate(node.obj.MeshFilter.sharedMesh);
        DestroyImmediate(node.obj.MeshCollider.sharedMesh);

        node.nodeState = NodeState.Idle;
    }

    //Destroys all children connected to this node
    private void DestroyChildren (Node node) {
        //Destory all of this node's children
        for (int i = 0; i < node.children.Length; i++) {
            DestroyMeshes (node.children[i]);

            DestroyChildren(node.children[i]);

            DestroyImmediate(node.children[i].obj.gameObject);
            nodes.Remove(node.children[i]);
        }

        node.children = new Node[0];

        node.nodeState = NodeState.Idle;
    }

    public void UpdatePositions (Vector3 vec) {
        planetSettings.center -= vec;

        for (int i = 0; i < nodes.Count; i++) {
            nodes[i].obj.position -= vec;
        }
    }
}

public enum NodeState {
    Merging,
    Splitting,
    Idle
}

[System.Serializable]
public class PlanetSettings {
    public double radius;
    public byte maxSubLevel;
    public byte minSubLevel;
    public byte resolution;
    public Vector3 center;
};
