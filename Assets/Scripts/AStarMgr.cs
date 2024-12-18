using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// A*������
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

    //��ͼ�Ŀ��
    private int mapWidth;
    private int mapHeight;

    //��ͼ���еĸ��Ӷ�������
    public AStarNode[,] nodes;
    //�����б�
    private List<AStarNode> openList;
    //�ر��б�
    private List<AStarNode> closeList;

    /// <summary>
    /// ��ʼ����ͼ��Ϣ
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void IintMap(int width, int height)
    {
        //���ݵ�ͼ�Ŀ������ʼ����ͼ��Ϣ
        //�赲������������ã���Ϊ��ʱû�е�ͼ������Ϣ
        mapWidth = width;
        mapHeight = height;
        nodes = new AStarNode[mapWidth, mapHeight];
        openList = new List<AStarNode>();
        closeList = new List<AStarNode>();


        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                //��һ�������Ǵ�������һ�и��Ӷ��� �Ǹ����ӵ� ���ϰ�һ���Ƕ���ͼ�����ļ��У�
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walkable);
                nodes[i, j] = node;
            }
        }


    }


    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {

        //ʵ����Ŀ�д�����������������꣬������Ҫת���ɸ�������
        //����Ͳ���ת���ˣ�ֱ�Ӵ����������

        //�����жϴ���������յ��Ƿ�Ϸ�
        //1.����Ҫ�ڵ�ͼ��Χ��
        if (startPos.x < 0 || startPos.x >= mapWidth || startPos.y < 0 || startPos.y >= mapHeight
            || endPos.x < 0 || endPos.x >= mapWidth || endPos.y < 0 || endPos.y >= mapHeight)
        {
            return null;
        }
        //2.Ҫ�����赲
        //3.������Ϸ�ֱ�ӷ���null ����Ѱ·
        AStarNode startNode = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode endNode = nodes[(int)endPos.x, (int)endPos.y];
        if (startNode.NodeType == E_Node_Type.Stop || endNode.NodeType == E_Node_Type.Stop)
        {
            Debug.Log("��ʼ���߽��������赲");
            return null;
        }

        //��տ����͹ر��б� ������һ��Ѱ·�����ݸ���
        closeList.Clear();
        openList.Clear();

        //�ѿ�ʼ�����ر��б���(ÿ�ο�ʼǰ��Ҫ����)
        startNode.Father = null;
        startNode.G = 0;
        startNode.F = 0;
        startNode.H = 0;
        closeList.Add(startNode);

        //��Ҫ�ظ��Ľ���Ѱ·����
        while (true)
        {
            //Ӧ�õõ������յ�ĸ��Ӷ���

            //����㿪ʼ �ҵ���Χ8�����Ӷ��󣬲����뿪���б���
            //8����λ�ֱ��ǣ���������ϵ�����Ͻ�Ϊ��׼���ұ�x++  ����y++��
            //���� x - 1  y -1
            FindNearlyNodeToOpenList(startNode.X - 1, startNode.Y - 1, 1.4f, startNode, endNode);
            //�� x  y -1
            FindNearlyNodeToOpenList(startNode.X, startNode.Y - 1, 1, startNode, endNode);
            //���� x + 1 y -1
            FindNearlyNodeToOpenList(startNode.X + 1, startNode.Y - 1, 1.4f, startNode, endNode);
            //�� x - 1 y
            FindNearlyNodeToOpenList(startNode.X - 1, startNode.Y, 1, startNode, endNode);
            //�� x + 1 y
            FindNearlyNodeToOpenList(startNode.X + 1, startNode.Y, 1, startNode, endNode);
            //���� x - 1 y + 1
            FindNearlyNodeToOpenList(startNode.X - 1, startNode.Y + 1, 1.4f, startNode, endNode);
            //�� x y + 1
            FindNearlyNodeToOpenList(startNode.X, startNode.Y + 1, 1, startNode, endNode);
            //���� x+1 y+1 
            FindNearlyNodeToOpenList(startNode.X + 1, startNode.Y + 1, 1.4f, startNode, endNode);

            //��·�ж� �����б���Ϊ���ˣ�˵��û���ҵ�·�� ����Ϊ����·
            if (openList.Count == 0)
            {
                Debug.Log("û���ҵ�·��,��·");
                return null;
            }

            //ѡ�������б���FֵҲ����Ѱ·������С�ĵ�
            openList.Sort(SortOpenList);


            //ѡ�������б���FֵҲ����Ѱ·������С�ĵ� ����ر��б���
            closeList.Add(openList[0]);
            //�������Ҫ���ҵ������ �ֱ���µ���� ������һ�ε�Ѱ·������
            startNode = openList[0];
            //�������ӿ����б����Ƴ�
            openList.RemoveAt(0);


            //���������Ѿ����յ��ˣ���ô�õ����ս�����س�ȥ
            //��������յ㣬��ô�������������Χ��8���㣬����Ѱ·
            if (startNode == endNode)
            {
                //�ҵ����յ� ������
                List<AStarNode> pathlist = new List<AStarNode>();
                while (endNode.Father !=null)
                {
                    //�Ѹ��׶�����ӽ�ȥ Ҳ����·����
                    pathlist.Add(endNode.Father);
                    //����Ѱ�Ҳ�Ϊ�յĸ��׶���
                    endNode = endNode.Father;
                }
                //��Ϊ�Ǵ��յ㿪ʼ�ҵģ�����Ҫ��תһ��
                pathlist.Reverse();

                //����������յ�·����
                return pathlist;
            }
        }
    }


    /// <summary>
    /// ����Χ�ĵ���뿪���б���
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void FindNearlyNodeToOpenList(int x, int y,float g,AStarNode father,AStarNode end)
    {

        //�ж���Щ����1.���Ǳ߽� 2.�Ƿ����赲 3.�Ƿ��ڿ������߹ر��б��� ��������ǲŷ��뿪���б���

        //�߽��ж�
        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
        {
            return;
        }

        //ȡ�������
        AStarNode node = nodes[x, y];
        if (node ==null || node.NodeType == E_Node_Type.Stop || 
            closeList.Contains(node) || openList.Contains(node))
        {
            return;
        }

        //����������G H Fֵ F= G + H
        //��¼������
        node.Father = father;
        //����Gֵ(�����������ľ���+�������𵽵ľ���)
        node.G = father.G + g;
        //����Hֵ(�������յ�ľ��� ����Ҫ�����ֵ)
        node.H = Mathf.Abs(end.X - node.X) + Mathf.Abs(end.Y - node.Y);
        //����Ѱ·����Fֵ
        node.F = node.G + node.H;



        //���ͨ����������жϣ���ô�Ͱ��������뿪���б���
        openList.Add(node);
    }


    /// <summary>
    /// �Զ���������
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
