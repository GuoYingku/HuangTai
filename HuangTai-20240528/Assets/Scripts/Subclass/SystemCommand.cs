using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HuangtaiPowerPlantControlSystem
{
    [DataContract]
    //远程驱动命令类
    public class SystemCommand
    {
        [DataMember]
        public string QUERY_SYSTEM { get; set; }
        //正在控制/查询的子系统的ID
        [DataMember]
        public int DATA_TYPE { get; set; }
        //数据类型：1-流量，2-运动参数，3-三维料堆，4-安全信息，5-任务规划信息，6-远程驱动系统交互
        [DataMember]
        public int QUERY_TYPE { get; set; }
        //命令类型：0-提供给本地服务器端以初始化的数据，1-PLC的一帧数据，2-命令模式
        [DataMember]
        public string COMMAND_NAME { get; set; }
        //命令名：该名字即为“基本指令表.xlsx”中的“英文指令名”
        [DataMember]
        public int COMMAND_TYPE { get; set; }
        //命令内容：0-False，1-True
        [DataMember]
        public float COMMAND_DATA { get; set; }
        //数据内容：度数，深度
    }
}