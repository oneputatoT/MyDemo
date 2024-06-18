using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CandyType
{ 
    Ordinary,    //普通
    Diamond,     //菱形
    Vertical,    //纵向
    Horizontal,  //横向
    AllIn        //全部删除型
}

public class CandyAttribute 
{
    int Color;
    CandyType candyType;
    Sprite sprite;

    public CandyAttribute(int color,CandyType candyType, Sprite sprite)
    {
        this.Color = color;
        this.candyType = candyType;
        this.sprite = sprite;
    }

    public int GetColor()
    {
        return Color;
    }

    public CandyType GetCandyType()
    {
        return candyType;
    }

    public Sprite GetSprite()
    { 
        return sprite;
    }
}
