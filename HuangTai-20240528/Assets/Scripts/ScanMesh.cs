using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using HuangtaiPowerPlantControlSystem;
using System.Linq;
using System.Threading.Tasks;

public class ScanMesh : MonoBehaviour
{
    public struct DataListStruct {
        public List<Vector3> v3PointList;//点三维 数据列表
        public List<int> v3FaceIndexList;//面索引 数据列表

        public DataListStruct(List<Vector3> v3PointList, List<int> v3FaceIndexList) {
            this.v3PointList = v3PointList;
            this.v3FaceIndexList = v3FaceIndexList;
        }
    }//高层数据结构体
    public struct SaveArrDataStruct {
        public int idx;
        public int idz;
        public float new_y_data;

        public SaveArrDataStruct(int idx, int idz, float new_y_data) {
            this.idx = idx;
            this.idz = idz;
            this.new_y_data = new_y_data;
        }
    }//数据处理结构体

    public GameObject modelMask;

    private bool isRunning = false;//确保模型不在更新状态 
    private bool deleteIsOk = false;//判断旧的模型是否成功删除
    private bool readIsOK = false;//判断是否成功读取数据
    private bool isOnceGetModel = true;//判断是否是第一次加载模型

    DataListStruct meshStruct = new DataListStruct();//网格数据结构体
    List<SaveArrDataStruct> vectorSaveDataList = new List<SaveArrDataStruct>();//保存第一次的数据

    public Material modelMaterial;//模型材质球

    private void Awake()
    {
        //获得结构体预加载数据
        StartCoroutine(getStringByPath(@"/circle-50cm.ply"));
    }
    void Start()
    {
        UiStateSystem.changeThreeDModel = false;
    }

    void Update()
    {
        if (!readIsOK || DataManager.Instance.CurrentAccount == null)
            return;

        //更新模型
        updateModel();
    }

    private void updateModel() {

        if (!isRunning && UiStateSystem.changeThreeDModel)
        {
            isRunning = true;

            StartCoroutine(destroyMesh());//删除旧模型
        }

        if (deleteIsOk)
        {
            deleteIsOk = false;

            //modelMask.SetActive(true);
            //生成新模型
            GameObject childGameObj = createChildObj();
            getModelByData(childGameObj);
        }
    }

    //根据读取数据的类型 读取Json数值
    public CoalHeapDEM ReadJson(string typeNum)
    {
        if (typeNum == "Text")
        {
            //string demoText = Address.demoText;
            //CoalHeapDEM demData = JsonConvert.DeserializeObject<CoalHeapDEM>(demoText);    //用 Newtonsoft.Json 解析 JSON 数据
            //return demData;
        }
        else if (typeNum == "RealData")
        {
            if (UiStateSystem.curHeapDEM != null)
            {
                return UiStateSystem.curHeapDEM;
            }
            else
            {
                Debug.Log("未获取到三维数据");
            }
        }
        else
        {
            return null;
        }
        return null;
    }

    //新的数据来到后 0.5s后销毁之前的子物体
    private IEnumerator destroyMesh()
    {
        if (GetComponentsInChildren<Transform>(true).Length > 1) {
            Destroy(gameObject.transform.Find("New Game Object").gameObject);
        }

        yield return new WaitForSeconds(0.5f);
        deleteIsOk = true;
    }

    //创建子物体
    public GameObject createChildObj() {

        GameObject game = new GameObject();
        game.transform.SetParent(GameObject.Find("ScanMesh").transform);//生成子物体

        //设定子物体坐标
        game.transform.localPosition = new Vector3(0, -15f, 0);
        GameObject.Find("ScanMesh").transform.localScale = new Vector3(3.2f, 3.2f, 3.2f);
        game.transform.localScale = new Vector3(1, 1, 1);

        MeshFilter filter = game.AddComponent<MeshFilter>();//添加网格体过滤组件
        MeshRenderer renderer = game.AddComponent<MeshRenderer>();//添加渲染网格体组件
        //Material material = new Material(Shader.Find("Legacy Shaders/Diffuse"));//添加材质并适配对应的Shader
        //material.mainTexture = new Texture2D(1, 1);//更改材质上的贴图纹理为白模纯色
        renderer.sharedMaterial = modelMaterial;

        return game;
    }

    //根据传入物体赋网格模型
    public async void getModelByData(GameObject game)
    {
        //yield return  new WaitForSeconds(1.0f);

        //创建网格模型
        Mesh baseMesh = new Mesh();
        baseMesh.vertices = meshStruct.v3PointList.ToArray();
        baseMesh.triangles = meshStruct.v3FaceIndexList.ToArray();
        Vector3[] v3Arr = baseMesh.vertices;

        await Task.Run(() => {
            //数据处理 v3Arr更新 优化
            if (isOnceGetModel)
            {
                DealGetData(v3Arr);
                isOnceGetModel = false;
            }
            else
            {
                DealGetData(vectorSaveDataList, v3Arr);
            }
        });
        
        //更新网格模型
        baseMesh.vertices = v3Arr;
        MeshFilter filter = game.GetComponent<MeshFilter>();
        filter.mesh = baseMesh;

        //modelMask.SetActive(false);

        //重置标识位
        isRunning = false;
        UiStateSystem.changeThreeDModel = false;
    }

