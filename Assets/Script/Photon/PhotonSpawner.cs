using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject PlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomVectorX = Random.Range(79, 88);
            int randomVectorZ = Random.Range(134, 150);
            PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(randomVectorX, 3f, randomVectorZ), Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
