using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimaState : GameSystemState
{
    public AnimaState(GameManager gameManager, IMachine machine) : base(gameManager, machine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        //如果动画播完了才转换
        if (gameManager.AnimationIsDone())
        {
            machine.ChangeNextState();
        }
    }
}
