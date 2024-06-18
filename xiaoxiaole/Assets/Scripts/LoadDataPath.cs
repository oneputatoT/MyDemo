using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Paths",fileName ="DataPaths")]
public class LoadDataPath : ScriptableObject
{
    [Header("棋盘预制体文件名")]
    public string squarePrefabPath;

    [Header("糖果预制体文件名")]
    public string candyPrefabPath;

    [Header("棋盘背景文件名")]
    public string[] squareTexPath;

    [Header("糖果类型文件名")]
    public string[] candyTexPath;

    public string candyAllInPath;


    [Header("爆炸粒子文件名")]
    public string[] explosionPrefabPath;
}
