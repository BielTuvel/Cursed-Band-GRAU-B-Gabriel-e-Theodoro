using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;

public class Client : MonoBehaviour
{
    public TMP_InputField TextInputField;

    public PlayerDeck Deck;
    public GameObject ClientView;
    public CardDataBase DataBase;

    [Header("Client Network")]
    [SerializeField] private string _clientName;
    [SerializeField] private string _ipAddress = "127.0.0.1";
    [SerializeField] private int _port = 54010;
 
    private bool _isSocketReady;
    private TcpClient _clientSocket;
    private NetworkStream _stream = null;
    private StreamWriter _writer;
    private StreamReader _reader;
	//private byte[] _buffer = new byte[49152];
	//private int _bytesReceived = 0;
	//private string _receivedMessage = "";
    //private IEnumerator _listenServerMessagesCoroutine = null;

    public void ConnectToServer()
    {
        if(_isSocketReady)
        {
            return;
        }

        try
        {
            _clientSocket = new TcpClient(_ipAddress, _port);
            _stream = _clientSocket.GetStream();
            _writer = new StreamWriter(_stream);
            _reader = new StreamReader(_stream);
            _isSocketReady = true;
        }   
        catch(Exception exception)
        {
            Debug.Log("Socket error: " + exception.Message);
        }
    }

    public void OnSend()
    {
        Send(TextInputField.text);
    }

    private void Update()
    {
        if(_isSocketReady)
        {
            if(_stream.DataAvailable)
            {
                string data = _reader.ReadLine();

                if(data != null)
                {
                    OnIncomingData(data);
                }
            }
        }
    }

    private void OnIncomingData(string data)
    {
        if(data == "%NAME")
        {
            Send("&NAME|" + _clientName);
            return;
        }
        else if(data.Contains("ID"))
        {
            //JsonUtility.FromJsonOverwrite(data, Deck);
            PlayerDeckData deckData = new PlayerDeckData();
            deckData = JsonUtility.FromJson<PlayerDeckData>(data);

            for(int i = 0; i < 5; i++)
            {
                int cardID = deckData.Cards[i].ID;
                Deck.Deck[i] = DataBase.CardList[cardID];
            }

            ClientView.SetActive(true);
        }

        Debug.Log("Server: " + data);
    }

    private void Send(string data)
    {
        if(!_isSocketReady)
        {
            return;
        }

        _writer.WriteLine(data);
        _writer.Flush();
    }

    private void CloseSocket()
    {
        if(!_isSocketReady)
        {
            return;
        }

        _writer.Close();
        _reader.Close();
        _clientSocket.Close();
        _isSocketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }

    //Old Network
    /*public void ConnectToServer()
    {
        //If already connected, ignore
        if(_isSocketReady)
        {
            return;
        }

        try
        {
            _clientSocket = new TcpClient(_ipAddress, _port);
            //Debug.Log($"Client started on {_ipAddress}::{_port}");
            //_stream = _clientSocket.GetStream(); -> Talvez possa ficar aqui
            _listenServerMessagesCoroutine = ListenToServerMessages();
            StartCoroutine(_listenServerMessagesCoroutine);
            //SendMessageToServer("I'm connected now!");
            _isSocketReady = true;
        }
        catch (System.Exception)
        {
            Debug.Log($"Exception: Start server first");
            CloseClient();
        }
    }

    public void SendMessageToServer() 
    {
        try
        {
            _stream = _clientSocket.GetStream();
        }
        catch (System.Exception)
        {
            CloseClient();
            return;
        }

        if(!_clientSocket.Connected)
        {
            Debug.Log("Stablish server connection!");
            return;
        }

        byte[] encodedMessage = Encoding.ASCII.GetBytes(TextInputField.text); //Encode message as bytes

        _stream.Write(encodedMessage, 0, encodedMessage.Length);
    }

    public void SendMessageToServer(string message) 
    {
        try
        {
            _stream = _clientSocket.GetStream();
        }
        catch (System.Exception)
        {
            CloseClient();
            return;
        }

        if(!_clientSocket.Connected)
        {
            Debug.Log("Stablish server connection!");
            return;
        }

        byte[] encodedMessage = Encoding.ASCII.GetBytes(message); //Encode message as bytes

        _stream.Write(encodedMessage, 0, encodedMessage.Length);
    }

    protected void DisplayMessage(string message)
    {
        Debug.Log($"Server sent: {message}");
    }

    private IEnumerator ListenToServerMessages()
    {
        if(!_clientSocket.Connected)
        {
            yield break;
        }

        _stream = _clientSocket.GetStream();

        do
        {
            //Async read, calls "OnMessageReceived" when read overs
            _stream.BeginRead(_buffer, 0, _buffer.Length, OnMessageReceived, null);

            if(_bytesReceived > 0)
            {
                DisplayMessage(_receivedMessage);
                _bytesReceived = 0;
            }

            yield return null;

        } while (_bytesReceived >= 0 && _clientSocket != null && _stream != null);
    }

    private void OnMessageReceived(IAsyncResult result)
    {
        if(result.IsCompleted && _clientSocket.Connected)
        {
            _bytesReceived = _stream.EndRead(result);
            _receivedMessage = Encoding.ASCII.GetString(_buffer, 0, _bytesReceived);
        }
    }

    private void CloseClient()
	{
        if(_clientSocket == null)
        {
            return;
        }
        else
        {
            if (_clientSocket.Connected)
            {
                _clientSocket.Close();
                _stream.Close();
                _stream = null;
            }

            _isSocketReady = false;
            _clientSocket = null;
        }

        if (_listenServerMessagesCoroutine != null)
		{
			StopCoroutine(_listenServerMessagesCoroutine);
			_listenServerMessagesCoroutine = null;
		}	
	}

    private void OnApplicationQuit()
    {
        CloseClient();
    }

    private void OnDisable()
    {
        CloseClient();
    }

    public TcpClient ClientSocket { get => _clientSocket; }*/
}
