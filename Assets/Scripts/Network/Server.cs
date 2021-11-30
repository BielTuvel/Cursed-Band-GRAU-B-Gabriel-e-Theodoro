using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Server : MonoBehaviour
{
    public Action OnPlayersConnected;
    public Action OnPlayerConnected;

    public List<ServerClient> Clients = new List<ServerClient>();
    public List<ServerClient> DisconnectedClients = new List<ServerClient>();

    public PlayerDeck[] Decks;
    public PlayerDeckData[] DeckData;

    [Header("Server Network")]
	//[SerializeField] private string _ipAddress = "127.0.0.1";
	[SerializeField] private int _port = 54010;
    //[SerializeField] [TextArea] private string _receivedMessage = "";

    private TcpListener _serverSocket = null;
    //private TcpClient _clientSocket = null;
    //private NetworkStream[] _streams = null;
	//private byte[] _buffer = new byte[49152];
	//private int _bytesReceived = 0;
    private bool _hasServerStarted;
    //private bool _isListeningToClients;
    //private IEnumerator _listenClientMessagesCoroutine = null;

    private void Start()
    {
        try
        {
            _serverSocket = new TcpListener(IPAddress.Any, _port);
            _serverSocket.Start();
            ListenToIncomingClients();
            _hasServerStarted = true;
            Debug.Log("Server has started on port: " + _port);
        }
        catch (Exception exception)
        {
            Debug.Log("Socket error: " + exception.Message);
        }
    }

    private void Update()
    {
        if(!_hasServerStarted)
        {
            return;
        }

        foreach(ServerClient client in Clients)
        {
            if(!IsConnected(client.ClientSocket))
            {
                client.ClientSocket.Close();
                DisconnectedClients.Add(client);
                return;
            }
            else
            {
                NetworkStream stream = client.ClientSocket.GetStream();

                if(stream.DataAvailable)
                {
                    StreamReader reader = new StreamReader(stream, true);
                    string data = reader.ReadLine();

                    if(data != null)
                    {
                        OnIncomingData(client, data);
                    }
                }
            }
        }

        for(int i = 0; i < DisconnectedClients.Count - 1; i++)
        {
            BroadcastMessage(DisconnectedClients[i].ClientName + " has disconnected!", Clients);

            Clients.Remove(DisconnectedClients[i]);
            DisconnectedClients.RemoveAt(i);
        }
    }

    private void ListenToIncomingClients()
    {
        _serverSocket.BeginAcceptTcpClient(OnClientConnected, _serverSocket);
    }

    private void OnClientConnected(IAsyncResult result)
    {
        Clients.Add(new ServerClient(_serverSocket.EndAcceptTcpClient(result)));
        ListenToIncomingClients();
        
        //BroadcastMessage(Clients[Clients.Count - 1].ClientName + " has connected!", Clients);
        BroadcastMessage("%NAME", new List<ServerClient>() { Clients[Clients.Count - 1]});
        //SendMessage("%NAME", Clients[Clients.Count - 1]);

        /*if(Clients.Count > 1)
        {
            OnPlayersConnected?.Invoke();
        }*/
    }

    private void OnIncomingData(ServerClient client, string data)
    {
        //Debug.Log(client.ClientName + " has sent: " + data);
        if(data.Contains("&NAME"))
        {
            PlayerDeck deck = Decks[Clients.Count - 1];
            PlayerDeckData deckData = DeckData[Clients.Count - 1];

            for(int i = 0; i < 5; i++)
            {
                deckData.Cards[i].ID = deck.Deck[i].Id;
            }

            try
            {
                string deckObject = JsonUtility.ToJson(deckData);
                BroadcastMessage(deckObject, new List<ServerClient>() { Clients[Clients.Count - 1]});
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }

            client.ClientName = data.Split('|')[1];
            //BroadcastMessage(client.ClientName + " has connected!", Clients);*/

            return; 
        }

        BroadcastMessage(client.ClientName + " : " + data, Clients); 
    }

    private void BroadcastMessage(string data, List<ServerClient> clients)
    {
        foreach(ServerClient client in clients)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.ClientSocket.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch(Exception exception)
            {
                Debug.Log("Write error: " + exception.Message + "to client " + client.ClientName);
            }
        }
    }

    /*private void SendMessage(string data, ServerClient client)
    {
        try
        {
            StreamWriter writer = new StreamWriter(client.ClientSocket.GetStream());
            writer.WriteLine(data);
            writer.Flush();
        }
        catch(Exception exception)
        {
            Debug.Log("Write error: " + exception.Message + "to client " + client.ClientName);
        }
    }*/

    private bool IsConnected(TcpClient client)
    {
        try
        {
            if(client != null && client.Client != null && client.Client.Connected)
            {
                if(client.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    //Old Network
	/*protected string ReceivedMessage
	{
		get { return _receivedMessage; }
		set
		{
			_receivedMessage = value;
		}
	}

    private void Update()
    {
        if(!_hasServerStarted)
        {
            return;
        }

        foreach(ServerClient client in Clients)
        {
            if(Input.GetKeyDown(KeyCode.H))
            {
                if (client.ClientSocket != null)
		        {
		        	client.ClientSocket.Close();
		        	client.ClientSocket = null;
		        }
            }

            if(!IsClientConnected(client.ClientSocket))
            {
                DisconnectedClients.Add(client);
                continue;
            }
        }

        if(Clients.Count > 0 && !_isListeningToClients) 
        {   
            _listenClientMessagesCoroutine = ListenToClientMessages();
            StartCoroutine(_listenClientMessagesCoroutine);
            _isListeningToClients = true;
        }

        for(int i = 0; i < DisconnectedClients.Count - 1; i++)
        {
            Clients.Remove(DisconnectedClients[i]);
            SendMessageToClients(DisconnectedClients[i].ClientName + " has disconnected!");
            DisconnectedClients.RemoveAt(1);
            Debug.Log("Disconnected!");
        }
    }

    protected virtual void StartServer()
    {
        try
        {
            //InititializeServerData();
            //IPAddress ip = IPAddress.Parse(_ipAddress);
            _serverSocket = new TcpListener(IPAddress.Any, _port);
            //_serverSocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            _serverSocket.Start();
            Debug.Log($"Server Started on {_ipAddress}::{_port}");
            _hasServerStarted = true;
		    //Wait for async client connection 
		    StartAcceptingClient();
        }
        catch (System.Exception)
        {
            Debug.Log("Socket or Format Exception: Port or IP not available");
            return;
        }
    }

    private void Start()
    {
        StartServer();
    }

    private void OnClientConnected(IAsyncResult result)
    {
        //Set the client reference
        TcpClient clientSocket;
		clientSocket = _serverSocket.EndAcceptTcpClient(result);

        ServerClient serverClient = new ServerClient(clientSocket);
        Clients.Add(serverClient);
        SendMessageToClients("Welcome! " + serverClient.ClientName);
        StartAcceptingClient();
    }

    private bool IsClientConnected(TcpClient client)
    {
        if(client != null && client.Connected)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartAcceptingClient()
    {
        _serverSocket.BeginAcceptTcpClient(OnClientConnected, null);
    }

    private void SetMessageToUpperCase()
    {
        _receivedMessage = _receivedMessage.ToUpper();
    }

    protected void SendMessageToClients(string message)
    {
        if(Clients.Count > 0)
        {
            foreach(ServerClient client in Clients)
            {
                NetworkStream stream = client.ClientSocket.GetStream();

                if(stream == null)
                {
                    Debug.Log("Start the client first");
                    return;
                }

                byte[] encodedMessage = Encoding.ASCII.GetBytes(message);

                stream.Write(encodedMessage, 0, encodedMessage.Length);
            }
        }
    }

    protected virtual void CloseServer()
	{
		Debug.Log("Server Closed");

        foreach(ServerClient client in Clients)
        {
            if (client.ClientSocket != null)
		    {
		    	//_stream?.Close();
		    	//_stream = null;
		    	client.ClientSocket.Close();
		    	client.ClientSocket = null;
		    }
        }

		//Close server connection
		if (_serverSocket != null)
		{
			_serverSocket.Stop();
			_serverSocket = null;
		}

		if (_listenClientMessagesCoroutine != null)
		{
			StopCoroutine(_listenClientMessagesCoroutine);
			_listenClientMessagesCoroutine = null;
		}
	}

    protected virtual void DisplayMessage(string messageReceived)
    {
        Debug.Log($"Client sent: {messageReceived}");
        SetMessageToUpperCase();
        SendMessageToClients(_receivedMessage);
    }

    private IEnumerator ListenToClientMessages()
    {
        while ((_bytesReceived >= 0) && Clients.Count > 0) //&& stream != null && client.ClientSocket != null
        {
            foreach(ServerClient client in Clients)
            {
                if(client.ClientSocket == null)
                {
                    continue;
                }

                NetworkStream stream = client.ClientSocket.GetStream();

                stream.BeginRead(_buffer, 0, _buffer.Length, OnMessageReceived, stream);

                if(_bytesReceived > 0)
                {
                    DisplayMessage(_receivedMessage);
                    _bytesReceived = 0;
                }
            }
            
            yield return null;
        } 

        _isListeningToClients = false;
    }

    private void OnMessageReceived(IAsyncResult result)
    {
        NetworkStream stream = (NetworkStream)result.AsyncState;

        if(result.IsCompleted)
        {
            _bytesReceived = stream.EndRead(result);
            _receivedMessage = Encoding.ASCII.GetString(_buffer, 0, _bytesReceived);
        }
    }

    private void OnApplicationQuit()
    {
        CloseServer();
    }
    */
}
