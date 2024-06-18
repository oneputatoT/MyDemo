using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class PlayState : GameSystemState
{

    Vector3 startPos;
    bool IsDrag;

    int curR;
    int curC;

    public PlayState(GameManager gameManager, IMachine machine) : base(gameManager, machine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        startPos = Vector2.zero;
        limit = 0;
    }

    public override void Update()
    {
        base.Update();
        //当按键按下时
        if (gameManager.buttonDown)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform != null && hit.transform.TryGetComponent(out Square temp))
            {
                curR = temp.index.x;
                curC = temp.index.y;
                //如果这个位置是糖果的话
                if (gameManager.IsCandy(curR, curC))
                {
                    startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    IsDrag = true;
                }
                
            }
        }
        else if(gameManager.buttonUp)
        {
            startPos = Vector3.zero;
            IsDrag = false;
        }

        //当按键按下,且不松手时
        if (IsDrag)
        {
            Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - startPos;

            int swapIndexR=-1;
            int swapIndexC=-1;

            //有做出位移
            if (Vector3.Magnitude(dir) >= 0.1f)
            {
                //方向向右,且向右幅度大于向上下的
                if (dir.x > 0 && Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    swapIndexR = curR;
                    swapIndexC = curC + 1;
                }
                //方向向左,且向左幅度大于向上下的
                else if (dir.x < 0 && Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    swapIndexR = curR;
                    swapIndexC = curC - 1;
                }
                //方向向上,且向上幅度大于向左右的
                else if (dir.y> 0 && Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
                {
                    swapIndexR = curR-1;
                    swapIndexC = curC;
                }
                //方向向下,且向下幅度大于向左右的
                else if (dir.y < 0 && Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
                {
                    swapIndexR = curR +1;
                    swapIndexC = curC;
                }
                IsDrag = false;
            }

            if (swapIndexR != -1 && swapIndexC != -1 && gameManager.IsCandy(swapIndexR, swapIndexC))
            {
                //记录当前的位置信息
                Vector3 curVec = gameManager.GetCandyInBoard(curR, curC).transform.position;
                Vector3 swapVec = gameManager.GetCandyInBoard(swapIndexR, swapIndexC).transform.position;
                
                //交换两个物体的颜色和CandyControl组件
                
                gameManager.SwapTwoCandy(curR, curC, swapIndexR, swapIndexC);

                Sequence s1 = DOTween.Sequence();
                Sequence s2 = DOTween.Sequence();


                //判断是否存在特殊方块

                //此时CurR，CurX就是交换后的物体的行列了
                //利用||的特性，如果前面true，则不看后面的方法
                if (CheckSpecCandyType(curR,curC,swapIndexR,swapIndexC)||gameManager.Match())
                {
                    s1.Append(gameManager.GetCandyInBoard(curR, curC).transform.DOMove(curVec, 0.5f));
                    s2.Append(gameManager.GetCandyInBoard(swapIndexR, swapIndexC).transform.DOMove(swapVec, 0.5f));

                    gameManager.AddAnimationCount();
                    s1.OnComplete(() => { gameManager.SubAnimation(); });

                    gameManager.AddAnimationCount();
                    s2.OnComplete(() => { gameManager.SubAnimation(); });

                    machine.SwitchState(GameState.Anima);
                    machine.SetNextState(GameState.Clear);

                }
                else
                {
                    //先移动物体,做一个视觉效果
                    s1.Append(gameManager.GetCandyInBoard(curR, curC).transform.DOMove(curVec, 0.5f));
                    s1.Append(gameManager.GetCandyInBoard(curR, curC).transform.DOMove(swapVec, 0.5f));

                    
                    s2.Append(gameManager.GetCandyInBoard(swapIndexR, swapIndexC).transform.DOMove(swapVec, 0.5f));
                    s2.Append(gameManager.GetCandyInBoard(swapIndexR, swapIndexC).transform.DOMove(curVec, 0.5f));

                    gameManager.AddAnimationCount();
                    s1.OnComplete(() => { gameManager.SubAnimation(); });

                    gameManager.AddAnimationCount();
                    s2.OnComplete(() => { gameManager.SubAnimation(); });
                    
                    //再进行交换
                    gameManager.SwapTwoCandy(swapIndexR, swapIndexC, curR, curC);

                    machine.SwitchState(GameState.Anima);
                    machine.SetNextState(GameState.Play);
                }

            }
        }
    }


    bool CheckSpecCandyType(int curR,int curC,int swapR,int swapC)
    {
        bool haveSpecType = false;
        if (gameManager.GetCandyType(curR, curC) == CandyType.AllIn)
        {
            gameManager.AddToTempDelectList(new Vector2Int(curR, curC));
            if (gameManager.GetCandyType(swapR, swapC) != CandyType.AllIn)
            {
                int theColor = gameManager.GetCandyColorInBoard(swapR, swapC);
                for (int i = 0; i < gameManager.row; i++)
                {
                    for (int j = 0; j < gameManager.col; j++)
                    {
                        if (gameManager.GetCandyColorInBoard(i, j) == theColor)
                        {
                            gameManager.AddToTempDelectList(new Vector2Int(i, j));
                        }
                    }
                }
            }
            else
            {
                AddAllCandy();
            }
            haveSpecType = true;
        }

        if (gameManager.GetCandyType(swapR, swapC) == CandyType.AllIn)
        {
            gameManager.AddToTempDelectList(new Vector2Int(swapR, swapC));
            if (gameManager.GetCandyType(curR, curC) != CandyType.AllIn)
            {
                int theColor = gameManager.GetCandyColorInBoard(curR, curC);
                for (int i = 0; i < gameManager.row; i++)
                {
                    for (int j = 0; j < gameManager.col; j++)
                    {
                        if (gameManager.GetCandyColorInBoard(i, j) == theColor)
                        {
                            gameManager.AddToTempDelectList(new Vector2Int(i, j));
                        }
                    }
                }
            }
            else
            {
                AddAllCandy();
            }
            haveSpecType = true;
        }

        return haveSpecType;
    }

    void AddAllCandy()
    {
        for (int i = 0; i < gameManager.row; i++)
        {
            for (int j = 0; j < gameManager.col; j++)
            {
                gameManager.AddToTempDelectList(new Vector2Int(i, j));
            }
        }
    }

}
