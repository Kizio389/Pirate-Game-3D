using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    private bool isConnected = false;

    public InputField ipMessage;
    public Button buttonSend;

    public GameObject contentChat;
    public GameObject messagePrefab;
    private void Awake()
    {
        ChatConnection();
    }
    private void Start()
    {
        buttonSend.onClick.AddListener(SendMessage);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowCursor();
        }

        if (isConnected)
        {
            chatClient.Service();
        }
    }

    public void ChatConnection()
    {
        if (!isConnected)
        {
            // Kết nối chat
            chatClient = new ChatClient(this);
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
                PhotonNetwork.AppVersion,
                new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));

            isConnected = true;
        }
        else
        {
            // Ngắt kết nối và hủy đăng ký channel
            string[] unsubChannels = { "World" };
            chatClient.Unsubscribe(unsubChannels);
            chatClient.Disconnect();

            isConnected = false;
             // Ẩn chuột khi ngắt kết nối
        }
    }

    public void SendMessage()
    {
        if (isConnected && !string.IsNullOrEmpty(ipMessage.text))
        {
            chatClient.PublishMessage("World", ipMessage.text); // Gửi tin nhắn tới channel
            ipMessage.text = ""; // Xóa nội dung của InputField
        }
    }

    private void ShowCursor()
    {
        Cursor.visible = !Cursor.visible; // Đảo ngược trạng thái hiển thị con trỏ
        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked; // Khóa hoặc mở khóa con trỏ
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Level: " + level + " message: " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("State: " + state.ToString());
    }

    public void OnConnected()
    {
        string[] channels = { "World" };
        chatClient.Subscribe(channels);
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            Debug.Log("Channel: " + channelName);
            Debug.Log("Sender: " + senders[i]);
            Debug.Log("Message: " + messages[i]);

            if (messagePrefab != null && contentChat != null)
            {
                // Tạo một bản sao của prefab
                GameObject message = GameObject.Instantiate(messagePrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    contentChat.transform);

                // Kiểm tra và gán TextMeshProUGUI thay vì Text
                var textComponent = message.GetComponent<TMPro.TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = senders[i] + ": " + messages[i].ToString();
                }
                else
                {
                    Debug.LogError("messagePrefab is missing a TextMeshProUGUI component.");
                }
            }
            else
            {
                Debug.LogError("messagePrefab or contentChat is not assigned.");
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        // Không dùng private message, không cần implement.
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("User: " + user + " is: " + status);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (var channel in channels)
        {
            Debug.Log("OnSubscribed: " + channel);
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        foreach (var channel in channels)
        {
            Debug.Log("Unsubscribed: " + channel);
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        // Không cần thiết trong trường hợp này.
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log("User: " + user + " has unsubscribed from channel: " + channel);
    }
}