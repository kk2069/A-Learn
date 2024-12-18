using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public enum E_Node_Type
{
    //�����ߵĵط�
    Walkable,
    //�����ߵ��赲
    Stop,
}

/// <summary>
/// A*������
/// </summary>
public class AStarNode
{
    //�����ľ���
    public float G { get; set; }
    //���յ�ľ���
    public float H { get; set; }
    //Ѱ·����
    public float F { get; set; }
    //������
    public AStarNode Father;

    //���Ӷ�������
    public int X;
    public int Y;

    //��������
    public E_Node_Type NodeType;

    /// <summary>
    /// ���캯�� �����������͸�������
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
