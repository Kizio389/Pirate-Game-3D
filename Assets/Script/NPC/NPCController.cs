using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NPCController : MonoBehaviour
{
    public GameObject player; // Tham chiếu đến nhân vật người chơi
    public Transform taskLocation; // Điểm đến của nhiệm vụ
    public List<string> dialogues; // Danh sách hội thoại
    public List<string> missionDialogues; // Hội thoại khi đến điểm nhiệm vụ

    public Image dialogueBackground; // Hình nền hội thoại
    public TextMeshProUGUI dialogueText; // Nội dung hội thoại

    private NavMeshAgent agent; // Điều khiển di chuyển NPC
    private Animator animator; // Animator của NPC
    private bool isTalking = false; // Trạng thái đối thoại
    private int dialogueIndex = 0; // Chỉ số hội thoại hiện tại
    private bool isLeading = false; // Trạng thái dẫn đường
    private bool atTaskLocation = false; // Trạng thái đã đến điểm nhiệm vụ
    private bool isPlayerInRange = false; // Kiểm tra người chơi có trong phạm vi
    private bool isFacingPlayer = false; // Trạng thái đã quay mặt về phía người chơi

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Lấy NavMeshAgent
        animator = GetComponent<Animator>(); // Lấy Animator
    }

    void Update()
    {
        // Mở hội thoại khi nhấn phím E và người chơi ở gần
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange && !isTalking)
        {
            StartDialogue();
        }

        // Tiếp tục dẫn đường đến điểm nhiệm vụ
        if (isLeading && !atTaskLocation)
        {
            if (Vector3.Distance(transform.position, taskLocation.position) < 1.0f)
            {
                ReachTaskLocation();
            }
        }

        // Tiếp tục hiển thị hội thoại khi nhấn phím E
        if (Input.GetKeyDown(KeyCode.E) && isTalking)
        {
            ShowNextDialogue();
        }

        // Cập nhật trạng thái chạy của NPC
        animator.SetBool("IsRunning", !agent.isStopped && agent.velocity.magnitude > 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            isFacingPlayer = false; // Reset trạng thái quay mặt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        dialogueIndex = 0;
        ShowDialogue();
    }

    void ShowDialogue()
    {
        if (dialogueIndex < dialogues.Count)
        {
            dialogueBackground.gameObject.SetActive(true);
            dialogueText.text = dialogues[dialogueIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowNextDialogue()
    {
        dialogueIndex++;
        ShowDialogue();
    }

    void EndDialogue()
    {
        isTalking = false;
        dialogueBackground.gameObject.SetActive(false);

        // Sau khi kết thúc hội thoại, NPC bắt đầu dẫn đường
        StartLeading();
    }

    void StartLeading()
    {
        isLeading = true;
        agent.isStopped = false;
        agent.SetDestination(taskLocation.position);
    }

    void ReachTaskLocation()
    {
        isLeading = false;
        atTaskLocation = true;
        agent.isStopped = true;
        animator.SetBool("IsRunning", false);

        // Quay mặt NPC về phía người chơi
        LookAtPlayer();

        // Cập nhật hội thoại đến đoạn nhiệm vụ
        dialogues = missionDialogues;

        // Hiển thị hội thoại tại điểm nhiệm vụ
        dialogueIndex = 0;
        StartDialogue();
    }

    void LookAtPlayer()
    {
        // Tính toán hướng quay về phía người chơi
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0; // Chỉ xoay trên trục Y

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        // Nếu góc nhỏ hơn 1 độ, coi như đã đối mặt với người chơi
        if (angle < 1.0f)
        {
            isFacingPlayer = true;
            transform.rotation = targetRotation; // Đảm bảo hướng chính xác
        }
        else
        {
            // Quay mượt mà về phía người chơi
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
