using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemState : IState
{

    protected GameManager gameManager;
    protected IMachine machine;
    protected int limit = 0;

    public GameSystemState(GameManager gameManager, IMachine machine)
    {
        this.gameManager = gameManager;
        this.machine = machine;
    }


    public virtual void Enter()
    {
        
    }


    public virtual void Update()
    {
        
    }


    public virtual void Exit()
    {
        
    }

}