    //根据路径取得数据
    public IEnumerator getStringByPath(string path) {

        string data = Application.streamingAssetsPath + path;
        WWW inf = new WWW(data);//读取数据

        meshStruct = MakeBaseMesh(inf.text);//数据处理为结构体
        readIsOK = true;

        yield return inf;
    }

    //根据传入的字符串 解析数据 创建基础网格
    public DataListStruct MakeBaseMesh(string arrData)
    {
        string[] splitArr = arrData.Split("\r\n"); //将数据处理为数组

        int Vertex_Count = 0;
        int Face_count = 0;
        int End_header = 0;

        DataListStruct dataListStruct = new DataListStruct();

        List<Vector3> list1 = new List<Vector3>();
        List<int> list2 = new List<int>();

        foreach (string plyData in splitArr) {//处理数组非数据部分
            if (plyData != "end_header")
            {
                string[] split = plyData.Split(new char[] { ' ' });
                if (split.Length >= 3)
                {
                    if (split[0] == "element" && split[1] == "vertex")
                    {
                        Vertex_Count = Convert.ToInt32(split[2]);
                    }
                    if (split[0] == "element" && split[1] == "face")
                    {
                        Face_count = Convert.ToInt32(split[2]);
                    }
                }
                End_header++;
            }
            else
            {
                break;
            }
        }

        for (int i = End_header + 1; i < End_header + 1 + Vertex_Count; i++)//循环获得顶点列表
        {
            string[] splitPt1 = splitArr[i].Split(new char[] { ' ' });
            float x = Convert.ToSingle(splitPt1[0]);
            float y = Convert.ToSingle(splitPt1[1]);
            float z = Convert.ToSingle(splitPt1[2]);

            list1.Add(new Vector3(x, 0, y));
        }

        for (int i = End_header + 1 + Vertex_Count; i < End_header + 1 + Vertex_Count + Face_count; i++)//循环获得面列表
        {
            string[] splitPt2 = splitArr[i].Split(new char[] { ' ' });

            int vertex_num = Convert.ToInt32(splitPt2[0]);
            int[] face = new int[vertex_num];
            for (int j = 1; j <= vertex_num; j++)
            {
                int ver_id = Convert.ToInt32(splitPt2[j]);
                face[j - 1] = ver_id;

            }
            int id0 = face[0];
            face[0] = face[1];
            face[1] = id0;

            for (int j = 1; j <= vertex_num; j++)
            {
                list2.Add(face[j - 1]);
            }
        }

        dataListStruct.v3PointList = list1;
        Debug.Log(list1.Count);
        dataListStruct.v3FaceIndexList = list2;
        Debug.Log(list2.Count);

        return dataListStruct;
    }

    //第一次 数据处理
    public void DealGetData(Vector3[] v3Arr)
    {
        //取得新的数据
        CoalHeapDEM demData = ReadJson("RealData");

        for (int id = 0; id < v3Arr.Count(); id++)
        {
            Vector3 ver_pos = v3Arr[id];//取出要处理的一个数据

            float new_x = ver_pos.x + demData.RAIDUS;
            float new_z = ver_pos.z + demData.RAIDUS;
            int idx = Math.Min(demData.NX - 1, (int)((new_x - demData.X0) / demData.DX));
            int idz = Math.Min(demData.NZ - 1, (int)((new_z - demData.Z0) / demData.DZ));
            float new_y = InterpolationProcessing(new_x, new_z, demData);

            SaveArrDataStruct dataSt = new SaveArrDataStruct();
            dataSt.idx = idx;
            dataSt.idz = idz;
            dataSt.new_y_data = new_y;
            vectorSaveDataList.Add(dataSt);//将要保存的存入列表

            v3Arr[id].y = new_y;
        }
    }

    //后续 数据处理
    public void DealGetData(List<SaveArrDataStruct> list, Vector3[] v3Arr)
    {
        //取得新的数据
        CoalHeapDEM demData = ReadJson("RealData");

        for (int id = 0; id < list.Count; id++)
        {
            SaveArrDataStruct dataSt = list[id];//遍历取列表数据
            float new_y_data = -demData.DEM[dataSt.idz, dataSt.idx];//根据列表的 x-idx y-idz 更改z-newy 的值

            v3Arr[id].y = new_y_data;
        }
    }

    //双线性插值运算
    public float InterpolationProcessing(float x,float z, CoalHeapDEM demData)
    {
        int m = (int)((z - demData.Z0) / demData.DZ);//根据要求的点的坐标 取得周围点的左下角索引
        int n = (int)((x - demData.X0) / demData.DX);

        if ((m < 0 || m >= demData.NZ)||(n<0||n>=demData.NX)) {
            return 0;
        }

        float u = (z - demData.Z0) / demData.DZ  - m;//获得要求的点的坐标 与 左下角坐标 的小数部分
        float v = (x - demData.X0) / demData.DX -n;

        float result = (1 - u) * (1 - v) * demData.DEM[m, n] + u * (1 - v) * demData.DEM[m + 1, n] + 
            (1 - u) * v * demData.DEM[m, n + 1] + u * v * demData.DEM[m + 1, n + 1];

        return -result;
    }
}