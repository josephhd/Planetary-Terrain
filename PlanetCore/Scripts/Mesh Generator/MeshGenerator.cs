using System.Collections.Generic;
using UnityEngine;
using NoiseVecGenerator;
using VectorDoubles;
using System;

public class MeshGenerator {
    public MeshGenerator (PlanetSettings PlanetSettings, NoiseSettings NoiseSettings, Node CallerNode){
        planetSettings = PlanetSettings;
        callerNode = CallerNode;

        #region Define Important Values

        verts = planetSettings.resolution;
        radius = planetSettings.radius;

        vertexPosScale = 2 * callerNode.scale / (verts - 1);
        tileCenter = (verts - 1) / 2f;
        uvOffset = 2.0 * new Vector3Double(callerNode.uv.x, 0, callerNode.uv.y);

        vertices = new List<Vector3>(verts + 1 * verts + 1);
        normals = new List<Vector3>(verts + 1 * verts + 1);
        uvs = new List<Vector2>(verts + 1 * verts + 1);
        triangles = new List<int>(6 * (verts + 1 - 1) * (verts + 1 - 1));

        noiseGenerator = new NoiseVector(NoiseSettings);

        #endregion
    }
    private bool complete;
    public bool Complete {
        get { return complete; }
    }

    private PlanetSettings planetSettings;
    private Node callerNode;

    private int verts;
    private double radius;

    private double vertexPosScale;          //coefficient that scales vertices to be the correct scale
    private double tileCenter;              //the center (0, 0 at bottom left) of the tile in vertices
    private Vector3Double uvOffset;         //the offset of each chunk on a normalized plane

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> triangles;

    private NoiseVector noiseGenerator;

    public MeshData meshData;

    public void GenerateMesh(object stateInfo) {
        
        //Calculate tile center (for gameobject "node" position)
        Vector3Double intialPos = callerNode.rotation * SphereizedPos(tileCenter, tileCenter, callerNode.scale);
        Vector3Double noiseVector = intialPos * noiseGenerator.GetNoiseValue(intialPos);
        Vector3Double center = radius * intialPos + noiseVector;

        GenerateVertices(center);
        GenerateNormals();
        GenerateTriangles();
        GenerateUVs();
        GenerateEdgeSkirt();

        meshData = new MeshData(vertices, triangles, uvs, normals, center + planetSettings.center, planetSettings.center);
        complete = true;
    }

    //Generate the vertices, including temporaries for normal calcs
    private void GenerateVertices(Vector3Double center) {
        int vertexIndex = 0;

        Vector3Double s = new Vector3Double();
        Vector3Double noiseVec = new Vector3Double();
        Vector3 vertexPosition = new Vector3();

        for (int y = 0; y < verts + 1; y++) {
            for (int x = 0; x < verts + 1; x++) { 
                //Set sphereized position, and subtract center position (brings coordinates back into range of floats)
                //Calculate sphereized position
                s = callerNode.rotation * SphereizedPos(x, y, callerNode.scale);
                noiseVec = s * noiseGenerator.GetNoiseValue(s);

                s = radius * s;
                s += noiseVec;
                s = s - center;

                vertexPosition.Set((float)s.x, (float)s.y, (float)s.z);

                vertices.Add(vertexPosition);

                vertexIndex++;
            }
        }
    }

    //Generate the normals, and remove the temporary vertices from the list
    private void GenerateNormals () {
        int vertexIndex = 0;
        List<Vector3> rm = new List<Vector3>();

        for (int y = 0; y < verts + 1; y++) {
            for (int x = 0; x < verts + 1; x++) {
                if (y < verts && x < verts) {
                    Vector3 A = vertices[vertexIndex];
                    Vector3 B = vertices[vertexIndex + 1];
                    Vector3 C = vertices[vertexIndex + verts + 1];

                    Vector3 AB = (A - B).normalized;
                    Vector3 AC = (A - C).normalized;

                    normals.Add(Vector3.Cross(AC, AB));
                } else {
                    rm.Add(vertices[vertexIndex]);
                }

                vertexIndex++;
            }
        }

        for (int i = 0; i < rm.Count; i++) {
            vertices.Remove(rm[i]);
        }

        rm = null;
    }

