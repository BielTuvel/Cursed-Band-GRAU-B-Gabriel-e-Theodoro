using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ServerClient 
{
    private TcpClient _clientSocket;
    private string _clientName;

    public ServerClient(TcpClient clientSocket) 
    {
        _clientSocket = clientSocket;
        _clientName = "Guest";
    }

    public TcpClient ClientSocket { get => _clientSocket; set => _clientSocket = value; }
    public string ClientName{ get => _clientName; set => _clientName = value; }
}
