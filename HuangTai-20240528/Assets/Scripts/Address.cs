using System.Net;

public static class Address
{

    //public static string serviceIP = "192.168.1.180";//黄台 数据库 ip 192.168.1.180
    //public static string serviceTaoIP = "192.168.1.201";//黄台 远程驱动 ip 192.168.1.180
    //public static string serviceYuanIP = "192.168.1.180";//黄台 三维扫描 ip 192.168.1.180

    //public static string serviceIP = "192.168.1.8";//本地 数据库
    public static string serviceIP = "localhost";//本地 数据库
    public static string serviceTaoIP = "192.168.1.40";//本地 远程驱动
    public static string serviceYuanIP = "192.168.1.5";//本地 三维扫描

    public static string taoUrl = "ws://" + serviceTaoIP + ":" + "11000";
    public static string yuanUrl = "ws://" + serviceYuanIP + ":" + "12000";
}
