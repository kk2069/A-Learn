using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAstar : MonoBehaviour
{
    //���Ͻǵ�һ���������λ��
    public int beginX = 3;
    public int beginY = 5;

    //֮��ÿ��������֮���λ��ƫ��
    public int offsetX = 2;
    public int offsetY = -2;

    //��ͼ�Ŀ��
    public int mapWidth = 5;
    public int mapHeight = 5;

    //�谭������ ��ɫ
    public Material stopMaterial;
    //��ʼ������� ��ɫ
    public Material StartPosMaterial;
    //·������ ��ɫ
    public Material PatnMaterial;
    //���óɰ�ɫ
    public Material NormalMaterial;


    private Dictionary<string, GameObject> cubeDic = new Dictionary<string, GameObject>();

    //��ʼ�� ����һ��Ϊ���������
    private Vector2 begingPos = Vector2.right * -1;

    //����·����List
    private List<AStarNode> list = new List<AStarNode>();


    private void Start()
    {
        //��ʼ�����Ե�ͼ
        AStarMgr.GetInstance().IintMap(mapWidth, mapHeight);

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                //����������
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);

                //���������������
                obj.name = i + "_" + j;
                //�洢�����嵽�ֵ���
                cubeDic.Add(obj.name, obj);

                AStarNode node = AStarMgr.Instance.nodes[i,j];
                if (node != null)
                {
                    if (node.NodeType == E_Node_Type.Stop)
                    {
                        //�赲Ϊ��ɫ�ϰ���
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
            //��ȡ�������λ��
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000))
            {
                //û�п�ʼ�� ��Ҫ��¼�����ʼ��  
                if (begingPos == Vector2.right * -1)
                {
                    //���µ����ʱ�����·���� ������һ�ε�·����
                    //��·�������óɰ�ɫ
                    if (list != null)
                    {
                        //�����Ϊ�� ���ҳɹ���
                        for (int i = 0; i < list.Count; ++i)
                        {
                            //��·���ϵĵ����ó���ɫ
                            cubeDic[list[i].X + "_" + list[i].Y].GetComponent<MeshRenderer>().material = NormalMaterial;
                        }
                    }

                    //�õ�������������� ����֪���ڼ��еڼ��� �õ���0��λ������ ��1��λ������
                    string[] strArr = hitInfo.collider.name.Split('_');
                    //�õ������λ�� ���� �������õĿ�ʼ���λ��
                    begingPos = new Vector2(int.Parse(strArr[0]), int.Parse(strArr[1]));

                    //�ѵ�����Ķ���ת���ɻ�ɫ
                    hitInfo.collider.gameObject.GetComponent<MeshRenderer>().material = StartPosMaterial;
                }
                else
                {
                    //������� ��������� ������յ�������Ѱ·
                    string[] strArr = hitInfo.collider.name.Split('_');
                    //�õ��յ�
                    var endPos = new Vector2(int.Parse(strArr[0]), int.Parse(strArr[1]));

                    //��·�ѿ�ʼ·�������óɰ�ɫ  ������·��ʱ���ɫ�������ɫ ����������һ��
                    cubeDic[(int)begingPos.x + "_" + (int)begingPos.y].GetComponent<MeshRenderer>().material = NormalMaterial;

                    //��ʼѰ·
                    list = AStarMgr.GetInstance().FindPath(begingPos, endPos);
                    if (list !=null)
                    {
                        //�����Ϊ�� ���ҳɹ���
                        for (int i = 0; i < list.Count; ++i)
                        {
                            //��·���ϵĵ����ó���ɫ
                            cubeDic[list[i].X + "_" + list[i].Y].GetComponent<MeshRenderer>().material = PatnMaterial;
                        }
                    }

                  
                    //�����ʼ�㣬�´ε�����������µĿ�ʼ��
                    begingPos = Vector2.right * -1;
                }
            }
        }
    }
}
