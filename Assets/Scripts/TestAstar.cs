using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAstar : MonoBehaviour
{
    //左上角第一个立方体的位置
    public int beginX = 3;
    public int beginY = 5;

    //之后每个立方体之间的位置偏移
    public int offsetX = 2;
    public int offsetY = -2;

    //地图的宽高
    public int mapWidth = 5;
    public int mapHeight = 5;

    //阻碍材质球 红色
    public Material stopMaterial;
    //开始点材质球 黄色
    public Material StartPosMaterial;
    //路径材质 绿色
    public Material PatnMaterial;
    //设置成白色
    public Material NormalMaterial;


    private Dictionary<string, GameObject> cubeDic = new Dictionary<string, GameObject>();

    //开始点 给他一个为负的坐标点
    private Vector2 begingPos = Vector2.right * -1;

    //最终路径点List
    private List<AStarNode> list = new List<AStarNode>();


    private void Start()
    {
        //初始化测试地图
        AStarMgr.GetInstance().IintMap(mapWidth, mapHeight);

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                //创建立方体
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);

                //设置立方体的名字
                obj.name = i + "_" + j;
                //存储立方体到字典中
                cubeDic.Add(obj.name, obj);

                AStarNode node = AStarMgr.Instance.nodes[i,j];
                if (node != null)
                {
                    if (node.NodeType == E_Node_Type.Stop)
                    {
                        //阻挡为红色障碍物
                        obj.GetComponent<MeshRenderer>().material = stopMaterial;
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //获取鼠标点击的位置
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000))
            {
                //没有开始点 需要记录这个开始点  
                if (begingPos == Vector2.right * -1)
                {
                    //重新点击的时候清除路径点 清理上一次的路径点
                    //把路径点设置成白色
                    if (list != null)
                    {
                        //如果不为空 则找成功了
                        for (int i = 0; i < list.Count; ++i)
                        {
                            //把路径上的点设置成绿色
                            cubeDic[list[i].X + "_" + list[i].Y].GetComponent<MeshRenderer>().material = NormalMaterial;
                        }
                    }

                    //得到点击到的立方体 才能知道第几行第几列 得到第0个位置是行 第1个位置是列
                    string[] strArr = hitInfo.collider.name.Split('_');
                    //得到点击的位置 行列 就是设置的开始点的位置
                    begingPos = new Vector2(int.Parse(strArr[0]), int.Parse(strArr[1]));

                    //把点击到的对象转换成黄色
                    hitInfo.collider.gameObject.GetComponent<MeshRenderer>().material = StartPosMaterial;
                }
                else
                {
                    //有起点了 那这里就是 来点出终点来进行寻路
                    string[] strArr = hitInfo.collider.name.Split('_');
                    //得到终点
                    var endPos = new Vector2(int.Parse(strArr[0]), int.Parse(strArr[1]));

                    //死路把开始路径点设置成白色  避免死路的时候黄色不变成绿色 所以事先清一次
                    cubeDic[(int)begingPos.x + "_" + (int)begingPos.y].GetComponent<MeshRenderer>().material = NormalMaterial;

                    //开始寻路
                    list = AStarMgr.GetInstance().FindPath(begingPos, endPos);
                    if (list !=null)
                    {
                        //如果不为空 则找成功了
                        for (int i = 0; i < list.Count; ++i)
                        {
                            //把路径上的点设置成绿色
                            cubeDic[list[i].X + "_" + list[i].Y].GetComponent<MeshRenderer>().material = PatnMaterial;
                        }
                    }

                  
                    //清除开始点，下次点击重置设置新的开始点
                    begingPos = Vector2.right * -1;
                }
            }
        }
    }
}
