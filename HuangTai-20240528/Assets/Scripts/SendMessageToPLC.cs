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

        command.QUERY_SYSTEM = "TEST";//��ϵͳID
    }

    */




    #region ��������
    /*  ��������
     *  hostIP:���ڴ�ŷ�������������IP��ַ
     *  port:�������������ս�㣬��IP:�˿ں�
     *  point:����û�����Ķ˿ں�
     *  mainSocket:������Ҫ������������׽���
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

    #region ʵ��������
    //ʵ�����������Ժ��б�����͵Ļ����������Ӧ���ൽ��������У������ǰѶ��ֶ�ȡ�ϻ���Ϊ������
    private ServerCommand command = new ServerCommand();

    // �������л�ѡ��
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented // ����������ʽ
    };
    #endregion



    //ģ�飺����ͨ��
    #region ����ͨ��

    #region �������Ӱ�ť�¼�
    private void ConnectPlcServer()
    {
        hostIP = IPAddress.Parse("192.168.1.10");
        point = int.Parse("13000");

        flag = true;
        flag2 = true;

        try
        {
            port = new IPEndPoint(hostIP, point);

            //ʵ����Socket����
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSocket.Connect(port);

            Debug.Log("����������Զ��������...\r\n");
            //Thread thread = new Thread(new ThreadStart(Proccess));
            //thread.Start();
            Proccess();
        }
        catch (Exception ey)
        {
            Debug.Log("������û�п���\r\n" + ey.Message);
        }




    }
    #endregion

    #region ����ͨ�Ž���
    private void Proccess()
    {
        /*
        while (flag)
        {
            if (mainSocket.Connected)
            {
                if (flag2 == true)
                {
                    //this.Invoke(new SetTextCallback(PrintText), new object[] { "��������Զ��������\r\n" });
                    Debug.Log("��������Զ��������\r\n");
                    flag2 = false;
                }
                Debug.Log("���Ի�ȡJson\r\n");
            }
        }
        */

        if (mainSocket.Connected)
        {
            if (flag2 == true)
            {
                //this.Invoke(new SetTextCallback(PrintText), new object[] { "��������Զ��������\r\n" });
                Debug.Log("��������Զ��������\r\n");
                flag2 = false;
            }
            Debug.Log("���Է���ָ���ȡJson\r\n");
        }
        SendCommand();
    }
    #endregion

    private void GetJson()//�ӷ�������ȡJson����
    {
        if (mainSocket.Connected)
        {
            Debug.Log("��ʼ��ȡJson\r\n");
            byte[] frameLengthBuffer = new byte[4]; // ���ݶ��壬����֡����Ϊ4���ֽ�
            int frameLength = 0;

            mainSocket.Receive(frameLengthBuffer, frameLengthBuffer.Length, 0);
            frameLength = BitConverter.ToInt32(frameLengthBuffer, 0);


            byte[] receiveByte = new byte[frameLength];


            mainSocket.Receive(receiveByte, receiveByte.Length, 0);
            string strInfo = Encoding.BigEndianUnicode.GetString(receiveByte);


            string strInfo2 = frameLength.ToString();
            string jsontext = strInfo;

            fullVariables = JsonConvert.DeserializeObject<FullVariables>(jsontext);    //�� Newtonsoft.Json ���� JSON ����

            //this.Invoke(new SetTextCallback(GetJSON), new object[] { text });


            //Thread.Sleep(10);
        }
    }


    //����ָ���������ȡJSON
    #region �׽���ͨ�Ż�ȡJSON
    public void SendCommand()
    {
        if (mainSocket.Connected)
        {
            int type1 = int.Parse("6");//��������
            int type2 = int.Parse("1");//��������
            string type3 = "";//������
            int type4 = int.Parse("1");//��������

            //��ѯPLC��һ֡����
            SetCommand(type1, type2, type3, type4);

            SendCommandToServer();

            GetJson();
        }
        else
        {
            Debug.Log("�����������쳣");
        }
    }



    //��������������
    #region ��������
    private void SetCommand(int TYPE1, int TYPE2, string TYPE3, int TYPE4)
    {
        command.QUERY_SYSTEM = "MC";
        command.DATA_TYPE = TYPE1;
        command.QUERY_TYPE = TYPE2;
        command.COMMAND_NAME = TYPE3;
        command.COMMAND_TYPE = TYPE4;
    }
    #endregion



    #region �������������
    //����ģ�飺����ͷ���������Json��
    private void SendCommandToServer()
    {
        try
        {
            string json = JsonConvert.SerializeObject(command, settings);


            //���屨�������ֽ�����
            byte[] sendByte0 = Encoding.BigEndianUnicode.GetBytes(json);
            //���屨��ͷ���ֽ�����
            byte[] frameLengthBuffer = new byte[4];
            //��������������Ԫ�����������ȣ�ת������飬��ֵ������õı���
            frameLengthBuffer = BitConverter.GetBytes(sendByte0.Length);
            //ͨ����������������ƴ����һ��
            byte[] sendByte = frameLengthBuffer.Concat(sendByte0).ToArray();


            mainSocket.Send(sendByte, sendByte.Length, 0);

            Debug.Log("���͵�ָ��:" + json);
        }
        catch (Exception ex)
        {
            Debug.Log("����ͳ������⣺" + ex);
        }
    }
    #endregion






    //��ť�����ݿ��ȡJSON
    #region ���ݿ��ȡJSON
    private void FormSQLGetJson()
    {

    }
    #endregion

    #region �Ͽ����Ӱ�ť�¼�
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