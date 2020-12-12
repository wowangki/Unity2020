using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum NODE_DIRECTION
    {
        NONE,
        UP,
        LEFT,
        RIGHT,
        DOWN
    }

    public GameObject Owner;
    public Vector3 pos;
    public Vector2Int Id { get; set; }
    public bool IsClose { get; set; } = false;
    public NODE_DIRECTION Direction { get; set; } = NODE_DIRECTION.NONE;
    public float DistToStart { get; set; } = 0f;
    public float DistToEnd { get; set; } = 0f;
    public float TotalDist { get { return DistToStart + DistToEnd; } }
    public Node Parent { get; set; } = null;

    public void ResetNode()
    {
        Direction = NODE_DIRECTION.NONE;
        DistToStart = 0f;
        DistToEnd = 0f;
        Parent = null;
    }
}
