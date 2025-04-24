using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainExit : MonoBehaviour
{
    public GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(enemy == null)
            {
                GameManager.instance.YouWin();
            }
            else
            {
                StartCoroutine(GameManager.instance.playerDialogue("I need to kill my target first."));
            }
        }
    }
}
