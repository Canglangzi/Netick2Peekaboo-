using UnityEngine;
using Netick;
using Netick.Unity;

[DisallowMultipleComponent]
[RequireComponent(typeof(NetworkManager))]
[CKBScriptInfo("Network Manager HUD", "This script provides a graphical user interface (GUI) for interacting with the NetworkManager.", ScriptHeaderBackColor.Red)]
public sealed class NetworkManagerHud : GUIBase
{
    public NetworkManager NetworkManager;

    [Header("Network Settings")]
    [SerializeField] private float _size = 250f; // HUD Size
    [SerializeField] private int _numberOfClients = 1;
    [SerializeField] private int _numberOfServers = 1;
    [SerializeField] private Vector2 _position = new Vector2(10, 50);
    [SerializeField] private DisplayPosition _displayPosition = DisplayPosition.TopLeft;

    [SerializeField] private int _port = 7777;
    [SerializeField] private string _serverIPAddress = "127.0.0.1";

    protected override void DisplayGUI()
    {
        
        Vector2 position = GetPositionBasedOnDisplay();
        GUILayout.BeginArea(new Rect(position.x, position.y, _size, _size));

        GUILayout.Space(5);
        /*if (!Sandbox.IsVisible)          
            return;*/
        if (!Network.IsRunning)
        {
            StartButtons();
           // Debug.Log("Network Manager has not been running.");
        }
        else
        {
            StatusLabels();
          //  Debug.Log("Runing.");
        }

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        // IP and Port input along with Client button in one line
        GUILayout.BeginHorizontal();
        _serverIPAddress = GUILayout.TextField(_serverIPAddress, GUILayout.Width(100));
        _port = int.TryParse(GUILayout.TextField(_port.ToString(), GUILayout.Width(50)), out int port) ? port : _port;
        
        if (GUILayout.Button("Client", GUILayout.Width(60), GUILayout.Height(20)))
        {
            NetworkManager.ServerIPAddress = _serverIPAddress; // Set the IP address
            NetworkManager.Port = _port; // Set the port
            NetworkManager.StartClient();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Host, Server, Single Player and Start Multiple Peers buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Host", GUILayout.Width(60), GUILayout.Height(20)))
        {
            NetworkManager.ServerIPAddress = _serverIPAddress; 
            NetworkManager.Port = _port; // Set the port
            NetworkManager.StartHost();
        }
        if (GUILayout.Button("Server", GUILayout.Width(60), GUILayout.Height(20)))
        {
            NetworkManager.ServerIPAddress = _serverIPAddress; 
            NetworkManager.Port = _port; // Set the port
            NetworkManager.StartServer();
        }
        if (GUILayout.Button("Single Player", GUILayout.Width(90), GUILayout.Height(20)))
        {
            NetworkManager.ServerIPAddress = _serverIPAddress; 
            NetworkManager.Port = _port; // Set the port
            NetworkManager.StartSinglePlayer();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Start Multiple Peers configuration
        GUILayout.BeginHorizontal();
        _numberOfClients = int.TryParse(GUILayout.TextField(_numberOfClients.ToString(), GUILayout.Width(40)), out int clients) ? clients : _numberOfClients;
        _numberOfServers = int.TryParse(GUILayout.TextField(_numberOfServers.ToString(), GUILayout.Width(40)), out int servers) ? servers : _numberOfServers;
        if (GUILayout.Button("Start Peers", GUILayout.Width(90), GUILayout.Height(20)))
        {
            NetworkManager.NumberOfClients = _numberOfClients;
            NetworkManager.NumberOfServers = _numberOfServers;
            NetworkManager.Port = _port; // Set the port
            NetworkManager.StartMultiplePeers();
        }
        GUILayout.EndHorizontal();
    }

    private void StatusLabels()
    {
        GUILayout.Space(5);

        if (NetworkManager.IsHost)
        {
            GUILayout.Label($"Host - {_serverIPAddress}:{_port}", GUILayout.Width(_size));
            if (GUILayout.Button("Stop Host", GUILayout.Width(60), GUILayout.Height(20)))
            {
                NetworkManager.StopHost();
            }
        }
        else if (NetworkManager.IsClient)
        {
            GUILayout.Label($"Client - {_serverIPAddress}:{_port}", GUILayout.Width(_size));
            
            if (GUILayout.Button("Stop Client", GUILayout.Width(60), GUILayout.Height(20)))
            {
                NetworkManager.StopClient();
            }

            // Show "Disconnect from Server" button when connected as a client
            if (NetworkManager.IsConnected)
            {
                if (GUILayout.Button("Disconnect from Server", GUILayout.Width(150), GUILayout.Height(20)))
                {
                    NetworkManager.ClientDisconnectFromServer(); // Disconnect the client from the server
                }
            }
            else
            {
                GUILayout.Label("Enter IP and Port to retry connection", GUILayout.Width(_size));
                GUILayout.BeginHorizontal();
                _serverIPAddress = GUILayout.TextField(_serverIPAddress, GUILayout.Width(100)); // Input field for IP
                _port = int.TryParse(GUILayout.TextField(_port.ToString(), GUILayout.Width(50)), out int port) ? port : _port; // Input field for Port

                if (GUILayout.Button("Retry Connect", GUILayout.Width(90), GUILayout.Height(20)))
                {
                    NetworkManager.ServerIPAddress = _serverIPAddress; // Set the IP address
                    NetworkManager.Port = _port; // Set the port
                    NetworkManager.ClientConnect(); // Attempt to connect again
                }
                GUILayout.EndHorizontal();
            }
        }
     
        else if (NetworkManager.IsServer)
        {
            GUILayout.Label("Server", GUILayout.Width(_size));
            if (GUILayout.Button("Stop Server", GUILayout.Width(60), GUILayout.Height(20)))
            {
                NetworkManager.StopServer();
            }
        }
        else if (NetworkManager.IsSinglePlayer)
        {
            GUILayout.Label("Single Player", GUILayout.Width(_size));
            if (GUILayout.Button("Stop Single Player", GUILayout.Width(90), GUILayout.Height(20)))
            {
                NetworkManager.StopSinglePlayer();
            }
        }
    }

    private Vector2 GetPositionBasedOnDisplay()
    {
        Vector2 position = _position;

        switch (_displayPosition)
        {
            case DisplayPosition.TopLeft:
                position = new Vector2(_position.x, _position.y);
                break;
            case DisplayPosition.TopRight:
                position = new Vector2(Screen.width - _size - _position.x, _position.y);
                break;
            case DisplayPosition.BottomLeft:
                position = new Vector2(_position.x, Screen.height - _size - _position.y);
                break;
            case DisplayPosition.BottomRight:
                position = new Vector2(Screen.width - _size - _position.x, Screen.height - _size - _position.y);
                break;
        }

        return position;
    }

    private enum DisplayPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
