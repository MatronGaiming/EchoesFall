using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitFromDungeon : MonoBehaviour
{
    public GameObject enemyLeader;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(enemyLeader != null)
            {
                StartCoroutine(GameManager.instance.playerDialogue("This is my way out but I need to take out my target!'"));
            }
            else
            {
                GameManager.instance.YouWin();
            }
        }
    }
}
