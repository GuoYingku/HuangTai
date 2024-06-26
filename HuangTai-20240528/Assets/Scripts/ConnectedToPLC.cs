using HuangtaiPowerPlantControlSystem;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;


public class ConnectedToPLC : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        Thread newThread = new Thread(Proccess2);       //启用线程  开始连接服务器读取json文件
        newThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }


    #region 变量定义

    SystemVariables systemVariables = new SystemVariables();//PLC1变量对象
    PLC2Variables plc2Variables = new PLC2Variables();//PLC2变量对象
    FullVariables fullVariables = new FullVariables();//PLC全体变量对象

    bool flag = false;
    bool flag2 = false;

    private DateTime startTime;
    IPAddress hostIP;
    IPEndPoint port;
    int point;
    Socket socketWatcher;
    //Socket subSocket;
    /*  变量定义
     *  startTime:程序启动时刻
     *  hostIP:用于存放服务器（本机）IP地址
     *  port:服务器的网络终结点，即IP:端口号
     *  point:存放用户输入的端口号
     *  socketWatcher:监听子系统的套接字
     *  subSocket:用于接收数据流的套接字---实现通信
     */
    #endregion

    private SystemCommand command = new SystemCommand();

    // 设置序列化选项
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented // 设置缩进格式
    };

    //变量转移方法（将PLC1和PLC2的变量移至一起）
    public static void CopyProperties(SystemVariables a, PLC2Variables b, FullVariables c)
    {
        Type aType = typeof(SystemVariables);
        Type bType = typeof(PLC2Variables);
        Type cType = typeof(FullVariables);

        PropertyInfo[] aProperties = aType.GetProperties();
        PropertyInfo[] bProperties = bType.GetProperties();
        PropertyInfo[] cProperties = cType.GetProperties();

        foreach (PropertyInfo aProp in aProperties)
        {
            PropertyInfo cProp = cProperties.FirstOrDefault(p => p.Name == aProp.Name && p.PropertyType == aProp.PropertyType);
            if (cProp != null)
            {
                cProp.SetValue(c, aProp.GetValue(a));
            }
        }

        foreach (PropertyInfo bProp in bProperties)
        {
            PropertyInfo cProp = cProperties.FirstOrDefault(p => p.Name == bProp.Name && p.PropertyType == bProp.PropertyType);
            if (cProp != null)
            {
                cProp.SetValue(c, bProp.GetValue(b));
            }
        }
    }

    //模块：网络通信
    #region 网络通信

    #region 套接字监听线程
    private void Proccess2()
    {
        hostIP = IPAddress.Parse("192.168.1.10");//远程驱动系统要监听的IP地址
        point = int.Parse("13000");//在这个端口号监听

        flag = true;//循环控制置ture

        try
        {
            port = new IPEndPoint(hostIP, point);//设置监听端口

            //创建监听用的套接字对象
            socketWatcher = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketWatcher.Bind(port);//绑定端口
            socketWatcher.Listen(10);//开始监听


            while (flag)
            {
                //通过线程调用Proceess3方法
                Socket subSocket = socketWatcher.Accept();

                Thread thread3 = new Thread(Proccess3);

                thread3.Start(subSocket);

                flag2 = true;

                Thread.Sleep(10);
            }
        }
        catch (Exception e)
        {
            Debug.Log("监听异常：" + e.Message);
            SocketExhandling();
        }
    }

    #endregion

    #region 套接字通信线程
    //未来跟多个子系统进行连接，肯定需要创建不止一个辅助线程，需要再更改架构
    private void Proccess3(object socket)
    {
        Socket subSocket = socket as Socket;

        if (subSocket.Connected)
        {
            Debug.Log("套接字通信建立成功");

            while (flag2)
            {
                try
                {
                    byte[] frameLengthBuffer = new byte[4]; // 根据定义，数据帧长度为4个字节
                    int frameLength;

                    //接收报文头部信息
                    subSocket.Receive(frameLengthBuffer, frameLengthBuffer.Length, 0);
                    //头部转义为整型
                    frameLength = BitConverter.ToInt32(frameLengthBuffer, 0);

                    //动态缓冲区大小
                    byte[] receiveBuffer = new byte[frameLength];

                    //接收报文主体
                    subSocket.Receive(receiveBuffer, frameLength, 0);
                    string strInfo = Encoding.BigEndianUnicode.GetString(receiveBuffer);



                    //报文发送结果还原和查看
                    string strInfo2 = frameLength.ToString();
                    string text = strInfo2 + "\n\r" + strInfo;

                    //Debug.Log("报分返回结果" + text);

                    ReceiveText(strInfo, subSocket);


                    Thread.Sleep(30);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.Interrupted)
                    {
                        Debug.Log("套接字通信被中断");
                        SocketExhandling();
                    }
                    else
                    {
                        Debug.Log("套接字通信异常：" + e.Message);
                        SocketExhandling();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("套接字通信异常：" + e.Message);
                    SocketExhandling();
                }
            }
        }
    }
    #endregion

    #region 对命令接收方法
    //功能模块
    private void ReceiveText(string text, Socket socket)
    {
        try
        {
            //将接收到的“命令”JSON 字符串反序列化为对象
            SystemCommand command2 = JsonConvert.DeserializeObject<SystemCommand>(text);


            command.QUERY_SYSTEM = command2.QUERY_SYSTEM;
            command.DATA_TYPE = command2.DATA_TYPE;
            command.QUERY_TYPE = command2.QUERY_TYPE;
            command.COMMAND_NAME = command2.COMMAND_NAME;
            command.COMMAND_TYPE = command2.COMMAND_TYPE;


            //Type type = systemVariables.GetType();
            //PropertyInfo[] properties = type.GetProperties();

            string json = JsonConvert.SerializeObject(fullVariables, settings);

            Debug.Log(json);  //拿到的数据在这里*****************************************************************************************************************************************************************************************

            //判断是否收到查询命令
            if (command.QUERY_TYPE == 1)
            {
                byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes(json);


                byte[] frameLengthBuffer = new byte[4];
                frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();


                //通过套接字发送JSON字符串给到其他子系统
                socket.Send(sendByte, sendByte.Length, 0);
            }
            else if (command.QUERY_TYPE == 2)
            {


                switch (command.COMMAND_NAME)
                {
                    case "SUPPLYPOWER_ON": // 命令名为 "SUPPLYPOWER_ON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes("SUPPLYPOWER_ON 1");
                            byte[] frameLengthBuffer = new byte[4];
                            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();
                            socket.Send(sendByte, sendByte.Length, 0);
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes("SUPPLYPOWER_ON 0");
                            byte[] frameLengthBuffer = new byte[4];
                            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();
                            socket.Send(sendByte, sendByte.Length, 0);
                        }
                        break;

                    case "SUPPLYPOWER_OFF": // 命令名为 "SUPPLYPOWER_OFF"
                        if (command.COMMAND_TYPE == 1)
                        {
                            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes("SUPPLYPOWER_OFF 1");
                            byte[] frameLengthBuffer = new byte[4];
                            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();
                            socket.Send(sendByte, sendByte.Length, 0);
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes("SUPPLYPOWER_OFF 0");
                            byte[] frameLengthBuffer = new byte[4];
                            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();
                            socket.Send(sendByte, sendByte.Length, 0);
                        }
                        break;

                    case "CONTROLPOWER_ON": // 命令名为 "CONTROLPOWER_ON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes("CONTROLPOWER_ON 1");
                            byte[] frameLengthBuffer = new byte[4];
                            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();
                            socket.Send(sendByte, sendByte.Length, 0);
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes("CONTROLPOWER_ON 0");
                            byte[] frameLengthBuffer = new byte[4];
                            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();
                            socket.Send(sendByte, sendByte.Length, 0);
                        }
                        break;

                    case "CONTROLPOWER_OFF": // 命令名为 "CONTROLPOWER_OFF"
                        if (command.COMMAND_TYPE == 1)
                        {
                            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes("CONTROLPOWER_OFF 1");
                            byte[] frameLengthBuffer = new byte[4];
                            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();
                            socket.Send(sendByte, sendByte.Length, 0);
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes("CONTROLPOWER_OFF 0");
                            byte[] frameLengthBuffer = new byte[4];
                            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();
                            socket.Send(sendByte, sendByte.Length, 0);
                        }
                        break;

                    case "LIGHTPOWER_ON": // 命令名为 "LIGHTPOWER_ON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.0", 0, "True");//内存地址DB414.DBX654.0
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.0", 0, "False");//内存地址DB414.DBX654.0
                        }
                        break;

                    case "LIGHTPOWER_OFF": // 命令名为 "LIGHTPOWER_OFF"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.1", 0, "True");//内存地址DB414.DBX654.1
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.1", 0, "False");//内存地址DB414.DBX654.1
                        }
                        break;

                    case "ROTATE_LEFT": // 命令名为 "ROTATE_LEFT"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.2", 0, "True");//内存地址DB414.DBX652.2
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.2", 0, "False");//内存地址DB414.DBX652.2
                        }
                        break;

                    case "ROTATE_RIGHT": // 命令名为 "REVERSE_LEFT"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.3", 0, "True");//内存地址DB414.DBX652.3
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.3", 0, "False");//内存地址DB414.DBX652.3
                        }
                        break;

                    case "ROTATE_STOP": // 命令名为 "ROTATE_STOP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.4", 0, "True");//内存地址DB414.DBX652.4
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.4", 0, "False");//内存地址DB414.DBX652.4
                        }
                        break;

                    case "ROTATE_MODE": // 命令名为 "ROTATE_MODE"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.5", 0, "True");//内存地址DB414.DBX654.5
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.5", 0, "False");//内存地址DB414.DBX654.5
                        }
                        break;

                    case "STACKING_START": // 命令名为 "STACKING_START"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.5", 0, "True");//内存地址DB414.DBX652.5
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.5", 0, "False");//内存地址DB414.DBX652.5
                        }
                        break;

                    case "STACKING_STOP": // 命令名为 "STACKING_STOP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX656.0", 0, "True");//内存地址DB414.DBX656.0
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX656.0", 0, "False");//内存地址DB414.DBX656.0
                        }
                        break;

                    case "REVERSE_LEFT": // 命令名为 "REVERSE_LEFT"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.7", 0, "True");//内存地址DB414.DBX652.7
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.7", 0, "False");//内存地址DB414.DBX652.7
                        }
                        break;

                    case "REVERSE_RIGHT": // 命令名为 "REVERSE_RIGHT"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.0", 0, "True");//内存地址DB414.DBX653.0
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.0", 0, "False");//内存地址DB414.DBX653.0
                        }
                        break;

                    case "REVERSE_STOP": // 命令名为 "REVERSE_STOP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.1", 0, "True");//内存地址DB414.DBX653.1
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.1", 0, "False");//内存地址DB414.DBX653.1
                        }
                        break;

                    case "REVERSE_MODE": // 命令名为 "REVERSE_MODE"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.2", 0, "True");//内存地址DB414.DBX653.2
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.2", 0, "False");//内存地址DB414.DBX653.2
                        }
                        break;

                    case "ELEVATE_UP": // 命令名为 "ELEVATE_UP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.3", 0, "True");//内存地址DB414.DBX653.3
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.3", 0, "False");//内存地址DB414.DBX653.3
                        }
                        break;

                    case "ELEVATE_DOWN": // 命令名为 "ELEVATE_DOWN"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.4", 0, "True");//内存地址DB414.DBX653.4
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.4", 0, "False");//内存地址DB414.DBX653.4
                        }
                        break;

                    case "ELEVATE_STOP": // 命令名为 "ELEVATE_STOP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.5", 0, "True");//内存地址DB414.DBX653.5
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.5", 0, "False");//内存地址DB414.DBX653.5
                        }
                        break;

                    case "ELEVATE_MODE": // 命令名为 "ELEVATE_MODE"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.6", 0, "True");//内存地址DB414.DBX653.6
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.6", 0, "False");//内存地址DB414.DBX653.6
                        }
                        break;

                    case "SCRAPER_CONTROL": // 命令名为 "SCRAPER_CONTROL"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.7", 0, "True");//内存地址DB414.DBX653.7
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.7", 0, "False");//内存地址DB414.DBX653.7
                        }
                        break;

                    case "SCRAPER_STOP_BUTTON": // 命令名为 "SCRAPER_STOP_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX655.7", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX655.7", 0, "False");
                        }
                        break;

                    case "STARTUP_ALARM": // 命令名为 "STARTUP_ALARM"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.6", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.6", 0, "False");
                        }
                        break;

                    case "FAULT_RESET": // 命令名为 "FAULT_RESET"
                                        // 属于等待PLC进行判断的变量，不需要使其变为False，而是等待PLC自行将其复位为False
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.2", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.2", 0, "False");
                        }
                        break;

                    case "EMERGENCY_STOP": // 命令名为 "EMERGENCY_STOP"
                                           // 属于等待PLC进行判断的变量，不需要使其变为False，而是等待PLC自行将其复位为False
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.3", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.3", 0, "False");
                        }
                        break;

                    case "BYPASS_BUTTON": // 命令名为 "BYPASS_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.4", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.4", 0, "False");
                        }
                        break;

                    case "SYSTEM_UNLOCK": // 命令名为 "SYSTEM_UNLOCK"
                                          //按下置位，仅接收1
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.0", 0, "True");
                        }
                        break;

                    case "SYSTEM_LOCK": // 命令名为 "SYSTEM_LOCK"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.1", 0, "True");
                        }
                        break;

                    case "STACKING_MODE_HM": // 命令名为 "STACKING_MODE_HM"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB404.DBX108.0", 0, "True");
                        }
                        break;

                    case "STACKING_MODE_AUTO": // 命令名为 "STACKING_MODE_AUTO"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB404.DBX108.1", 0, "True");
                        }
                        break;

                    case "PICKUPING_MODE_HM": // 命令名为 "PICKUPING_MODE_HM"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB404.DBX109.6", 0, "True");
                        }
                        break;

                    case "PICKUPING_MODE_AUTO": // 命令名为 "PICKUPING_MODE_AUTO"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB404.DBX109.7", 0, "True");
                        }
                        break;

                    case "STACKING_MID_LUB_BUTTON": // 命令名为 "STACKING_MID_LUB_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB401.DBX5.7", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB401.DBX5.7", 0, "False");
                        }
                        break;

                    case "STACKING_MID_STOP_BUTTON": // 命令名为 "STACKING_MID_STOP_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB401.DBX7.0", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB401.DBX7.0", 0, "False");
                        }
                        break;

                    case "STACKING_UP_LUB_BUTTON": // 命令名为 "STACKING_UP_LUB_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB401.DBX6.0", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB401.DBX6.0", 0, "False");
                        }
                        break;

                    case "STACKING_UP_STOP_BUTTON": // 命令名为 "STACKING_UP_STOP_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB401.DBX7.1", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB401.DBX7.1", 0, "False");
                        }
                        break;

                    //case "BYPASS_BUTTON": // 命令名为 "BYPASS_BUTTON"
                    //    if (command.COMMAND_TYPE == 1)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "True");
                    //    }
                    //    else if (command.COMMAND_TYPE == 0)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "False");
                    //    }
                    //    break;

                    //case "BYPASS_BUTTON": // 命令名为 "BYPASS_BUTTON"
                    //    if (command.COMMAND_TYPE == 1)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "True");
                    //    }
                    //    else if (command.COMMAND_TYPE == 0)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "False");
                    //    }
                    //    break;

                    //case "BYPASS_BUTTON": // 命令名为 "BYPASS_BUTTON"
                    //    if (command.COMMAND_TYPE == 1)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "True");
                    //    }
                    //    else if (command.COMMAND_TYPE == 0)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "False");
                    //    }
                    //    break;

                    default:
                        // 处理其他值或未赋值的情况
                        break;
                }
            }
            else if (command.QUERY_TYPE == 3)
            {
                /*
                //先初始化二维数组
                byte[,] imageArray = new byte[3000, 3000];
                for (int i = 0; i < imageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < imageArray.GetLength(1); j++)
                    {
                        imageArray[i, j] = 232;
                    }
                }

                //将二维数组转换成字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < imageArray.GetLength(0); i++)
                {
                    //先从左到右处理第0行的元素，再一行一行往下处理
                    for (int j = 0; j < imageArray.GetLength(1); j++)
                    {
                        sb.Append(imageArray[i, j].ToString());
                        sb.Append(" "); //可以根据需要添加分隔符
                    }

                    sb.AppendLine(); //可以根据需要添加换行符
                }
                string arrayAsString = sb.ToString();

                MessageBox.Show(arrayAsString);

                byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes(arrayAsString);

                byte[] frameLengthBuffer = new byte[4];
                frameLengthBuffer = BitConverter.GetBytes(arrayAsString.Length);

                MessageBox.Show(arrayAsString.Length.ToString());

                byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();

                MessageBox.Show(sendByte.Length.ToString());

                //通过套接字发送JSON字符串给到其他子系统
                socket.Send(sendByte, sendByte.Length, 0);
                */
            }

        }
        catch (Exception e)
        {
            // 解析失败，表示数据不是 JSON 格式
            Debug.Log("数据接收出现问题：ReceiveText方法" + e);
            Thread.CurrentThread.Abort();
        }
    }
    #endregion

    #region 套接字异常处理
    private void SocketExhandling()
    {
        //if (socketWatcher != null && socketWatcher.Connected)
        //{
        //    socketWatcher.Shutdown(SocketShutdown.Both);
        //    socketWatcher.Close();
        //    socketWatcher.Dispose();
        //}

        //if (subSocket != null && subSocket.Connected)
        //{
        //    subSocket.Shutdown(SocketShutdown.Both);
        //    subSocket.Close();
        //    subSocket.Dispose();
        //}

        //flag = false;
        flag2 = false;// 循环控制置false，停止不断接收数据
    }
    #endregion

    #endregion



}
