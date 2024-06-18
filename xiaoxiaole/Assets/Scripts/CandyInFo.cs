using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyInFo : MonoBehaviour
{
    public Vector2Int index;
    public int color;
    public CandyType type;

    public CandyInFo(Vector2Int index,int color,CandyType type)
    {
        this.index = index;
        this.color = color;
        this.type = type;
    }
}
