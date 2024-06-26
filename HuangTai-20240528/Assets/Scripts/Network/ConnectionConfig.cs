using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "connection_config", menuName = "ConnectionConfig")]
public class ConnectionConfig : ScriptableObject
{
    public List<ConnectionInfo> configs;

}