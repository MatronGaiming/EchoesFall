using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.isLeaderKilled == true)
        {
            GameManager.instance.YouWin();
        }
    }
}
