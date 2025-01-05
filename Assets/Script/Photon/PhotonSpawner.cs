using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonSpawner : MonoBehaviour
{


    public GameObject spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
        GameObject player = PhotonNetwork.Instantiate(
        "Player",
        spawnPoint.transform.position,
        spawnPoint.transform.rotation);
        DontDestroyOnLoad(gameObject);
    }


        // Update is called once per frame
        void Update()
    {
        
    }
}
