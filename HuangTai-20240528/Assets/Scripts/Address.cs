using System.Net;

public static class Address
{

    //public static string serviceIP = "192.168.1.180";//��̨ ���ݿ� ip 192.168.1.180
    //public static string serviceTaoIP = "192.168.1.201";//��̨ Զ������ ip 192.168.1.180
    //public static string serviceYuanIP = "192.168.1.180";//��̨ ��άɨ�� ip 192.168.1.180

    //public static string serviceIP = "192.168.1.8";//���� ���ݿ�
    public static string serviceIP = "localhost";//���� ���ݿ�
    public static string serviceTaoIP = "192.168.1.40";//���� Զ������
    public static string serviceYuanIP = "192.168.1.5";//���� ��άɨ��

    public static string taoUrl = "ws://" + serviceTaoIP + ":" + "11000";
    public static string yuanUrl = "ws://" + serviceYuanIP + ":" + "12000";
}
