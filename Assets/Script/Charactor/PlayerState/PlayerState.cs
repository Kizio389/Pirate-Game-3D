using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviourPun
{
    //------ Player Health -------- //
    public float currentHealth;
    public float maxHealth;


    //------ Player Clories -------- //
    public float currentCalories;
    public float maxCalories;

    float distanceTravelled =0;
    Vector3 lastPosition;

    public GameObject playerBody;



   

    public static PlayerState Instance { get;  set; }
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Xóa instance trùng lặp
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Giữ lại instance qua các scene
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentCalories = maxCalories;

        // Gán playerBody tự động nếu không được gán từ Inspector
        if (playerBody == null && PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine)
            {
                playerBody = this.gameObject; // Gán prefab hiện tại làm playerBody
            }
        }

        if (playerBody != null)
        {
            lastPosition = playerBody.transform.position; // Khởi tạo lastPosition
        }
        else
        {
            Debug.LogError("PlayerBody is not assigned or could not be found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(playerBody.transform.position, lastPosition);
        distanceTravelled += distance;
        Debug.Log($"Distance: {distance}, Player Position: {playerBody.transform.position}, Last Position: {lastPosition}");
        Debug.Log($"Distance: {distance}, Distance Travelled: {distanceTravelled}");

        if (distanceTravelled >= 5)
        {
            distanceTravelled = 0;
            currentCalories -= 50;
            Debug.Log($"Calories reduced: {currentCalories}");
        }

        lastPosition = playerBody.transform.position;
    }

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    } public void setCalories(float newCalories)
    {
        currentCalories = newCalories;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead");
        }
        else
        {
            Debug.Log("Player is hurt");
        }
    }
}
