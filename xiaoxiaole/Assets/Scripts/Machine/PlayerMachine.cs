using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMachine : IMachine
{
    Dictionary<GameState, IState> stateList;

    IState curState;
    IState nextState;

    GameManager gameManager;



    public PlayerMachine(GameManager gameManager)
    {
        this.gameManager = gameManager;

        stateList = new Dictionary<GameState, IState>();
        

    }


    public void Start()
    {
        curState.Enter();
    }

    public void Update()
    {
        curState.Update();
    }

    public void Exit()
    {
        curState.Exit();
    }

    public void SwitchState(GameState state)
    {
        if (curState != null)
        {
            Exit();
        }

        if (stateList.ContainsKey(state))
        {
            curState = stateList[state];
        }
        
        Start();

    }

    void SwitchState(IState state)
    {
        if (curState == nextState)
        {
            return;
        }


        if (curState != null)
        {
            Exit();
        }

        curState = state;

        Start();

    }


    public void AddState(GameState stateType, IState state)
    {
        stateList.TryAdd(stateType, state);
    }

    public void ChangeNextState()
    {
        SwitchState(nextState);
    }

    public void SetNextState(GameState stateType)
    {
        nextState = stateList[stateType];
    }

}
