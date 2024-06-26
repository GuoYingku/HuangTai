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
        Thread newThread = new Thread(Proccess2);       //�����߳�  ��ʼ���ӷ�������ȡjson�ļ�
        newThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }


    #region ��������

    SystemVariables systemVariables = new SystemVariables();//PLC1��������
    PLC2Variables plc2Variables = new PLC2Variables();//PLC2��������
    FullVariables fullVariables = new FullVariables();//PLCȫ���������

    bool flag = false;
    bool flag2 = false;

    private DateTime startTime;
    IPAddress hostIP;
    IPEndPoint port;
    int point;
    Socket socketWatcher;
    //Socket subSocket;
    /*  ��������
     *  startTime:��������ʱ��
     *  hostIP:���ڴ�ŷ�������������IP��ַ
     *  port:�������������ս�㣬��IP:�˿ں�
     *  point:����û�����Ķ˿ں�
     *  socketWatcher:������ϵͳ���׽���
     *  subSocket:���ڽ������������׽���---ʵ��ͨ��
     */
    #endregion

    private SystemCommand command = new SystemCommand();

    // �������л�ѡ��
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented // ����������ʽ
    };

    //����ת�Ʒ�������PLC1��PLC2�ı�������һ��
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

    //ģ�飺����ͨ��
    #region ����ͨ��

    #region �׽��ּ����߳�
    private void Proccess2()
    {
        hostIP = IPAddress.Parse("192.168.1.10");//Զ������ϵͳҪ������IP��ַ
        point = int.Parse("13000");//������˿ںż���

        flag = true;//ѭ��������ture

        try
        {
            port = new IPEndPoint(hostIP, point);//���ü����˿�

            //���������õ��׽��ֶ���
            socketWatcher = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketWatcher.Bind(port);//�󶨶˿�
            socketWatcher.Listen(10);//��ʼ����


            while (flag)
            {
                //ͨ���̵߳���Proceess3����
                Socket subSocket = socketWatcher.Accept();

                Thread thread3 = new Thread(Proccess3);

                thread3.Start(subSocket);

                flag2 = true;

                Thread.Sleep(10);
            }
        }
        catch (Exception e)
        {
            Debug.Log("�����쳣��" + e.Message);
            SocketExhandling();
        }
    }

    #endregion

    #region �׽���ͨ���߳�
    //δ���������ϵͳ�������ӣ��϶���Ҫ������ֹһ�������̣߳���Ҫ�ٸ��ļܹ�
    private void Proccess3(object socket)
    {
        Socket subSocket = socket as Socket;

        if (subSocket.Connected)
        {
            Debug.Log("�׽���ͨ�Ž����ɹ�");

            while (flag2)
            {
                try
                {
                    byte[] frameLengthBuffer = new byte[4]; // ���ݶ��壬����֡����Ϊ4���ֽ�
                    int frameLength;

                    //���ձ���ͷ����Ϣ
                    subSocket.Receive(frameLengthBuffer, frameLengthBuffer.Length, 0);
                    //ͷ��ת��Ϊ����
                    frameLength = BitConverter.ToInt32(frameLengthBuffer, 0);

                    //��̬��������С
                    byte[] receiveBuffer = new byte[frameLength];

                    //���ձ�������
                    subSocket.Receive(receiveBuffer, frameLength, 0);
                    string strInfo = Encoding.BigEndianUnicode.GetString(receiveBuffer);



                    //���ķ��ͽ����ԭ�Ͳ鿴
                    string strInfo2 = frameLength.ToString();
                    string text = strInfo2 + "\n\r" + strInfo;

                    //Debug.Log("���ַ��ؽ��" + text);

                    ReceiveText(strInfo, subSocket);


                    Thread.Sleep(30);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.Interrupted)
                    {
                        Debug.Log("�׽���ͨ�ű��ж�");
                        SocketExhandling();
                    }
                    else
                    {
                        Debug.Log("�׽���ͨ���쳣��" + e.Message);
                        SocketExhandling();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("�׽���ͨ���쳣��" + e.Message);
                    SocketExhandling();
                }
            }
        }
    }
    #endregion

    #region ��������շ���
    //����ģ��
    private void ReceiveText(string text, Socket socket)
    {
        try
        {
            //�����յ��ġ����JSON �ַ��������л�Ϊ����
            SystemCommand command2 = JsonConvert.DeserializeObject<SystemCommand>(text);


            command.QUERY_SYSTEM = command2.QUERY_SYSTEM;
            command.DATA_TYPE = command2.DATA_TYPE;
            command.QUERY_TYPE = command2.QUERY_TYPE;
            command.COMMAND_NAME = command2.COMMAND_NAME;
            command.COMMAND_TYPE = command2.COMMAND_TYPE;


            //Type type = systemVariables.GetType();
            //PropertyInfo[] properties = type.GetProperties();

            string json = JsonConvert.SerializeObject(fullVariables, settings);

            Debug.Log(json);  //�õ�������������*****************************************************************************************************************************************************************************************

            //�ж��Ƿ��յ���ѯ����
            if (command.QUERY_TYPE == 1)
            {
                byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes(json);


                byte[] frameLengthBuffer = new byte[4];
                frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
                byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();


                //ͨ���׽��ַ���JSON�ַ�������������ϵͳ
                socket.Send(sendByte, sendByte.Length, 0);
            }
            else if (command.QUERY_TYPE == 2)
            {


                switch (command.COMMAND_NAME)
                {
                    case "SUPPLYPOWER_ON": // ������Ϊ "SUPPLYPOWER_ON"
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

                    case "SUPPLYPOWER_OFF": // ������Ϊ "SUPPLYPOWER_OFF"
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

                    case "CONTROLPOWER_ON": // ������Ϊ "CONTROLPOWER_ON"
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

                    case "CONTROLPOWER_OFF": // ������Ϊ "CONTROLPOWER_OFF"
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

                    case "LIGHTPOWER_ON": // ������Ϊ "LIGHTPOWER_ON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.0", 0, "True");//�ڴ��ַDB414.DBX654.0
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.0", 0, "False");//�ڴ��ַDB414.DBX654.0
                        }
                        break;

                    case "LIGHTPOWER_OFF": // ������Ϊ "LIGHTPOWER_OFF"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.1", 0, "True");//�ڴ��ַDB414.DBX654.1
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.1", 0, "False");//�ڴ��ַDB414.DBX654.1
                        }
                        break;

                    case "ROTATE_LEFT": // ������Ϊ "ROTATE_LEFT"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.2", 0, "True");//�ڴ��ַDB414.DBX652.2
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.2", 0, "False");//�ڴ��ַDB414.DBX652.2
                        }
                        break;

                    case "ROTATE_RIGHT": // ������Ϊ "REVERSE_LEFT"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.3", 0, "True");//�ڴ��ַDB414.DBX652.3
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.3", 0, "False");//�ڴ��ַDB414.DBX652.3
                        }
                        break;

                    case "ROTATE_STOP": // ������Ϊ "ROTATE_STOP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.4", 0, "True");//�ڴ��ַDB414.DBX652.4
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.4", 0, "False");//�ڴ��ַDB414.DBX652.4
                        }
                        break;

                    case "ROTATE_MODE": // ������Ϊ "ROTATE_MODE"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.5", 0, "True");//�ڴ��ַDB414.DBX654.5
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.5", 0, "False");//�ڴ��ַDB414.DBX654.5
                        }
                        break;

                    case "STACKING_START": // ������Ϊ "STACKING_START"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.5", 0, "True");//�ڴ��ַDB414.DBX652.5
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.5", 0, "False");//�ڴ��ַDB414.DBX652.5
                        }
                        break;

                    case "STACKING_STOP": // ������Ϊ "STACKING_STOP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX656.0", 0, "True");//�ڴ��ַDB414.DBX656.0
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX656.0", 0, "False");//�ڴ��ַDB414.DBX656.0
                        }
                        break;

                    case "REVERSE_LEFT": // ������Ϊ "REVERSE_LEFT"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.7", 0, "True");//�ڴ��ַDB414.DBX652.7
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.7", 0, "False");//�ڴ��ַDB414.DBX652.7
                        }
                        break;

                    case "REVERSE_RIGHT": // ������Ϊ "REVERSE_RIGHT"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.0", 0, "True");//�ڴ��ַDB414.DBX653.0
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.0", 0, "False");//�ڴ��ַDB414.DBX653.0
                        }
                        break;

                    case "REVERSE_STOP": // ������Ϊ "REVERSE_STOP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.1", 0, "True");//�ڴ��ַDB414.DBX653.1
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.1", 0, "False");//�ڴ��ַDB414.DBX653.1
                        }
                        break;

                    case "REVERSE_MODE": // ������Ϊ "REVERSE_MODE"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.2", 0, "True");//�ڴ��ַDB414.DBX653.2
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.2", 0, "False");//�ڴ��ַDB414.DBX653.2
                        }
                        break;

                    case "ELEVATE_UP": // ������Ϊ "ELEVATE_UP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.3", 0, "True");//�ڴ��ַDB414.DBX653.3
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.3", 0, "False");//�ڴ��ַDB414.DBX653.3
                        }
                        break;

                    case "ELEVATE_DOWN": // ������Ϊ "ELEVATE_DOWN"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.4", 0, "True");//�ڴ��ַDB414.DBX653.4
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.4", 0, "False");//�ڴ��ַDB414.DBX653.4
                        }
                        break;

                    case "ELEVATE_STOP": // ������Ϊ "ELEVATE_STOP"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.5", 0, "True");//�ڴ��ַDB414.DBX653.5
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.5", 0, "False");//�ڴ��ַDB414.DBX653.5
                        }
                        break;

                    case "ELEVATE_MODE": // ������Ϊ "ELEVATE_MODE"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.6", 0, "True");//�ڴ��ַDB414.DBX653.6
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.6", 0, "False");//�ڴ��ַDB414.DBX653.6
                        }
                        break;

                    case "SCRAPER_CONTROL": // ������Ϊ "SCRAPER_CONTROL"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.7", 0, "True");//�ڴ��ַDB414.DBX653.7
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX653.7", 0, "False");//�ڴ��ַDB414.DBX653.7
                        }
                        break;

                    case "SCRAPER_STOP_BUTTON": // ������Ϊ "SCRAPER_STOP_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX655.7", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX655.7", 0, "False");
                        }
                        break;

                    case "STARTUP_ALARM": // ������Ϊ "STARTUP_ALARM"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.6", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.6", 0, "False");
                        }
                        break;

                    case "FAULT_RESET": // ������Ϊ "FAULT_RESET"
                                        // ���ڵȴ�PLC�����жϵı���������Ҫʹ���ΪFalse�����ǵȴ�PLC���н��临λΪFalse
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.2", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.2", 0, "False");
                        }
                        break;

                    case "EMERGENCY_STOP": // ������Ϊ "EMERGENCY_STOP"
                                           // ���ڵȴ�PLC�����жϵı���������Ҫʹ���ΪFalse�����ǵȴ�PLC���н��临λΪFalse
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.3", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.3", 0, "False");
                        }
                        break;

                    case "BYPASS_BUTTON": // ������Ϊ "BYPASS_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.4", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB414.DBX654.4", 0, "False");
                        }
                        break;

                    case "SYSTEM_UNLOCK": // ������Ϊ "SYSTEM_UNLOCK"
                                          //������λ��������1
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.0", 0, "True");
                        }
                        break;

                    case "SYSTEM_LOCK": // ������Ϊ "SYSTEM_LOCK"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB414.DBX652.1", 0, "True");
                        }
                        break;

                    case "STACKING_MODE_HM": // ������Ϊ "STACKING_MODE_HM"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB404.DBX108.0", 0, "True");
                        }
                        break;

                    case "STACKING_MODE_AUTO": // ������Ϊ "STACKING_MODE_AUTO"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB404.DBX108.1", 0, "True");
                        }
                        break;

                    case "PICKUPING_MODE_HM": // ������Ϊ "PICKUPING_MODE_HM"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB404.DBX109.6", 0, "True");
                        }
                        break;

                    case "PICKUPING_MODE_AUTO": // ������Ϊ "PICKUPING_MODE_AUTO"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB404.DBX109.7", 0, "True");
                        }
                        break;

                    case "STACKING_MID_LUB_BUTTON": // ������Ϊ "STACKING_MID_LUB_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB401.DBX5.7", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB401.DBX5.7", 0, "False");
                        }
                        break;

                    case "STACKING_MID_STOP_BUTTON": // ������Ϊ "STACKING_MID_STOP_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB401.DBX7.0", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB401.DBX7.0", 0, "False");
                        }
                        break;

                    case "STACKING_UP_LUB_BUTTON": // ������Ϊ "STACKING_UP_LUB_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB401.DBX6.0", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB401.DBX6.0", 0, "False");
                        }
                        break;

                    case "STACKING_UP_STOP_BUTTON": // ������Ϊ "STACKING_UP_STOP_BUTTON"
                        if (command.COMMAND_TYPE == 1)
                        {
                            //UC_page12.PLC2Write("DB401.DBX7.1", 0, "True");
                        }
                        else if (command.COMMAND_TYPE == 0)
                        {
                            //UC_page12.PLC2Write("DB401.DBX7.1", 0, "False");
                        }
                        break;

                    //case "BYPASS_BUTTON": // ������Ϊ "BYPASS_BUTTON"
                    //    if (command.COMMAND_TYPE == 1)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "True");
                    //    }
                    //    else if (command.COMMAND_TYPE == 0)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "False");
                    //    }
                    //    break;

                    //case "BYPASS_BUTTON": // ������Ϊ "BYPASS_BUTTON"
                    //    if (command.COMMAND_TYPE == 1)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "True");
                    //    }
                    //    else if (command.COMMAND_TYPE == 0)
                    //    {
                    //        UC_page12.PLC2Write("DB414.DBX654.4", 0, "False");
                    //    }
                    //    break;

                    //case "BYPASS_BUTTON": // ������Ϊ "BYPASS_BUTTON"
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
                        // ��������ֵ��δ��ֵ�����
                        break;
                }
            }
            else if (command.QUERY_TYPE == 3)
            {
                /*
                //�ȳ�ʼ����ά����
                byte[,] imageArray = new byte[3000, 3000];
                for (int i = 0; i < imageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < imageArray.GetLength(1); j++)
                    {
                        imageArray[i, j] = 232;
                    }
                }

                //����ά����ת�����ַ���
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < imageArray.GetLength(0); i++)
                {
                    //�ȴ����Ҵ����0�е�Ԫ�أ���һ��һ�����´���
                    for (int j = 0; j < imageArray.GetLength(1); j++)
                    {
                        sb.Append(imageArray[i, j].ToString());
                        sb.Append(" "); //���Ը�����Ҫ��ӷָ���
                    }

                    sb.AppendLine(); //���Ը�����Ҫ��ӻ��з�
                }
                string arrayAsString = sb.ToString();

                MessageBox.Show(arrayAsString);

                byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes(arrayAsString);

                byte[] frameLengthBuffer = new byte[4];
                frameLengthBuffer = BitConverter.GetBytes(arrayAsString.Length);

                MessageBox.Show(arrayAsString.Length.ToString());

                byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();

                MessageBox.Show(sendByte.Length.ToString());

                //ͨ���׽��ַ���JSON�ַ�������������ϵͳ
                socket.Send(sendByte, sendByte.Length, 0);
                */
            }

        }
        catch (Exception e)
        {
            // ����ʧ�ܣ���ʾ���ݲ��� JSON ��ʽ
            Debug.Log("���ݽ��ճ������⣺ReceiveText����" + e);
            Thread.CurrentThread.Abort();
        }
    }
    #endregion

    #region �׽����쳣����
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
        flag2 = false;// ѭ��������false��ֹͣ���Ͻ�������
    }
    #endregion

    #endregion



}
