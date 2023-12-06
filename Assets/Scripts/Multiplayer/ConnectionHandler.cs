using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

public class ConnectionHandler : MonoBehaviour
{
    NetworkManager networkManager;

    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        networkManager.OnClientConnectedCallback += OnClientConnected;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    void OnApplicationPause(bool paused)
    {
        if (!paused)
        {
            if (networkManager == null) return;

            if (networkManager.IsConnectedClient)
                CheckConnection();
        }
    }

    void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.OnClientConnectedCallback -= OnClientConnected;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            networkManager.Shutdown();
        }
    }

    async void CheckConnection()
    {
        await Task.Delay(500);

        if (networkManager != null && !networkManager.IsConnectedClient)
            OnClientDisconnected(networkManager.LocalClientId);
    }

    void OnClientConnected(ulong clientId)
    {
        var remoteClientConnected = clientId != networkManager.LocalClientId;

        if (remoteClientConnected && !networkManager.ShutdownInProgress)
            LobbyManager.Instance.RemotePlayerConnected();
    }

    void OnClientDisconnected(ulong clientId)
    {
        var remoteClientDisconnected = clientId != networkManager.LocalClientId;

        if (!networkManager.ShutdownInProgress)
        {
            if (remoteClientDisconnected)
            {
                LobbyManager.Instance.RemotePlayerDisconnected();
            }
            //else
            //I leave the game and go to the main menu here
        }
    }
}