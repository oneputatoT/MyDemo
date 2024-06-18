using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeFactory 
{
    Factory resourceFactory;
    GameManager gameManager;

    //可能后续需要new一个新对象出来，而不是在这直接引用，不然改变的话，会导致其他发生变换
    //但是本设计中，是采用直接替换的，可能没有这种问题出现
    Dictionary<int,CandyAttribute> candyAttributeList;
    CandyAttribute allInAttribute;

    public AttributeFactory(Factory resourceFactory, GameManager gameManager)
    {
        this.resourceFactory = resourceFactory;
        this.gameManager = gameManager;
        candyAttributeList = new Dictionary<int, CandyAttribute>();
    }

    public void SetCandyAttrubuteList()
    {
        Vector2Int RAndC = resourceFactory.GetCandiesSpriteRAndC();
        int r = RAndC.x;
        int c = RAndC.y;

        for (int i = 0; i < r; i++)
        {
            //int len = resourceFactory.candiesSprite[i].Length;
            for (int j = 0; j < c; j++)
            {
                candyAttributeList.TryAdd(i * c + j,
                    new CandyAttribute(i, (CandyType)j, resourceFactory.GetCandiesSpriteInList(i, (CandyType)j)));
            }
        }
    }

    public void SetCandyAllInAttrubute(LoadDataPath path)
    {
        allInAttribute = new CandyAttribute(gameManager.candyColors, CandyType.AllIn,
            resourceFactory.LoadCandySprite(path.candyAllInPath));
    }

    public CandyAttribute GetAttribute(int color, CandyType candyType)
    {
        if (candyType == CandyType.AllIn)
        {
            return allInAttribute;
        }

        return candyAttributeList[color * gameManager.candyVariety + (int)candyType];
    }

}
