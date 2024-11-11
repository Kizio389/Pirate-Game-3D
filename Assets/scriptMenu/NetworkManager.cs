using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using static System.Collections.Specialized.BitVector32;
using UnityEditor;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runnerInstance;
    public string LobbyName = "abc";
    public Transform sessionListContentParent;
    public GameObject sessionListEntryPrefab;
    public Dictionary<string, GameObject> sessionListUIDictionary = new Dictionary<string, GameObject>();

    public string GameSceneName;

    private void Awake()
    {
        runnerInstance = gameObject.AddComponent<NetworkRunner>();
        if (runnerInstance == null)
        {
            runnerInstance = gameObject.AddComponent<NetworkRunner>();
        }
    }
    private void Start()
    {
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, LobbyName);
    }

    public void CreateRandomSession()
    {
        int randomInt = UnityEngine.Random.Range(1000, 9999);

        string randomSessionName = "Room_" + randomInt.ToString();
        runnerInstance.StartGame(new Fusion.StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(GameSceneName)),
            SessionName = randomSessionName,
            GameMode = GameMode.Shared,
        });
    }

    public int GetSceneIndex(string sceneName)
    {
        // Vòng lặp qua tất cả các scene trong Build Settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            // Lấy đường dẫn của scene từ Build Settings bằng chỉ số i
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

            // Trích xuất tên của scene từ đường dẫn, loại bỏ phần mở rộng (".unity")
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            // Kiểm tra nếu tên scene khớp với tên được truyền vào
            if (name == sceneName)
            {
                // Nếu khớp, trả về chỉ số của scene
                return i;
            }
        }
        // Nếu không tìm thấy scene, trả về -1
        return -1;
    }

    [SerializeField] private NetworkPrefabRef playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }
   

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        DeleteOldSessionsFromUI(sessionList);
        CompareLists(sessionList);
    }


    private void CreateEntryUI(SessionInfo session)
    {
        GameObject newEntry = GameObject.Instantiate(sessionListEntryPrefab);
        newEntry.transform.parent = sessionListContentParent;
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
        sessionListUIDictionary.Add(session.Name, newEntry);

        entryScript.roomname.text = session.Name;
        entryScript.playercount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
    }

    private void UpdateEntryUI(SessionInfo session)
    {
        sessionListUIDictionary.TryGetValue(session.Name, out GameObject newEntry);
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();


        entryScript.roomname.text = session.Name;
        entryScript.playercount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
    }
    private void DeleteOldSessionsFromUI(List<SessionInfo> sessionList)
    {
        bool isContained = false;
        GameObject uiDelete = null;
        foreach (KeyValuePair<string, GameObject> kvp in sessionListUIDictionary)
        {
            string sessionkey = kvp.Key;
            foreach (SessionInfo sessionInfo in sessionList)
            {
                if (sessionInfo.Name == sessionkey)
                {
                    isContained = true;
                    break;
                }
            }
            if (!isContained)
            {
                uiDelete = kvp.Value;
                sessionListUIDictionary.Remove(sessionkey);
                Destroy(uiDelete);
            }
        }
    }

    private void CompareLists(List<SessionInfo> sessionlist)
    {
        foreach (SessionInfo session in sessionlist)
        {
            if (sessionListUIDictionary.ContainsKey(session.Name))
            {
                UpdateEntryUI(session);
            }
            else
            {
                CreateEntryUI(session);
            }
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }



    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }





    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }


    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }


    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }


    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
}
