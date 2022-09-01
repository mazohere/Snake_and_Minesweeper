using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    // purely establishing public variables for all possible types and positions of cells. They are all public because they are all called outside of this script.
    public enum Type
    {
        Invalid,
        Empty,
        Mine,
        Number,
    }

    public Vector3Int position;
    public Type type;
    public int number;
    public bool revealed;
    public bool flagged;
    public bool exploded;
}
