using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DemoPhotonPun : MonoBehaviourPunCallbacks
{
    public InputField userNameText;
    public InputField RoomNameText;
    public InputField maxPlayer;

    public Button buttonFindRoom;
    public Button buttonBack;

    public GameObject PlayerNamePannel;
    public GameObject tableCreateRoom;
    public GameObject ConnectingPannel;
    public GameObject scrollView;
    public GameObject tableLooby;
    public GameObject RoomListPrefab;
    public GameObject roomListParent;

    private Dictionary<string, RoomInfo> roomListData = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> roomListGameObject = new Dictionary<string, GameObject>();

    void Start()
    {
        ActivateMyPannel(PlayerNamePannel.name);

        // Gắn sự kiện cho nút
        buttonFindRoom?.onClick.AddListener(ShowScrollView);
        buttonBack?.onClick.AddListener(ShowtableLooby);

        // Kiểm tra tất cả các tham chiếu cần thiết
        if (PlayerNamePannel == null || tableCreateRoom == null || ConnectingPannel == null || scrollView == null ||
            RoomListPrefab == null || roomListParent == null || tableLooby == null)
        {
            Debug.LogError("Một hoặc nhiều thành phần chưa được gán trong Inspector!");
        }
    }

    public void ActivateMyPannel(string PannelName)
    {
        PlayerNamePannel?.SetActive(PannelName.Equals(PlayerNamePannel.name));
        tableLooby?.SetActive(PannelName.Equals(tableLooby.name));
        tableCreateRoom?.SetActive(PannelName.Equals(tableCreateRoom.name));
        ConnectingPannel?.SetActive(PannelName.Equals(ConnectingPannel.name));
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon...");
        ActivateMyPannel(tableLooby.name);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Connected to Lobby.");
    }

    public void OnCreateRoomClick()
    {
        string roomName = RoomNameText.text;
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Tên phòng không được để trống!");
            return;
        }

        if (!int.TryParse(maxPlayer.text, out int maxPlayers) || maxPlayers <= 0 || maxPlayers > 20)
        {
            Debug.LogError("Số lượng người chơi không hợp lệ! Vui lòng nhập số từ 1 đến 20.");
            return;
        }

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayers };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    public void OnCancelClick()
    {
        ActivateMyPannel(tableLooby.name);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList || !room.IsOpen || !room.IsVisible)
            {
                if (roomListData.ContainsKey(room.Name))
                {
                    roomListData.Remove(room.Name);
                    if (roomListGameObject.ContainsKey(room.Name))
                    {
                        Destroy(roomListGameObject[room.Name]);
                        roomListGameObject.Remove(room.Name);
                    }
                }
            }
            else
            {
                if (!roomListData.ContainsKey(room.Name))
                {
                    roomListData.Add(room.Name, room);

                    if (RoomListPrefab == null || roomListParent == null)
                    {
                        Debug.LogError("RoomListPrefab hoặc roomListParent chưa được gán!");
                        continue;
                    }

                    GameObject roomListItemObject = Instantiate(RoomListPrefab, roomListParent.transform);
                    roomListItemObject.transform.localScale = Vector3.one;

                    // Gán thông tin phòng
                    var roomNameText = roomListItemObject.transform.GetChild(0).GetComponent<TMP_Text>();
                    var playerCountText = roomListItemObject.transform.GetChild(1).GetComponent<TMP_Text>();
                    var joinButton = roomListItemObject.transform.GetChild(2).GetComponent<Button>();

                    roomNameText?.SetText(room.Name);
                    playerCountText?.SetText($"{room.PlayerCount}/{room.MaxPlayers}");
                    joinButton?.onClick.AddListener(() => RoomJoinFromList(room.Name));

                    roomListGameObject.Add(room.Name, roomListItemObject);
                }
            }
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Tạo phòng thành công.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Tạo phòng thất bại: {message}");
    }

    public void RoomJoinFromList(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            StartCoroutine(WaitForLeaveLobbyAndJoinRoom(roomName));
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    private IEnumerator WaitForLeaveLobbyAndJoinRoom(string roomName)
    {
        while (PhotonNetwork.InLobby)
        {
            yield return null;
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    public void ClearRoomList()
    {
        foreach (var roomObject in roomListGameObject.Values)
        {
            Destroy(roomObject);
        }
        roomListGameObject.Clear();
    }

    public override void OnLeftLobby()
    {
        ClearRoomList();
        roomListData.Clear();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Đã vào phòng.");
        PhotonNetwork.LoadLevel("GameMainScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Tham gia phòng thất bại: {message}");
    }

    public void OnLoginClick()
    {
        string name = userNameText.text;
        if (!string.IsNullOrEmpty(name))
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
            ActivateMyPannel(ConnectingPannel.name);
        }
        else
        {
            Debug.LogError("Tên người chơi không được để trống!");
        }
    }

    public void ShowScrollView()
    {
        scrollView?.SetActive(true);
        tableLooby?.SetActive(false);
    }

    public void ShowtableLooby()
    {
        tableLooby?.SetActive(true);
        scrollView?.SetActive(false);
    }
}
