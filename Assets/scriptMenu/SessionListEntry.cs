using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionListEntry : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI roomname, playercount;
    [SerializeField]  public Button joinButton;
    public void JoinRoom ()
    {
        NetworkManager.runnerInstance.StartGame(new Fusion.StartGameArgs()
        {
            SessionName = roomname.text,
        });
    }    
}
