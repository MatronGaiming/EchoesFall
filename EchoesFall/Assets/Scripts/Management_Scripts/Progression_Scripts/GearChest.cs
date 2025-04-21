using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearChest : MonoBehaviour
{
    public GameObject enemy;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemy == null)
            {
                GameManager.instance.playerScript.isGearCollected = true;
                GameManager.instance.statePause();
                GameManager.instance.menuActive = GameManager.instance.menuMisc;
                GameManager.instance.miscTitle.text = "Combat Tutorial";
                GameManager.instance.miscText.text = "Press Left Mouse Button to swing your short sword.\n" + "When an enemy is not alerted, you can press F to assassinate the enemy.";
                GameManager.instance.menuActive.SetActive(true);

            }
            else
            {
                StartCoroutine(GameManager.instance.playerDialogue("I need to collect the key first. The Captain of the Guard should have it."));
            }
        }
    }
}
