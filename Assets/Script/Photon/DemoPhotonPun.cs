using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DemoPhotonPun : MonoBehaviourPunCallbacks
{
    public TMP_Text textState;
    public InputField IpRoomName;
    public Button buttonCreateRoom;
    public Button buttonJoinRoom;

    void Start()
    {
        Debug.Log("Bắt đầu kết nối tới Photon...");
        PhotonNetwork.ConnectUsingSettings();

        buttonCreateRoom.onClick.AddListener(CreateRoom);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master Server!");
        PhotonNetwork.JoinLobby();
        textState.text = "loading ...";
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        textState.text = "Connected";
       
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(IpRoomName.text))
        {
            Debug.LogError("Room name cannot be empty!");
            return;
        }
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("Client chưa sẵn sàng kết nối!");
            return;
        }

        if (!PhotonNetwork.InLobby)
        {
            Debug.LogError("Client chưa tham gia Lobby!");
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(IpRoomName.text, roomOptions);

    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Create Room Successful");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError($"Create Room Failed (Code: {returnCode}): {message}");
     
    }
}
