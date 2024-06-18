using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PatternCheck 
{
    List<byte[,]> compareGraph;
    List<Vector2Int[]> onePositionIndex;

    #region 初始化
    public PatternCheck()
    {
        compareGraph = new List<byte[,]>();
        onePositionIndex = new List<Vector2Int[]>();

        compareGraph.Add(new byte[,]
        { { 1,1,0},
          { 0,0,1} });

        compareGraph.Add(new byte[,]
        { { 0,0,1},
          { 1,1,0} });

        compareGraph.Add(new byte[,]
        { { 0,1,0},
          { 1,0,1} });

        compareGraph.Add(new byte[,]
        { { 1,1,0,1} });

        AddToPositionList();
    }

    void AddToPositionList()
    {

        foreach (byte[,] graph in compareGraph)
        {
            byte[,] temp = graph;
            onePositionIndex.Add(SetPosition(temp));
            //增加旋转三次的对照表，分别是按左下角旋转90度,180度,270度
            for (int i = 0; i < 3; i++)
            {
                temp = Rotate(temp);
                onePositionIndex.Add(SetPosition(temp));
            }
        }

    }

    Vector2Int[] SetPosition(byte[,] graph)
    {
        int row = graph.GetLength(0);   //行数
        int col = graph.GetLength(1);   //列数

        List<Vector2Int> tempList = new List<Vector2Int>();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (graph[i, j] == 1)
                {
                    tempList.Add(new Vector2Int(i, j));
                }
            }
        }

        return tempList.ToArray();
    }


    byte[,] Rotate(byte[,] graph)
    {
        int row = graph.GetLength(0);   //行数
        int col = graph.GetLength(1);   //列数

        byte[,] newGraph = new byte[col, row];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                //按左下角旋转,第一行的会变成最后一列，而列会变成行
                //所以会先得到每一行的最后一列数，然后再次循环是往前看
                newGraph[j,row-1-i]  = graph[i, j];
            }
        }

        return newGraph;
    }
    #endregion


    #region 检测
    public bool Check(int[,] candyColorInBoard)
    {
        for (int i = 0; i < candyColorInBoard.GetLength(0); i++)
        {
            for (int j = 0; j < candyColorInBoard.GetLength(1); j++)
            {
                //如果有一处符合
                if (CompareOnePosition(i,j,candyColorInBoard))
                {
                    
                    return true;
                }
            }
        }

        return false;
    }

    bool CompareOnePosition(int r, int c, int[,] candyColorInBoard)
    {
        foreach (Vector2Int[] arr in onePositionIndex)
        {
            if (CheckByPosition(r, c, candyColorInBoard, arr))
            {
                return true;
            }
        }

        return false;
    }


    bool CheckByPosition(int curR, int curC, int[,] candyColorInBoard, Vector2Int[] onePosition)
    {

        //找到对照表中的第一个1的位置
        int nextR = curR + onePosition[0].x;
        int nextC = curC + onePosition[0].y;


        int row = candyColorInBoard.GetLength(0);
        int col = candyColorInBoard.GetLength(1);


        //越界了，说明这个对照的框选范围不适用,
        if (nextR >= row || nextR < 0 || nextC >= col || nextC < 0)
        {
            return false;
        }


        int curColor = candyColorInBoard[nextR, nextC];

        for (int i = 1; i < onePosition.Length; i++)
        {
            nextR = curR + onePosition[i].x;
            nextC = curC + onePosition[i].y;

            if (nextR >= row || nextR < 0 || nextC >= col || nextC < 0)
            {
                return false;
            }

            if (candyColorInBoard[nextR, nextC] != curColor)
            {
                return false;
            }

        }

        Debug.Log($"行{curR + onePosition[0].x+1}列{curC + onePosition[0].y+1}可能存在消除对象");
        Debug.Log($"颜色{curColor + 1}");
        return true;

    }
    #endregion
}
