using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// A*管理器
/// </summary>
public class AStarMgr
{
    public static AStarMgr Instance;
    public static AStarMgr GetInstance()
    {
        if (Instance == null)
        {
            Instance = new AStarMgr();
        }
        return Instance;
    }

    //地图的宽高
    private int mapWidth;
    private int mapHeight;

    //地图所有的格子对象容器
    public AStarNode[,] nodes;
    //开启列表
    private List<AStarNode> openList;
    //关闭列表
    private List<AStarNode> closeList;

    /// <summary>
    /// 初始化地图信息
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void IintMap(int width, int height)
    {
        //根据地图的宽高来初始化地图信息
        //阻挡这里先随机设置，因为暂时没有地图配置信息
        mapWidth = width;
        mapHeight = height;
        nodes = new AStarNode[mapWidth, mapHeight];
        openList = new List<AStarNode>();
        closeList = new List<AStarNode>();


        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                //走一遍这里是创建出了一列格子对象 是高增加的 （障碍一般是读地图配置文件中）
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walkable);
                nodes[i, j] = node;
            }
        }


    }


    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {

        //实际项目中传入的往往是世界坐标，这里需要转换成格子坐标
        //这里就不做转换了，直接传入格子坐标

        //首先判断传入的起点和终点是否合法
        //1.首先要在地图范围内
        if (startPos.x < 0 || startPos.x >= mapWidth || startPos.y < 0 || startPos.y >= mapHeight
            || endPos.x < 0 || endPos.x >= mapWidth || endPos.y < 0 || endPos.y >= mapHeight)
        {
            return null;
        }
        //2.要不是阻挡
        //3.如果不合法直接返回null 结束寻路
        AStarNode startNode = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode endNode = nodes[(int)endPos.x, (int)endPos.y];
        if (startNode.NodeType == E_Node_Type.Stop || endNode.NodeType == E_Node_Type.Stop)
        {
            Debug.Log("开始或者结束点是阻挡");
            return null;
        }

        //清空开启和关闭列表 避免上一次寻路的数据干扰
        closeList.Clear();
        openList.Clear();

        //把开始点放入关闭列表中(每次开始前需要清下)
        startNode.Father = null;
        startNode.G = 0;
        startNode.F = 0;
        startNode.H = 0;
        closeList.Add(startNode);

        //需要重复的进行寻路计算
        while (true)
        {
            //应该得到起点和终点的格子对象

            //从起点开始 找到周围8个格子对象，并放入开启列表中
            //8个点位分别是（这里坐标系是左上角为基准，右边x++  下面y++）
            //左上 x - 1  y -1
            FindNearlyNodeToOpenList(startNode.X - 1, startNode.Y - 1, 1.4f, startNode, endNode);
            //上 x  y -1
            FindNearlyNodeToOpenList(startNode.X, startNode.Y - 1, 1, startNode, endNode);
            //右上 x + 1 y -1
            FindNearlyNodeToOpenList(startNode.X + 1, startNode.Y - 1, 1.4f, startNode, endNode);
            //左 x - 1 y
            FindNearlyNodeToOpenList(startNode.X - 1, startNode.Y, 1, startNode, endNode);
            //右 x + 1 y
            FindNearlyNodeToOpenList(startNode.X + 1, startNode.Y, 1, startNode, endNode);
            //左下 x - 1 y + 1
            FindNearlyNodeToOpenList(startNode.X - 1, startNode.Y + 1, 1.4f, startNode, endNode);
            //下 x y + 1
            FindNearlyNodeToOpenList(startNode.X, startNode.Y + 1, 1, startNode, endNode);
            //右下 x+1 y+1 
            FindNearlyNodeToOpenList(startNode.X + 1, startNode.Y + 1, 1.4f, startNode, endNode);

            //死路判断 开启列表中为空了，说明没有找到路径 则认为是死路
            if (openList.Count == 0)
            {
                Debug.Log("没有找到路径,死路");
                return null;
            }

            //选出开启列表中F值也就是寻路消耗最小的点
            openList.Sort(SortOpenList);


            //选出开启列表中F值也就是寻路消耗最小的点 放入关闭列表中
            closeList.Add(openList[0]);
            //这里很重要，找到这个点 又变成新的起点 进行下一次的寻路计算了
            startNode = openList[0];
            //把这个点从开启列表中移除
            openList.RemoveAt(0);


            //如果这个点已经是终点了，那么得到最终结果返回出去
            //如果不是终点，那么继续找这个点周围的8个点，继续寻路
            if (startNode == endNode)
            {
                //找到了终点 找完了
                List<AStarNode> pathlist = new List<AStarNode>();
                while (endNode.Father !=null)
                {
                    //把父亲对象添加进去 也就是路径点
                    pathlist.Add(endNode.Father);
                    //继续寻找不为空的父亲对象
                    endNode = endNode.Father;
                }
                //因为是从终点开始找的，所以要反转一下
                pathlist.Reverse();

                //这里就是最终的路径点
                return pathlist;
            }
        }
    }


    /// <summary>
    /// 把周围的点放入开启列表中
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void FindNearlyNodeToOpenList(int x, int y,float g,AStarNode father,AStarNode end)
    {

        //判断这些点是1.否是边界 2.是否是阻挡 3.是否在开启或者关闭列表中 如果都不是才放入开启列表中

        //边界判断
        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
        {
            return;
        }

        //取出这个点
        AStarNode node = nodes[x, y];
        if (node ==null || node.NodeType == E_Node_Type.Stop || 
            closeList.Contains(node) || openList.Contains(node))
        {
            return;
        }

        //计算这个点的G H F值 F= G + H
        //记录父对象
        node.Father = father;
        //计算G值(父对象离起点的距离+自身离起到的距离)
        node.G = father.G + g;
        //计算H值(自身离终点的距离 这里要算绝对值)
        node.H = Mathf.Abs(end.X - node.X) + Mathf.Abs(end.Y - node.Y);
        //计算寻路消耗F值
        node.F = node.G + node.H;



        //如果通过了上面的判断，那么就把这个点放入开启列表中
        openList.Add(node);
    }


    /// <summary>
    /// 自定义排序函数
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int SortOpenList(AStarNode a, AStarNode b)
    {
        if (a.F > b.F)
            return 1;
        else if (a.F == b.F)
            return 1;
        else
            return -1;
    }





}
