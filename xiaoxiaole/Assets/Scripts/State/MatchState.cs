using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchState : GameSystemState
{

    public MatchState(GameManager gameManager, IMachine machine) : base(gameManager, machine)
    {
    }

    public override void Update()
    {
        base.Update();
        //如果有存在可以删除的对象存在，切换ClearState
        if (gameManager.Match())
        {
            machine.SwitchState(GameState.Clear);
        }
        else
        {
            //如果没有可以删除的对象，检测棋盘是否可玩，可玩则切换Play模式，洗牌
            if (gameManager.CheckGameAble())
            {
                machine.SwitchState(GameState.Play);
            }
            //不可玩Shuffle状态,洗牌
            else
            {
                machine.SwitchState(GameState.Shuffle);
            }

        }

    }
}
