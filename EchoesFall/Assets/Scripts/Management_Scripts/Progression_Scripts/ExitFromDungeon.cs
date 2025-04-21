using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitFromDungeon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameManager.instance.playerScript.isGearCollected == false)
            {
                StartCoroutine(GameManager.instance.playerDialogue("'I need to collect my gear first!'"));
            }
        }
    }
}
