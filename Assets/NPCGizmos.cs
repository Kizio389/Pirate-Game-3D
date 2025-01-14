using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 4f); // Attacking Distance

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 9f); // Start Chasing Distance

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 10f); // Stop Chasing Distance
    }
}
