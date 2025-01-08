using UnityEngine;
using TMPro; // Thư viện cho TextMeshPro
using Photon.Pun;

public class NPCDialogue : MonoBehaviourPunCallbacks
{
    public string[] dialogue; // Mảng các câu thoại
    private int dialogueIndex = 0; // Vị trí của câu thoại hiện tại
    public GameObject dialogueCanvas; // Canvas hiển thị hội thoại
    public TextMeshProUGUI dialogueText; // TextMeshPro cho câu thoại
    public GameObject pressEPanel; // Panel hiển thị "Press E"
    public GameObject arrowPrefab; // Prefab mũi tên điều hướng
    public Transform targetLocation; // Vị trí mục tiêu mà người chơi cần đến

    private bool isPlayerInRange = false; // Kiểm tra xem người chơi có trong phạm vi NPC không

    private void Start()
    {
        Debug.Log("Start() called: Initializing UI states.");
        dialogueCanvas.SetActive(false);
        pressEPanel.SetActive(false);
    }

    // Phương thức để bắt đầu đối thoại
    public void StartDialogue()
    {
        Debug.Log("StartDialogue() called.");
        if (photonView == null || photonView.IsMine) // Chỉ người chơi sở hữu PhotonView mới có thể bắt đầu đối thoại
        {
            Debug.Log($"Dialogue index: {dialogueIndex} / Dialogue length: {dialogue.Length}");
            if (dialogueIndex < dialogue.Length)
            {
                Debug.Log($"Sending dialogue: {dialogue[dialogueIndex]}");
                photonView.RPC("UpdateDialogue", RpcTarget.All, dialogue[dialogueIndex]);
                dialogueIndex++;
            }
            else
            {
                Debug.Log("End of dialogue reached.");
                EndDialogue();
            }
        }
        else
        {
            Debug.LogWarning("PhotonView is not owned by this player.");
        }
    }

    // RPC để đồng bộ hóa câu thoại cho tất cả người chơi
    [PunRPC]
   
    void UpdateDialogue(string dialogueLine)
    {
        if (dialogueCanvas == null)
        {
            Debug.LogError("Error: dialogueCanvas is NULL. Check if it has been assigned in the Inspector.");
            return;
        }

        Debug.Log($"Updating dialogue. Setting dialogue text to: {dialogueLine}");
        dialogueText.text = dialogueLine;

        Debug.Log("Activating DialogueCanvas...");
        dialogueCanvas.SetActive(true); // Hiển thị Canvas

        if (!dialogueCanvas.activeSelf)
        {
            Debug.LogError("DialogueCanvas was not activated successfully.");
        }
    }

    // Phương thức kết thúc đối thoại và hiển thị mũi tên
    void EndDialogue()
    {
        Debug.Log("EndDialogue() called.");
        if (photonView.IsMine)
        {
            Debug.Log("Displaying navigation arrow.");
            photonView.RPC("ShowArrow", RpcTarget.All); // Hiển thị mũi tên cho tất cả người chơi
        }
        dialogueCanvas.SetActive(false); // Ẩn hộp thoại
    }

    // RPC để đồng bộ hóa mũi tên cho tất cả người chơi
    [PunRPC]
    void ShowArrow()
    {
        Debug.Log("ShowArrow() called.");
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        ArrowController arrowController = arrow.GetComponent<ArrowController>();
        arrowController.SetTarget(targetLocation);

        // Gán người chơi làm tham chiếu
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            arrowController.SetPlayer(player.transform);
        }
    }

    void Update()
    {
        // Kiểm tra nếu người chơi trong phạm vi và nhấn E để bắt đầu đối thoại
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E. Starting dialogue.");
            StartDialogue();
        }
    }

    // Phát hiện khi người chơi vào hoặc rời khỏi phạm vi của NPC
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered NPC range.");
            isPlayerInRange = true;
            pressEPanel.SetActive(true); // Hiển thị panel chữ "Press E"
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited NPC range.");
            isPlayerInRange = false;
            pressEPanel.SetActive(false); // Ẩn panel chữ "Press E"
            dialogueCanvas.SetActive(false); // Ẩn hộp thoại nếu có
        }
    }
}
