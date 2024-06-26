using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using HuangtaiPowerPlantControlSystem;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class SendMessageToPLC : MonoBehaviour
{
    void Awake()
    {
        /*
        Thread newThread = new Thread(ConnectPlcServer);
        newThread.Start();
        */
        ConnectPlcServer();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    /*

    public Form1()
    {
        InitializeComponent();
        this.FormClosed += MainForm_FormClosed;
    }
    private void Form1_Load(object sender, EventArgs e)
    {
        this.textBox1.Text = "127.0.0.1";
        this.textBox4.Text = "11000";
        this.textBox3.Text = "6";
        this.textBox5.Text = "1";
        this.comboBox2.Text = "1";

        command.QUERY_SYSTEM = "TEST";//子系统ID
    }

    */




    #region 声明变量
    /*  变量定义
     *  hostIP:用于存放服务器（本机）IP地址
     *  port:服务器的网络终结点，即IP:端口号
     *  point:存放用户输入的端口号
     *  mainSocket:用于主要数据流传输的套接字
     */

    IPAddress hostIP;
    IPEndPoint port;
    int point;
    Socket mainSocket;

    bool flag;
    bool flag2;
    bool flag3;

    private string data;
    public FullVariables fullVariables = new FullVariables();
    #endregion
    delegate void SetTextCallback(string text);

    #region 实例化对象
    //实例化样机（以后有别的类型的机器就添加相应的类到解决方案中，这里是把斗轮堆取料机作为样机）
    private ServerCommand command = new ServerCommand();

    // 设置序列化选项
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented // 设置缩进格式
    };
    #endregion



    //模块：网络通信
    #region 网络通信

    #region 尝试连接按钮事件
    private void ConnectPlcServer()
    {
        hostIP = IPAddress.Parse("192.168.1.10");
        point = int.Parse("13000");

        flag = true;
        flag2 = true;

        try
        {
            port = new IPEndPoint(hostIP, point);

            //实例化Socket对象
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSocket.Connect(port);

            Debug.Log("尝试连接至远程驱动端...\r\n");
            //Thread thread = new Thread(new ThreadStart(Proccess));
            //thread.Start();
            Proccess();
        }
        catch (Exception ey)
        {
            Debug.Log("服务器没有开启\r\n" + ey.Message);
        }




    }
    #endregion

    #region 网络通信进程
    private void Proccess()
    {
        /*
        while (flag)
        {
            if (mainSocket.Connected)
            {
                if (flag2 == true)
                {
                    //this.Invoke(new SetTextCallback(PrintText), new object[] { "已连接至远程驱动端\r\n" });
                    Debug.Log("已连接至远程驱动端\r\n");
                    flag2 = false;
                }
                Debug.Log("尝试获取Json\r\n");
            }
        }
        */

        if (mainSocket.Connected)
        {
            if (flag2 == true)
            {
                //this.Invoke(new SetTextCallback(PrintText), new object[] { "已连接至远程驱动端\r\n" });
                Debug.Log("已连接至远程驱动端\r\n");
                flag2 = false;
            }
            Debug.Log("尝试发送指令获取Json\r\n");
        }
        SendCommand();
    }
    #endregion

    private void GetJson()//从服务器获取Json数据
    {
        if (mainSocket.Connected)
        {
            Debug.Log("开始获取Json\r\n");
            byte[] frameLengthBuffer = new byte[4]; // 根据定义，数据帧长度为4个字节
            int frameLength = 0;

            mainSocket.Receive(frameLengthBuffer, frameLengthBuffer.Length, 0);
            frameLength = BitConverter.ToInt32(frameLengthBuffer, 0);


            byte[] receiveByte = new byte[frameLength];


            mainSocket.Receive(receiveByte, receiveByte.Length, 0);
            string strInfo = Encoding.BigEndianUnicode.GetString(receiveByte);


            string strInfo2 = frameLength.ToString();
            string jsontext = strInfo;

            fullVariables = JsonConvert.DeserializeObject<FullVariables>(jsontext);    //用 Newtonsoft.Json 解析 JSON 数据

            //this.Invoke(new SetTextCallback(GetJSON), new object[] { text });


            //Thread.Sleep(10);
        }
    }


    //发送指令到服务器获取JSON
    #region 套接字通信获取JSON
    public void SendCommand()
    {
        if (mainSocket.Connected)
        {
            int type1 = int.Parse("6");//数据类型
            int type2 = int.Parse("1");//命令类型
            string type3 = "";//命令名
            int type4 = int.Parse("1");//命令内容

            //查询PLC的一帧数据
            SetCommand(type1, type2, type3, type4);

            SendCommandToServer();

            GetJson();
        }
        else
        {
            Debug.Log("服务器连接异常");
        }
    }



    //方法：命令命名
    #region 命令命名
    private void SetCommand(int TYPE1, int TYPE2, string TYPE3, int TYPE4)
    {
        command.QUERY_SYSTEM = "MC";
        command.DATA_TYPE = TYPE1;
        command.QUERY_TYPE = TYPE2;
        command.COMMAND_NAME = TYPE3;
        command.COMMAND_TYPE = TYPE4;
    }
    #endregion



    #region 发送命令到服务器
    //功能模块：命令发送方法（发送Json）
    private void SendCommandToServer()
    {
        try
        {
            string json = JsonConvert.SerializeObject(command, settings);


            //定义报文主体字节数组
            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes(json);
            //定义报文头部字节数组
            byte[] frameLengthBuffer = new byte[4];
            //报文主体的数组的元素总数（长度）转义成数组，赋值给定义好的变量
            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
            //通过方法将两个数组拼接在一起
            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();


            mainSocket.Send(sendByte, sendByte.Length, 0);

            Debug.Log("发送的指令:" + json);
        }
        catch (Exception ex)
        {
            Debug.Log("命令发送出现问题：" + ex);
        }
    }
    #endregion






    //按钮：数据库获取JSON
    #region 数据库获取JSON
    private void FormSQLGetJson()
    {

    }
    #endregion

    #region 断开连接按钮事件
    private void DisconnectionPlcServer()
    {
        flag = false;

        if (mainSocket != null && mainSocket.Connected)
        {
            mainSocket.Shutdown(SocketShutdown.Both);
            mainSocket.Close();
            mainSocket.Dispose();
        }
    }
    #endregion



    #endregion

    #endregion



    /*
    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        System.Environment.Exit(0);
    }
    */
    

}