using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CandyControl : MonoBehaviour
{
    public Vector2Int index;

    CandyAttribute attr;

    public void Initialization(int i,int j,CandyAttribute canAttr)
    {
        SetIndex(i, j);
        SetAttr(canAttr);
    }

    public void SetIndex(int i, int j)
    {
        index.x = i;
        index.y = j;
    }

    public void SetAttr(CandyAttribute canAttr)
    {
        attr = canAttr;
    }

    public int GetColor()
    {
        return attr.GetColor();
    }

    public CandyType GetCandyType()
    {
        return attr.GetCandyType();   
    }

    public void SetCandySprite()
    {
        GetComponent<SpriteRenderer>().sprite = attr.GetSprite();
    }

    public void Delete()
    { 
        Destroy(gameObject);
    }
}
