 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using System.Diagnostics.Contracts;


public class GameManager : MonoBehaviour
{
    //单例
    public static GameManager instance
    {
        private set;
        get; 
    }


    //资源类读取
    Factory resourceFactory;
    public LoadDataPath dataPath;

    //属性生成工厂
    AttributeFactory attributeFactory;

    //棋盘检测是否错误的类
    PatternCheck patternCheck;


    [Header("棋盘设置")]
    public int row;
    public int col;
    //点与点之间的距离
    public float perWidthDistance;
    public float perHeighDistance;
    Transform parent;


    //糖果设置
    //用数字来标识是什么糖果
    //int.MaxValue代表无糖果
    //0-n代表几号贴图糖果
    [Header("糖果颜色")]
    public int candyColors;

    [Header("糖果种类")]
    public int candyVariety;

    int[,] candyColorInBoard;
    CandyControl[,] candiesControlInBoard;


    public int animationCount;


    //状态机
    IMachine playerMachine;

    //游戏操作检测
    public bool buttonDown;
    public bool buttonUp;

    [Header("菱形糖果爆炸范围")]
    public int diamondRadius;

    //需要删除的存储
    public List<Vector2Int> curDelectList
    {
        get;private set;
    }

    public List<Vector2Int> nextDelectList
    {
       get; private set;
    }


    //要转换的Index
    public List<List<CandyInFo>> needAllInList
    {
        get; private set;
    }//存可能变为Allin糖果的Index
    public List<List<CandyInFo>> needDiamondList
    {
        get; private set;
    }//存可能变为Diamond糖果的Index
    public List<List<CandyInFo>> needHorizontalList
    {
        get; private set;
    }//存可能变为Horizontal糖果的Index
    public List<List<CandyInFo>> needVerticalList
    {
        get; private set;
    }//存可能变为Vertical糖果的Index



    private void Awake()
    {
        if (instance == null)
        {
            instance = this as GameManager;
        }
        else
        {
            Destroy(gameObject);
        }
        
        
    }


    void OnEnable()
    {
        resourceFactory = new Factory(this);
        patternCheck = new PatternCheck();
        attributeFactory = new AttributeFactory(resourceFactory, this);

        //初始化棋盘背景
        InitializationSquare();

        //初始化糖果
        InitializationCandy();

    }

    private void Start()
    {
        curDelectList = new List<Vector2Int>();
        nextDelectList = new List<Vector2Int>();

        needAllInList = new List<List<CandyInFo>>();
        needDiamondList = new List<List<CandyInFo>>(); 
        needHorizontalList = new List<List<CandyInFo>>(); 
        needVerticalList = new List<List<CandyInFo>>();  



        playerMachine = new PlayerMachine(this);
        //注册状态
        playerMachine.AddState(GameState.Down, new DownState(this,playerMachine));
        playerMachine.AddState(GameState.Anima, new AnimaState(this, playerMachine));
        playerMachine.AddState(GameState.Play, new PlayState(this, playerMachine));
        playerMachine.AddState(GameState.Clear, new ClearState(this, playerMachine));
        playerMachine.AddState(GameState.Match, new MatchState(this, playerMachine));
        playerMachine.AddState(GameState.Shuffle, new ShuffleState(this, playerMachine));
        playerMachine.AddState(GameState.Creat, new CreatState(this, playerMachine));

        animationCount = 0;

        playerMachine.SwitchState(GameState.Down);

    }

    private void Update()
    {
        playerMachine.Update();

 
   
        buttonDown = Input.GetMouseButtonDown(0);
        buttonUp = Input.GetMouseButtonUp(0);



    }


