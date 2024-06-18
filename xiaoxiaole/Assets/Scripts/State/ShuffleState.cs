using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleState : GameSystemState
{
    

    public ShuffleState(GameManager gameManager, IMachine machine) : base(gameManager, machine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("是我洗牌模式！");

        if (limit >= 99)
        {
            Debug.Log("无效棋盘");
            return;
        }


        limit++;


        for (int i = 0; i < gameManager.row; i++)
        {
            for (int j = 0; j < gameManager.col; j++)
            {
                int rand = Random.Range(i*gameManager.col+j, gameManager.row * gameManager.col);

                int nextR = rand / gameManager.col;
                int nextC = rand % gameManager.col;

                //先交换位置
                gameManager.SwapTwoCandyPositong(i, j, nextR, nextC);

                //再把数组上的类进行更新
                gameManager.SwapTwoCandy(i, j, nextR, nextC);
            }
        }

        machine.SwitchState(GameState.Match);
    }
}
