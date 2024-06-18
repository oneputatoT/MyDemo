using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{ 
    Play,  //用户可以交换相邻糖果状态
    Match,  //检测是否可以删除与游玩状态
    Clear,  //清楚符合条件的糖果
    Down,   //补充糖果
    Shuffle,  //重置棋盘
    Anima,   //等待动画期间
    Creat,   //创建特殊糖果模式
}


public interface IMachine
{

    public void Start();

    public void Update();

    public void Exit();

    public void SwitchState(GameState state);

    public void AddState(GameState stateType, IState state);

    public void ChangeNextState();

    public void SetNextState(GameState stateType);

}
