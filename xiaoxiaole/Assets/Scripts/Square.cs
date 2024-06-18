using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Square : MonoBehaviour
{
    public Vector2Int index;


    //第几行第几列，x行，j列
    public void Initialization(int i, int j)
    {
        index.x = i;
        index.y = j;
    }
}
