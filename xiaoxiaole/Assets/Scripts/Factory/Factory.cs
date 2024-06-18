using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory
{
    string spritePath = "Textures/Blocks/";  //背景贴图路径
    string prefabPath = "Prefabs/";          //预制体路径
    string candySpritesPath = "Textures/Items/";//普通贴图文件路径前缀

    Dictionary<string,Sprite> spriteDic = new Dictionary<string,Sprite>();

    Dictionary<string,GameObject> prefabDic = new Dictionary<string,GameObject>();

    //糖果贴图
    List<Sprite[]> candiesSprite;
    //爆炸预制体
    List<GameObject> explosionPrefabs;

    GameManager manager;

    public Factory(GameManager gameManager)
    {
        manager = gameManager;
    }


    public GameObject LoadObjectFromResources(string str)
    {
        string path = prefabPath + str;


        if (prefabDic.ContainsKey(path))
        {
            return prefabDic[path];
        }

        GameObject obj = Resources.Load<GameObject>(path);
        if (obj == null)
        {
            Debug.Log("无法获取物体，或为不存在物体");
            return null;
        }

        prefabDic[path] = obj;

        return prefabDic[path];
    }

    public Sprite LoadSprite(string str)
    { 
        string path = spritePath + str;
       
        if (!spriteDic.ContainsKey(path))
        {
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            spriteDic[path] = sprite;
        }
        
        return spriteDic[path];
    }

    public Sprite LoadCandySprite(string str)
    {
        string path = candySpritesPath + str;

        if (!spriteDic.ContainsKey(path))
        {
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            spriteDic[path] = sprite;
        }

        return spriteDic[path];
    }

    //CandySprite相关
    public void SetCandiesSprite(int candyColors,int candyVariety,LoadDataPath dataPath)
    {
        candiesSprite = new List<Sprite[]>();

        //每个List里面包含一种颜色的各种类型贴图
        for (int i = 0; i < candyColors; i++)
        {
            Sprite[] temp = new Sprite[candyVariety];
            for (int j = 0; j < candyVariety; j++)
            {
                temp[j] = LoadCandySprite(dataPath.candyTexPath[i * candyVariety + j]);
            }
            candiesSprite.Add(temp);
        }

    }

    public Sprite GetCandiesSpriteInList(int color,CandyType candyType)
    {
        return candiesSprite[color][(int)candyType];
    }

    public Vector2Int GetCandiesSpriteRAndC()
    {
        return new Vector2Int(candiesSprite.Count, candiesSprite[0].Length);
    }




    //爆炸特效相关

    public void SetExplosionPrefabs(LoadDataPath dataPath)
    {
        explosionPrefabs = new List<GameObject>();
        for (int i = 0; i < dataPath.explosionPrefabPath.Length; i++)
        {
            explosionPrefabs.Add(LoadObjectFromResources(dataPath.explosionPrefabPath[i]));
        }
    }

    public GameObject GetExplosionPrefabs(int color)
    {
        if (color >= explosionPrefabs.Count)
        {
            Debug.Log($"不存在{color}");
            return null;
        }

        return explosionPrefabs[color];
    }
}
