using VectorDoubles;

public class Node {
    public MeshGenerator meshGen;

    //Node states
    public NodeState nodeState;
    public bool generating;


    //Node information
    public Chunk obj;

    public byte subLevel;
    public double scale, splitDist;

    public Vector2Double uv;
    public sbyte[,] rotation;
    public Node[] children;
}