using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine.Profiling;
using UnityEngine.UI;


public class PhotonVoice : MonoBehaviour
{
    public Photon.Voice.Unity.Recorder recorder; // Chỉ định rõ namespace
    private PhotonView photonView;

    private bool isActiveVoice = true;
    private Text textStateVoice; 

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Tìm và lấy thành phần Recorder trong game object
        recorder = GameObject.FindGameObjectWithTag("Recorder")
            .GetComponent<Photon.Voice.Unity.Recorder>();

        // Tìm và lấy thành phần Text trong game object có tag "StateVoice"
        textStateVoice = GameObject
            .FindGameObjectWithTag("StateVoice")
            .GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ClickButtonToVoiceChat();
        }
    }

    public void ClickButtonToVoiceChat()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (isActiveVoice)
            {
                recorder.RecordingEnabled = false; // Tắt ghi âm
                isActiveVoice = false;
                textStateVoice.text = "Voice chat: OFF";
            }
            else
            {
                recorder.RecordingEnabled = true; // Bật ghi âm
                isActiveVoice = true;
                textStateVoice.text = "Voice chat: ON";
            }
        }
    }
}
