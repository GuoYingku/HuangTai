using System.Runtime.Serialization;

[DataContract]
//Զ������������
public class ServerCommand
{
    [DataMember]
    public string QUERY_SYSTEM { get; set; }
    //���ڿ���/��ѯ����ϵͳ��ID
    [DataMember]
    public int DATA_TYPE { get; set; }
    //�������ͣ�1-������2-�˶�������3-��ά�϶ѣ�4-��ȫ��Ϣ��5-����滮��Ϣ��6-Զ������ϵͳ����
    [DataMember]
    public int QUERY_TYPE { get; set; }
    [DataMember]
    public string COMMAND_NAME { get; set; }
    //�������������ּ�Ϊ������ָ���.xlsx���еġ�Ӣ��ָ������
    [DataMember]
    public int COMMAND_TYPE { get; set; }
    //�������ݣ�0-False��1-True
    [DataMember]
    public float COMMAND_DATA { get; set; }
    //�������ݣ����������
}
public static class ServerCommandDataType
{
    public const int FLOW = 1;
    public const int MOTION = 2;
    public const int SCAN = 3;
    public const int SECURITY = 4;
    public const int TASK = 5;
    public const int REMOTE = 6;
}