    //Generate the core triangles
    private void GenerateTriangles () {
        int vertexIndex = 0;

        for (int y = 0; y < verts; y++) {
            for (int x = 0; x < verts; x++) {
                if (x < verts - 1 && y < verts - 1) {
                    AddTriangle(vertexIndex, vertexIndex + verts, vertexIndex + verts + 1);
                    AddTriangle(vertexIndex, vertexIndex + verts + 1, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
    }

    //Triangle creation helper
    private void AddTriangle(int a, int b, int c) {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
    }

    //Generate the core mesh uvs
    private void GenerateUVs() {
        for (int y = 0; y < verts; y++) {
            for (int x = 0; x < verts; x++) {
                double realDim = callerNode.scale * radius;
                uvs.Add(new Vector2((float)x / (verts - 1), (float)y / (verts - 1)) * (float)realDim);
            }
        }
    }

    //Generate the edge skirt, and calculate its normals
    private void GenerateEdgeSkirt() {
        Vector3 vert = new Vector3();
        Vector2 uv = new Vector2();

        int startPos = vertices.Count;
        int vertexIndex = vertices.Count;
        int edgeLength = 4 * (verts - 1);

        //Generate the vertices and triangles
        for (int y = 0; y < 2; y++) {
            //south edge
            for (int x = 0; x < verts - 1; x++) {
                vert = vertices[x] - normals[x] * y * (float)radius * (float)callerNode.scale * 0.1f;
                vertices.Add(vert);

                uv = new Vector2(x, y);
                uvs.Add(uv);

                normals.Add(normals[x]);

                if (y == 0) {
                    AddTriangle(vertexIndex, vertexIndex + edgeLength + 1, vertexIndex + edgeLength);
                    AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + edgeLength + 1);
                }

                vertexIndex++;
            }

            //east edge
            for (int x = verts - 1; x < verts * verts - 1; x += verts) {
                vert = vertices[x] - normals[x] * y * (float)radius * (float)callerNode.scale * 0.1f;
                vertices.Add(vert);

                uv = new Vector2((float)x % (verts - 1), y);
                uvs.Add(uv);

                normals.Add(normals[x]);

                if (y == 0) {
                    AddTriangle(vertexIndex, vertexIndex + edgeLength + 1, vertexIndex + edgeLength);
                    AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + edgeLength + 1);
                }

                vertexIndex++;
            }

            //north edge
            for (int x = verts * verts - 1; x > verts * (verts - 1); x--) {
                vert = vertices[x] - normals[x] * y * (float)radius * (float)callerNode.scale * 0.1f;
                vertices.Add(vert);

                uv = new Vector2((x - verts * (verts - 1)), y);
                uvs.Add(uv);

                normals.Add(normals[x]);

                if (y == 0) {
                    AddTriangle(vertexIndex, vertexIndex + edgeLength + 1, vertexIndex + edgeLength);
                    AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + edgeLength + 1);
                }

                vertexIndex++;
            }

            //west edge
            for (int x = verts * (verts - 1); x > 0; x -= verts) {
                vert = vertices[x] - normals[x] * y * (float)radius * (float)callerNode.scale * 0.1f;
                vertices.Add(vert);

                uv = new Vector2(((float)x % (verts - 1)), y);
                uvs.Add(uv);

                normals.Add(normals[x]);

                if (y == 0 && x > verts) {
                    AddTriangle(vertexIndex, vertexIndex + edgeLength + 1, vertexIndex + edgeLength);
                    AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + edgeLength + 1);
                } else if (y == 0 && x <= verts) {
                    AddTriangle(vertexIndex, startPos + edgeLength, vertexIndex + edgeLength);
                    AddTriangle(vertexIndex, startPos, startPos + edgeLength);
                }

                vertexIndex++;
            }
        }
    }   

    //Calculate the sphereized position of the chunk
    private Vector3Double SphereizedPos (double x, double y, double scale) {
        Vector3Double planePos = new Vector3Double(x * vertexPosScale - scale, 1, y * vertexPosScale - scale) + uvOffset;

        double x2 = planePos.x * planePos.x;
        double y2 = planePos.y * planePos.y;
        double z2 = planePos.z * planePos.z;

        return new Vector3Double
        (
         planePos.x * Math.Sqrt(1 - y2 / 2.0 - z2 / 2.0 + y2 * z2 / 3.0),
         planePos.y * Math.Sqrt(1 - x2 / 2.0 - z2 / 2.0 + x2 * z2 / 3.0),
         planePos.z * Math.Sqrt(1 - x2 / 2.0 - y2 / 2.0 + x2 * y2 / 3.0)
        );
    }
}

public class MeshData {
    public readonly List<Vector3> vertices;
    public readonly List<int> triangles;
    public readonly List<Vector3> normals;
    public readonly List<Vector2> uvs;
    public readonly Vector3Double centerPoint;
    public readonly Vector3 planetCenter;

    public MeshData (List<Vector3> Vertices, List<int> Triangles, List<Vector2> UVs, List<Vector3> Normals, Vector3Double Center, Vector3 PlanetCenter) {
        vertices = Vertices;
        triangles = Triangles;
        uvs = UVs;
        normals = Normals;
        centerPoint = Center;
        planetCenter = PlanetCenter;
    }
};