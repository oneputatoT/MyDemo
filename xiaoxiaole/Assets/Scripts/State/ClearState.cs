using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearState : GameSystemState
{
    public ClearState(GameManager gameManager, IMachine machine) : base(gameManager, machine)
    {
    }

    public override void Update()
    {
        base.Update();

       while(gameManager.nextDelectList.Count > 0)
        {
            foreach (Vector2Int item in gameManager.nextDelectList)
            {
                gameManager.AddToDelectList(item);
            }
            gameManager.ClearTempDelectList();


            int n = gameManager.curDelectList.Count;
            for (int i = 0; i < n; i++)
            {
                Vector2Int temp = gameManager.curDelectList[i];

                //DeleteCandy(temp.x, temp.y);
                if (gameManager.GetCandyInBoard(temp.x, temp.y) == null)
                {
                    continue;
                }

                DeleteClassified(temp.x, temp.y);
                gameManager.AddAnimationCount();  //ÿһ����ը������һ����������
            }
            gameManager.ClearDelectList();
        }

        machine.SwitchState(GameState.Anima);

        //�ж��Ƿ��д���Ҫ���������ǹ�
        if (gameManager.CanMakeSpecialCandy())
        {
            machine.SetNextState(GameState.Creat);
        }
        else
        {
            machine.SetNextState(GameState.Down);
        }

    }


    void DeleteCandy(int i, int j)
    {
        //�ȱ�����ɫ
        int theColor = gameManager.GetCandyColorInBoard(i, j);
        
        //if (theColor == int.MaxValue) return;  //˵��û���������ɾ��


        CandyControl temp = gameManager.GetCandyInBoard(i, j);
        temp.Delete();
        gameManager.SetCandyInBoard(i, j, null);

        gameManager.SetCandyColorInBoard(i, j, int.MaxValue);  //��Ϊ���ֵ�������ǹ�

        gameManager.CreateExplosion(i, j, theColor);
    }

    void DeleteDimond(int i, int j,int diamondRadius)
    {
        int curR = i;
        int curC = j;
        for (int h = 0; h < diamondRadius; h++)
        {
            for (int w = 0; w <= h * 2; w++)
            { 
                int r = curR -diamondRadius+h;
                int c = curC -h+ w;
                if (r >= 0 || r < gameManager.row || c >= 0 || c < gameManager.col)
                {
                    if (!gameManager.curDelectList.Contains(new Vector2Int(r, c)))
                    {
                        gameManager.AddToTempDelectList(new Vector2Int(r, c));
                    }
                }
            }

            for (int w = 0; w <= h * 2; w++)
            {
                int r = curR + diamondRadius - h;
                int c = curC - h + w;
                if (r >= 0 || r < gameManager.row || c >= 0 || c < gameManager.col)
                {
                    if (!gameManager.curDelectList.Contains(new Vector2Int(r, c)))
                    {
                        gameManager.AddToTempDelectList(new Vector2Int(r, c));
                    }
                }
            }
        }

        for (int w = 0; w <= diamondRadius * 2; w++)
        {
            int c = curC - diamondRadius + w;
            if (c >= 0 || c < gameManager.col)
            {
                if (!gameManager.curDelectList.Contains(new Vector2Int(curR, c)))
                {
                    gameManager.AddToTempDelectList(new Vector2Int(curR, c));
                }
            }
        }
    }

    void DeleteHorizontal(int i, int j)
    {

        for (int c = 0; c < gameManager.col; c++)
        {
            if (!gameManager.curDelectList.Contains(new Vector2Int(i, c)))
            {
                gameManager.AddToTempDelectList(new Vector2Int(i, c));
            }
        }
    }

    void DeleteVertical(int i, int j)
    {

        for (int r = 0; r < gameManager.row; r++)
        {
            if (!gameManager.curDelectList.Contains(new Vector2Int(r, j)))
            {
                gameManager.AddToTempDelectList(new Vector2Int(r, j));
            }
        }
    }

    //����ɾ��
    void DeleteClassified(int i,int j)
    {
       
        CandyType type = gameManager.GetCandyType(i, j);
        switch (type)
        {
            case CandyType.Diamond:
            {
                DeleteDimond(i, j, gameManager.diamondRadius);
                break;
            }

            case CandyType.Horizontal:
            {
                DeleteHorizontal(i, j);
                break;
            }

            case CandyType.Vertical:
            {
                DeleteVertical(i, j);
                break;
            }
                
            default: break;
        }

        //��ͨ��Allin����������Ĳ���
        DeleteCandy(i, j);
    }

}



