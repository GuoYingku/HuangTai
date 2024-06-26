using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Utility.DesignPatterns;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Reflection;

public partial class NetworkSystem : ISingleton<NetworkSystem>, IStartSystem, IExitSystem
{

    public delegate void ConnectedCallback(Socket socket);
    public delegate void ResponseCallback(string json);

    public static NetworkSystem Instance
    {
        get => ISingleton<NetworkSystem>.Instance;
    }

    private Dictionary<TargetSystem, Socket> _socketDict = new Dictionary<TargetSystem, Socket>();
    private Dictionary<TargetSystem, ResponseCallback> _responseCallbacks = new Dictionary<TargetSystem, ResponseCallback>();
    private int _totPreparations = 0;
    private int _connectedConnection = 0;

    private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
    };

    public void Init()
    {
        foreach (var type in Utility.Utility.GetAllConcreteSubclasses(typeof(IConnection)))
        {
            IConnection connection;
            PropertyInfo instanceProperty;
            if ((instanceProperty = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)) != null)
            {
                connection = (IConnection)instanceProperty.GetValue(null);
            }
            else
            {
                connection = Activator.CreateInstance(type) as IConnection;
            }
            TargetSystem targetSystem = (TargetSystem)type.GetProperty("TargetSystem").GetValue(connection);
            AddResponseCallback(targetSystem, (ResponseCallback)Delegate.CreateDelegate(typeof(ResponseCallback), null, type.GetMethod("OnReceive")));
        }
    }

    public void StartFunc()
    {
        _ = Connect();
    }

    public void ExitFunc()
    {
        Disconnect();
    }

    private async UniTask Connect()
    {
        UISystem.Instance.SetActive(UIID.Loading, true);
        //await ConnectCoroutine();//跨域访问小陶 出错地点 -ljz
        //ScanConnection.Instance.CoalHeapStateRequest();
        UISystem.Instance.SetActive(UIID.Loading, false);
        UISystem.Instance.SetActive(UIID.Login, true);
    }

    private void Disconnect()
    {
        foreach (Socket socket in _socketDict.Values)
        {
            CloseSocket(socket);
        }
    }

    private IEnumerator ConnectCoroutine()
    {
        ConnectionConfig connectionConfig = ConstStr.CONNECTION_CONFIG_TABLE.LoadAssetAtAddress<ConnectionConfig>();
        _totPreparations = connectionConfig.configs.Count;
        for (int i = 0; i < connectionConfig.configs.Count; ++i)
        {
            ConnectionInfo info = connectionConfig.configs[i];
            new Thread(() =>
            {
                ConnectionThread(info, null);
            }).Start();
        }
        while (_connectedConnection < _totPreparations)
        {
            yield return null;
        }
    }

    private void ConnectionThread(ConnectionInfo info,Action onConnected)
    {
        if (_socketDict.TryGetValue(info.targetSystem, out Socket existingSocket))
        {
            CloseSocket(existingSocket);
            _socketDict.Remove(info.targetSystem);
        }
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(info.host, info.port);
            Interlocked.Increment(ref _connectedConnection);
            Subject.Instance.Notify(ConstStr.LOADING_PROGRESS_SUBJECT, _connectedConnection * 1f / _totPreparations);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        _socketDict[info.targetSystem] = socket;
        onConnected?.Invoke();

        try
        {
            while (socket.Connected)
            {
                if (socket.Available > 0)
                {
                    byte[] frameLengthBuffer = new byte[4];
                    int frameLength;

                    socket.Receive(frameLengthBuffer, frameLengthBuffer.Length, 0);
                    frameLength = BitConverter.ToInt32(frameLengthBuffer, 0);

                    int receivedByteCount = 0;
                    byte[] receiveByte = new byte[frameLength];
                    while(receivedByteCount<frameLength)
                    {
                        int cntByteCount = Mathf.Min(socket.Available, frameLength - receivedByteCount);
                        receivedByteCount += socket.Receive(receiveByte, receivedByteCount, cntByteCount, SocketFlags.None);
                    }
                    string json = Encoding.BigEndianUnicode.GetString(receiveByte);

                    _responseCallbacks.TryGetValue(info.targetSystem, out ResponseCallback callback);
                    callback?.Invoke(json);
                }
            }
        }
        catch (Exception e)
        {
            //Debug.LogException(e);
        }
    }

    private void AddResponseCallback(TargetSystem targetSystem, ResponseCallback callback)
    {
        if (_responseCallbacks.TryGetValue(targetSystem, out ResponseCallback cntCallback))
        {
            cntCallback += callback;
        }
        else
        {
            _responseCallbacks[targetSystem] = callback;
        }
    }

    private void RemoveResponseCallback(TargetSystem targetSystem, ResponseCallback callback)
    {
        try
        {
            _responseCallbacks[targetSystem] -= callback;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public string SerializeObject(object obj)
    {
        return JsonConvert.SerializeObject(obj, jsonSettings);
    }

    public T DeserializeJson<T>(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            return default;
        }
    }

    public void SendRequest(TargetSystem targetSystem, object req)
    {
        try
        {
            string json = SerializeObject(req);
            byte[] originBytes = Encoding.BigEndianUnicode.GetBytes(json);
            byte[] frameLengthBuffer = new byte[4];
            frameLengthBuffer = BitConverter.GetBytes(originBytes.Length);
            byte[] sendByte = frameLengthBuffer.Concat(originBytes).ToArray();

            _socketDict[targetSystem].Send(sendByte, sendByte.Length, 0);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void CloseSocket(Socket socket)
    {
        try
        {
            if (socket != null && socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}