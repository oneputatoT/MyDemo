using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DownState : GameSystemState
{
   

    public DownState(GameManager gameManager, IMachine machine) : base(gameManager, machine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Down状态启动！");
    }


    public override void Update()
    {
        //如果判断需要下落
        if (Fall())
        {
            //储存下一次是什么状态
            machine.SetNextState(GameState.Down);
            //等待动画时间
            machine.SwitchState(GameState.Anima);
        }
        else
        {
            machine.SwitchState(GameState.Match);
        }
    }

    public bool Fall()
    {
        bool isFall = false;

        for (int j = 0; j < gameManager.col; j++)
        {
            
            if (!gameManager.IsCandy(0,j))
            {
                GameObject go = Object.Instantiate(gameManager.GetCandyGO(),
                           gameManager.GetParent().position + new Vector3(j * gameManager.perWidthDistance, gameManager.row * gameManager.perHeighDistance, 0), 
                           Quaternion.identity);
                //随机颜色
                int tempColor = Random.Range(0, gameManager.candyColors);
                
                //获取组件
                CandyControl tempControl = go.GetComponent<CandyControl>();

                //设置属性
                tempControl.Initialization(0, j, gameManager.GetCandyAttr(tempColor, CandyType.Ordinary));
                tempControl.SetCandySprite();

                //从上向下移动,运动到endVec
                Tweener tweener = go.transform.DOMove(gameManager.GetParent().position + new Vector3(j * gameManager.perWidthDistance, 0), 0.2f);


                //动画可能会有很多，为了完成全部动画，加入一个计数器来帮助我们确认这些动画是否都完成了
                gameManager.AddAnimationCount();

                go.transform.parent = gameManager.GetParent();

                //当上述动画完成后
                gameManager.SetCandyColorInBoard(0, j, tempColor);
                gameManager.SetCandyInBoard(0, j, tempControl);
                tweener.OnComplete(() =>
                {
                    gameManager.SubAnimation();
                });

                isFall = true;
            }
        }

        for (int i = 0; i < gameManager.row - 1; i++)
        {
            for (int j = 0; j < gameManager.col; j++)
            {
                //如果这个格子有糖果，下面格子没有，就将这个格子的下落
                if (gameManager.IsCandy(i, j) && !gameManager.IsCandy(i + 1, j))
                {
                    CandyControl curCandy = gameManager.GetCandyInBoard(i, j);
                    Transform go = curCandy.transform;
                    Tweener tweener = go.DOMove(new Vector3(j * gameManager.perWidthDistance, -(i + 1) * gameManager.perHeighDistance, 0), 0.2f);

                    gameManager.AddAnimationCount();

                    gameManager.SwapTwoCandy(i, j, i + 1, j);

                    tweener.OnComplete(() =>
                    {
                        gameManager.SubAnimation();
                    });
                    isFall = true;
                }
            }
        }

        return isFall;

    }

}
