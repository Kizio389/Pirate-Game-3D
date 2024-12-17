using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DemoPhotonPun : MonoBehaviourPunCallbacks
{
    // Các InputField để nhập tên người chơi, tên phòng và số lượng người chơi tối đa
    public InputField userNameText;
    public InputField RoomNameText;
    public InputField maxPlayer;

    // Các Button điều khiển chức năng
    public Button buttonFindRoom;
    public Button buttonBack;

    // Các panel trong UI
    public GameObject PlayerNamePannel; // Panel nhập tên người chơi
    public GameObject tableCreateRoom;  // Panel tạo phòng
    public GameObject ConnectingPannel; // Panel hiển thị khi đang kết nối
    public GameObject RoomListPanel;       // Panel chứa danh sách phòng
    public GameObject tableLooby;       // Panel sảnh chờ chính

    // Prefab và parent để hiển thị danh sách phòng
    public GameObject RoomListPrefab;   // Prefab của item danh sách phòng
    public GameObject roomListParent;   // Parent chứa danh sách phòng

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;      // Panel hiển thị khi đã vào phòng
    public GameObject playerListItemPrefab; // Prefab của item danh sách người chơi trong phòng
    public GameObject playerListItemParent; // Parent chứa danh sách người chơi
    public GameObject Playbutton;

    // Lưu trữ thông tin các phòng và các GameObject đại diện cho chúng
    private Dictionary<string, RoomInfo> roomListData = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> roomListGameObject = new Dictionary<string, GameObject>();

    private Dictionary<int, GameObject> playerListGameObject;


    void Start()
    {
        ActivateMyPannel(PlayerNamePannel.name); // Hiển thị panel nhập tên người chơi khi khởi động

        // Gắn sự kiện cho các nút
        buttonFindRoom?.onClick.AddListener(ShowScrollView); // Khi nhấn tìm phòng sẽ hiển thị danh sách phòng
        buttonBack?.onClick.AddListener(ShowtableLooby);     // Khi nhấn nút Back sẽ quay về sảnh chờ

        // Kiểm tra tất cả các tham chiếu cần thiết trong Inspector
        if (PlayerNamePannel == null || tableCreateRoom == null || ConnectingPannel == null || RoomListPanel == null ||
            RoomListPrefab == null || roomListParent == null || tableLooby == null)
        {
            Debug.LogError("Một hoặc nhiều thành phần chưa được gán trong Inspector!");
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Hàm kích hoạt panel tương ứng với tên
    public void ActivateMyPannel(string PannelName)
    {
        // Bật hoặc tắt các panel dựa trên panel được chọn
        PlayerNamePannel?.SetActive(PannelName.Equals(PlayerNamePannel.name));
        tableLooby?.SetActive(PannelName.Equals(tableLooby.name));
        tableCreateRoom?.SetActive(PannelName.Equals(tableCreateRoom.name));
        ConnectingPannel?.SetActive(PannelName.Equals(ConnectingPannel.name));
        InsideRoomPanel?.SetActive(PannelName.Equals(InsideRoomPanel.name));

        // Đảm bảo scrollView ẩn khi InsideRoomPanel được kích hoạt
        if (PannelName.Equals(InsideRoomPanel.name))
        {
            RoomListPanel?.SetActive(false);
        }
    }

    // Gọi khi kết nối đến server Photon thành công
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon...");
        ActivateMyPannel(tableLooby.name); // Chuyển sang sảnh chờ chính
        PhotonNetwork.JoinLobby(); // Tham gia lobby để lấy danh sách phòng
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Connected to Lobby.");
    }

    // Tạo phòng mới với thông tin nhập từ InputField
    public void OnCreateRoomClick()
    {
        string roomName = RoomNameText.text;
        if (string.IsNullOrEmpty(roomName)) // Kiểm tra tên phòng không được trống
        {
            Debug.LogError("Tên phòng không được để trống!");
            return;
        }

        // Kiểm tra số lượng người chơi hợp lệ
        if (!int.TryParse(maxPlayer.text, out int maxPlayers) || maxPlayers <= 0 || maxPlayers > 20)
        {
            Debug.LogError("Số lượng người chơi không hợp lệ! Vui lòng nhập số từ 1 đến 20.");
            return;
        }

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayers }; // Cài đặt giới hạn số người chơi
        PhotonNetwork.CreateRoom(roomName, roomOptions); // Tạo phòng mới
    }

    // Nút hủy quay lại sảnh chờ
    public void OnCancelClick()
    {
        ActivateMyPannel(tableLooby.name);
    }

    // Cập nhật danh sách phòng khi nhận được từ server Photon
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList || !room.IsOpen || !room.IsVisible)
            {
                // Nếu phòng không còn tồn tại, xóa khỏi danh sách
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
                    roomListData.Add(room.Name, room); // Thêm phòng mới vào danh sách

                    if (RoomListPrefab == null || roomListParent == null)
                    {
                        Debug.LogError("RoomListPrefab hoặc roomListParent chưa được gán!");
                        continue;
                    }

                    // Tạo item trong danh sách phòng
                    GameObject roomListItemObject = Instantiate(RoomListPrefab, roomListParent.transform);
                    roomListItemObject.transform.localScale = Vector3.one;

                    // Gán thông tin phòng vào item
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

    // Tham gia phòng từ danh sách phòng
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
            yield return null; // Chờ cho đến khi thoát khỏi lobby
        }
        PhotonNetwork.JoinRoom(roomName); // Tham gia phòng
    }

    // Xóa danh sách phòng trên UI
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

    // Gọi khi người chơi tham gia phòng thành công
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "Connected Inside Room .");
        ActivateMyPannel(InsideRoomPanel.name); // Hiển thị panel trong phòng

        if (playerListGameObject == null)
        {
            playerListGameObject = new Dictionary<int, GameObject>();
        }
        
       

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(playerListItemPrefab);
            playerListItem.transform.SetParent(playerListItemParent.transform);
            playerListItem.transform.localScale = Vector3.one;

            // Hiển thị tên người chơi trong danh sách
            playerListItem.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = p.NickName;
            if (p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListItem.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                playerListItem.transform.GetChild(0).gameObject.SetActive(false);
            }
            playerListGameObject.Add(p.ActorNumber, playerListItem);
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        GameObject playerListItem = Instantiate(playerListItemPrefab);
        playerListItem.transform.SetParent(playerListItemParent.transform);
        playerListItem.transform.localScale = Vector3.one;

        // Hiển thị tên người chơi trong danh sách
        playerListItem.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListItem.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            playerListItem.transform.GetChild(0).gameObject.SetActive(false);
        }
        playerListGameObject.Add(newPlayer.ActorNumber, playerListItem);

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListGameObject[otherPlayer.ActorNumber]);
        playerListGameObject.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.IsMasterClient)
        {
            Playbutton.SetActive(true);
        }
        else
        {
            Playbutton.SetActive(false);
        }
    }
    public void OnClickPlayButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameMainScene");
        }
        
    }

    public override void OnLeftRoom()
    {
        ActivateMyPannel(tableLooby.name);
        foreach (GameObject obj in playerListGameObject.Values)
        {
            Destroy(obj);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Tham gia phòng thất bại: {message}");
    }

    // Đăng nhập với tên người chơi
    public void OnLoginClick()
    {
        string name = userNameText.text;
        if (!string.IsNullOrEmpty(name))
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings(); // Kết nối tới server Photon
            ActivateMyPannel(ConnectingPannel.name);
        }
        else
        {
            Debug.LogError("Tên người chơi không được để trống!");
        }
    }
    public void BackFromplayerList()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        ActivateMyPannel(tableLooby.name);
    }
    // Hiển thị danh sách phòng
    public void ShowScrollView()
    {
        RoomListPanel?.SetActive(true);
        tableLooby?.SetActive(false);
    }

    // Quay lại sảnh chờ chính
    public void ShowtableLooby()
    {
        tableLooby?.SetActive(true);
        RoomListPanel?.SetActive(false);
    }
}