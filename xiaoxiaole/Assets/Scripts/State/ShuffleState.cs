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
        Debug.Log("����ϴ��ģʽ��");

        if (limit >= 99)
        {
            Debug.Log("��Ч����");
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

                //�Ƚ���λ��
                gameManager.SwapTwoCandyPositong(i, j, nextR, nextC);

                //�ٰ������ϵ�����и���
                gameManager.SwapTwoCandy(i, j, nextR, nextC);
            }
        }

        machine.SwitchState(GameState.Match);
    }
}
