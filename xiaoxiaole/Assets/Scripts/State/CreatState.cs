using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatState : GameSystemState
{
    public CreatState(GameManager gameManager, IMachine machine) : base(gameManager, machine)
    {
        Debug.Log("¿¥¡À¿œµ‹");
    }

    public override void Update()
    {
        base.Update();

        CreateSpeCandy(gameManager.needAllInList);
        CreateSpeCandy(gameManager.needDiamondList);
        CreateSpeCandy(gameManager.needHorizontalList);
        CreateSpeCandy(gameManager.needVerticalList);

        gameManager.ClearAllSpcCandyList();

        machine.SwitchState(GameState.Down);
    }



    void CreateSpeCandy(List<List<CandyInFo>> temp)
    {
        int n = temp.Count;
        for (int i = 0; i < n; i++)
        {
            int left = 0;
            int right = temp[i].Count;
            int mid = left + (right - left) / 2;
            Vector2Int index = temp[i][mid].index;
            int theColor = temp[i][mid].color;
            CandyType type = temp[i][mid].type;


            gameManager.CreateCandy(index.x, index.y, theColor, type);

        }
    }

}
