using System.Net.Sockets;

public interface IConnection
{
    TargetSystem TargetSystem { get; }
    void OnReceive(string json);
}