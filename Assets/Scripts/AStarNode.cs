using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子类型
/// </summary>
public enum E_Node_Type
{
    //可以走的地方
    Walkable,
    //不能走的阻挡
    Stop,
}

/// <summary>
/// A*格子类
/// </summary>
public class AStarNode
{
    //离起点的距离
    public float G { get; set; }
    //离终点的距离
    public float H { get; set; }
    //寻路消耗
    public float F { get; set; }
    //父对象
    public AStarNode Father;

    //格子对象坐标
    public int X;
    public int Y;

    //格子类型
    public E_Node_Type NodeType;

    /// <summary>
    /// 构造函数 传入格子坐标和格子类型
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="type"></param>
    public AStarNode(int x,int y,E_Node_Type type)
    {
        X = x;
        Y = y;
        NodeType = type;
    }

    

}