    //初始化棋盘
    void InitializationSquare()
    {
        List<Sprite> squareSprites = new List<Sprite>();
        for (int i = 0; i < dataPath.squareTexPath.Length; i++)
        {
            squareSprites.Add(resourceFactory.LoadSprite(dataPath.squareTexPath[i]));
        }

        //让背景块储存在工厂中
        GameObject go = resourceFactory.LoadObjectFromResources(dataPath.squarePrefabPath);
        parent = GameObject.Find("GameField").transform;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                GameObject tempGo = Instantiate(go, new Vector3(j * perWidthDistance, -i * perHeighDistance, 0), Quaternion.identity);
                tempGo.GetComponent<Square>().index=new Vector2Int(i,j);

                tempGo.GetComponent<SpriteRenderer>().sprite = squareSprites[(i * col + j) % 2];

                tempGo.transform.parent = parent;
            }
        }

        Camera cam = Camera.main;
        cam.GetComponent<Transform>().position = new Vector3((col + 1) / 2, -(row + 1) / 2, -10);
    }

    //初始化糖果
    void InitializationCandy()
    {
        candyColorInBoard = new int[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                candyColorInBoard[i, j] = int.MaxValue;
            }
        }

        candiesControlInBoard = new CandyControl[row, col];


        //将糖果贴图转移至ResourceFactory
        resourceFactory.SetCandiesSprite(candyColors, candyVariety,dataPath);

        //爆炸预制体也转移至ResourceFactory
        resourceFactory.SetExplosionPrefabs(dataPath);

        //让糖果预制体储存在工厂中
        resourceFactory.LoadObjectFromResources(dataPath.candyPrefabPath);

        //AllIn贴图
        resourceFactory.LoadCandySprite(dataPath.candyAllInPath);

        //设置糖果属性工厂
        attributeFactory.SetCandyAttrubuteList();
        attributeFactory.SetCandyAllInAttrubute(dataPath);
    }


    public bool IsCandy(int i, int j)
    {
        if (i < 0 || j < 0 || i >= row || j >= col)
        {
            return false;
        }

        if (candyColorInBoard[i, j] >= 0 && candyColorInBoard[i, j] < candyColors)
        {
            return true;
        }

        if (candiesControlInBoard[i, j] != null)
        {
            if (candiesControlInBoard[i, j].GetCandyType() == CandyType.AllIn)
            {
                return true;
            }
        }

        return false;
    }

    public GameObject GetCandyGO()
    {
        return resourceFactory.LoadObjectFromResources(dataPath.candyPrefabPath);
    }

    public Transform GetParent()
    {
        return parent;
    }

    public CandyAttribute GetCandyAttr(int color,CandyType candyType)
    {
        return attributeFactory.GetAttribute(color, candyType);
    }

    public int GetCandyColorInBoard(int i,int j) 
    {
        return candyColorInBoard[i, j];
    }

    public void CreateCandy(int i, int j, int color, CandyType type)
    {
        GameObject go = Instantiate(GetCandyGO(),GetParent().position+new Vector3(j * perWidthDistance, -i * perWidthDistance, 0), Quaternion.identity);
        CandyControl cc = go.GetComponent<CandyControl>();
        CandyAttribute ca = attributeFactory.GetAttribute(color, type);
        cc.SetAttr(ca);
        cc.SetCandySprite();
        
        go.transform.parent = GetParent();

        SetCandyInBoard(i, j, cc);
        SetCandyColorInBoard(i, j, cc.GetColor());
    }

    //设置candyColorInBoard棋盘上的颜色
    public void SetCandyColorInBoard(int i, int j,int color)
    {
        candyColorInBoard[i, j] = color;
    }

    //设置candiesControlInBoard上的CandyControl
    public void SetCandyInBoard(int i,int j,CandyControl control)
    {
        candiesControlInBoard[i,j] = control;
    }

    public CandyControl GetCandyInBoard(int i, int j)
    {
        return candiesControlInBoard[i, j];
    }

    public CandyType GetCandyType(int i, int j)
    {
        return GetCandyInBoard(i, j).GetCandyType();
    }

    /// <summary>
    /// 将当前行(<paramref name="curR"/>)和列(<paramref name="curC"/>)与
    /// 相邻的行(<paramref name="nextR"/>)和列(<paramref name="nextC"/>)进行交换
    /// </summary>
    /// <param name="curR">当前的行.</param>
    /// <param name="curC">当前的列.</param>
    /// <param name="nextR">要交换的行.</param>
    /// <param name="nextC">要交换的列.</param>


    public void SwapTwoCandy(int curR,int curC,int nextR,int nextC)
    {
        int tempColor = candyColorInBoard[curR, curC];
        candyColorInBoard[curR, curC] = candyColorInBoard[nextR, nextC];
        candyColorInBoard[nextR, nextC] = tempColor;

        if (candiesControlInBoard[curR, curC] != null)
        {
            candiesControlInBoard[curR, curC].SetIndex(nextR, nextC);
        }

        if (candiesControlInBoard[nextR, nextC] != null)
        {
            candiesControlInBoard[nextR, nextC].SetIndex(curR, curC);
        }


        CandyControl tempCandyControl = candiesControlInBoard[curR, curC];
        candiesControlInBoard[curR, curC] = candiesControlInBoard[nextR, nextC];
        candiesControlInBoard[nextR, nextC] = tempCandyControl;
    }

    public void SwapTwoCandyPositong(int curR, int curC, int nextR, int nextC)
    {
        Vector3 tempVec = candiesControlInBoard[curR, curC].transform.position;
        candiesControlInBoard[curR, curC].transform.position = candiesControlInBoard[nextR, nextC].transform.position;
        candiesControlInBoard[nextR, nextC].transform.position = tempVec;
    }

    public void AddAnimationCount()
    {  
        animationCount++;
    }

    public void SubAnimation()
    {
        animationCount--;
        animationCount = Mathf.Max(0, animationCount);
    }

    public bool AnimationIsDone()
    {
        return animationCount == 0;
    }

    //删除列表相关
    public void AddToTempDelectList(Vector2Int temp)
    {
        int r = temp.x;
        int c = temp.y;

        if (r < 0 || r >= row || c < 0 || c >= col)
        {
            return;
        }

        if (!nextDelectList.Contains(temp)&&IsCandy(r,c))
        { 
            nextDelectList.Add(temp);
        }
    }
    public void ClearTempDelectList()
    {
        nextDelectList.Clear();
    }

    public void AddToDelectList(Vector2Int temp)
    {
        curDelectList.Add(temp);
    }

    public void ClearDelectList()
    {
        curDelectList.Clear();
    }


    //根据位置和颜色生成爆炸特效
    public void CreateExplosion(int i,int j,int color)
    {
        Instantiate(resourceFactory.GetExplosionPrefabs(color),
            new Vector3(j * perWidthDistance, -i * perHeighDistance, 0), Quaternion.identity);
    }

    public bool CanMakeSpecialCandy()
    {
        return needAllInList.Count != 0 || needDiamondList.Count != 0 
            || needHorizontalList.Count != 0 || needVerticalList.Count != 0;
    }


    //此方法因为会被Play状态和Match状态引用，所以作为公共方法
    public bool Match()
    {
        Vector2Int[,] sameColorCount = new Vector2Int[row, col];
        Stack<Vector2Int> st = new Stack<Vector2Int>();
        int theColor = -1;
        int flag = 0;

        //匹配时，先把要删除的列表清空
        curDelectList.Clear();
        nextDelectList.Clear();

        bool needDelect = false;


        //扫描行
        for (int i = 0; i < row; i++)
        {
            theColor = -1;
            flag = 0;
            for (int j = 0; j < col; j++)
            {
                if (GetCandyColorInBoard(i,j) == candyColors)
                {
                    while (st.Count > 0)
                    {
                        Vector2Int temp = st.Pop();
                        sameColorCount[temp.x, temp.y].x = flag;
                    }
                    flag = 0;
                    continue;
                }


                //如果颜色相同就记录这个位置
                if (GetCandyColorInBoard(i, j) == theColor)
                {
                    flag++;
                    st.Push(new Vector2Int(i, j));
                }
                else
                {
                    while (st.Count != 0)
                    {
                        Vector2Int temp = st.Pop();
                        //Vector2Int中x代表行有多少相同的
                        sameColorCount[temp.x, temp.y].x = flag;
                    }

                    st.Push(new Vector2Int(i, j));
                    flag = 1;
                    theColor = GetCandyColorInBoard(i, j);
                }

                if (j == col - 1)
                {
                    while (st.Count != 0)
                    {
                        Vector2Int temp = st.Pop();
                        //Vector2Int中x代表行有多少相同的
                        sameColorCount[temp.x, temp.y].x = flag;
                    }
                }
            }
        }

        //扫描列
        for (int j = 0; j < col; j++)
        {
            theColor = -1;
            flag = 0;
            for (int i = 0; i < row; i++)
            {
                if (GetCandyColorInBoard(i, j) == candyColors)
                {
                    while (st.Count > 0)
                    {
                        Vector2Int temp = st.Pop();
                        sameColorCount[temp.x, temp.y].y = flag;
                    }
                    flag = 0;
                    continue;
                }


                //如果颜色相同就记录这个位置
                if (GetCandyColorInBoard(i, j) == theColor)
                {
                    flag++;
                    st.Push(new Vector2Int(i, j));
                }
                else
                {
                    while (st.Count != 0)
                    {
                        Vector2Int temp = st.Pop();
                        //Vector2Int中y代表列有多少相同的
                        sameColorCount[temp.x, temp.y].y = flag;
                    }

                    st.Push(new Vector2Int(i, j));
                    flag = 1;
                    theColor = GetCandyColorInBoard(i, j);
                }

                if (i == row - 1)
                {
                    while (st.Count != 0)
                    {
                        Vector2Int temp = st.Pop();
                        //Vector2Int中y代表列有多少相同的
                        sameColorCount[temp.x, temp.y].y = flag;
                    }
                }
            }
        }


        //遍历全部，找到可以删除的对象
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (sameColorCount[i, j].x > 2 || sameColorCount[i, j].y > 2)
                {
                    AddToTempDelectList(new Vector2Int(i, j));
                    needDelect = true;
                }
            }
        }

        //如果没有要删除的，直接返回false，即没有匹配的选项
        if (!needDelect)
        {
            return false;
        }



        ClearAllSpcCandyList();//先清空之前找到的符合条件的方块

        //如果相同颜色存在五个以上
        AddSpecialCandyRow(sameColorCount,5,needAllInList,CandyType.AllIn);
        AddSpecialCandyCol(sameColorCount,5,needAllInList,CandyType.AllIn);


        //菱形
        AddDiamondCandy(sameColorCount, CandyType.Diamond);

        //同一行有4个同样的颜色的Candy
        AddSpecialCandyRow(sameColorCount, 4, needHorizontalList,CandyType.Horizontal);

        //同一列有4个同样的颜色的Candy
        AddSpecialCandyCol(sameColorCount, 4, needVerticalList, CandyType.Vertical);
        

        return needDelect;
    }


    public bool CheckGameAble()
    {
        //bool isAble = patternCheck.Check(candyColorInBoard);

        return patternCheck.Check(candyColorInBoard);

    }



    void AddSpecialCandyRow(Vector2Int[,] sameColorCount,int step,List<List<CandyInFo>> list,CandyType type)
    {
        //先遍历列
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (sameColorCount[i, j].x == step)
                {
                    List<CandyInFo> temp = new List<CandyInFo>();
                    int curJ = j;
                    for (; j<col&&j <= curJ + step - 1; j++)
                    {
                        if (sameColorCount[i, j].x != -1)
                        {
                            temp.Add(new CandyInFo(new Vector2Int(i, j), GetCandyColorInBoard(i, j),
                                type));
                        }
                        sameColorCount[i, j].x = -1;
                    }
                    j--;
                    if (temp.Count == step)
                    {
                        list.Add(temp);
                    }
                }
            }
        }
    }

    void AddSpecialCandyCol(Vector2Int[,] sameColorCount, int step, List<List<CandyInFo>> list, CandyType type)
    {
        //再遍历行
        for (int j = 0; j < col; j++)
        {
            for (int i = 0; i < row; i++)
            {
                if (sameColorCount[i, j].y == step)
                {
                    List<CandyInFo> temp = new List<CandyInFo>();
                    int curI = i;
                    for (; i<row&&i <= curI + step - 1; i++)
                    {
                        if (sameColorCount[i, j].y != -1)
                        {
                            temp.Add(new CandyInFo(new Vector2Int(i, j), GetCandyColorInBoard(i, j),
                                type));
                        }
                        sameColorCount[i, j].y = -1;
                    }
                    i--;
                    if (temp.Count == step)
                    {
                        list.Add(temp);
                    }
                }
            }
        }
    }

    void AddDiamondCandy(Vector2Int[,] sameColorCount,CandyType type)
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if ((sameColorCount[i, j].x == 3 || sameColorCount[i, j].x == 4) &&
                    (sameColorCount[i, j].y == 3 || sameColorCount[i, j].y == 4))
                {
                    int n = 1;
                    List<CandyInFo> temp = new List<CandyInFo> ();
                    
                    int curColor = GetCandyColorInBoard(i, j);
                    temp.Add(new CandyInFo(new Vector2Int(i, j), curColor, CandyType.Diamond));

                    sameColorCount[i, j] = new Vector2Int(-1, -1);

                    //向右遍历
                    for (int x = j+1; x < col; x++)
                    {
                        if (sameColorCount[i, x].x != -1 && sameColorCount[i, x].y != -1 &&
                            GetCandyColorInBoard(i, x) == curColor)
                        {
                            sameColorCount[i, x] = new Vector2Int(-1, -1);
                            n++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //左
                    for (int x = j - 1; x >= 0; x--)
                    {
                        if (sameColorCount[i, x].x != -1 && sameColorCount[i, x].y != -1 &&
                            GetCandyColorInBoard(i, x) == curColor)
                        {
                            sameColorCount[i, x] = new Vector2Int(-1, -1);
                            n++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //下
                    for (int x = i + 1; x < row; x++)
                    {
                        if (sameColorCount[x, j].x != -1 && sameColorCount[x, j].y != -1 &&
                            GetCandyColorInBoard(i, x) == curColor)
                        {
                            sameColorCount[x, j] = new Vector2Int(-1, -1);
                            n++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //向上
                    for (int x = i - 1; x >=0; x--)
                    {
                        if (sameColorCount[x, j].x != -1 && sameColorCount[x, j].y != -1 &&
                            GetCandyColorInBoard(i, x) == curColor)
                        {
                            sameColorCount[x, j] = new Vector2Int(-1, -1);
                            n++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (n >= 5)
                    {
                        needDiamondList.Add(temp);
                    }
                }
            }
        }
    }


    public void ClearAllSpcCandyList()
    {
        needAllInList.Clear();
        needDiamondList.Clear();
        needHorizontalList.Clear();
        needVerticalList.Clear();
    }



}
