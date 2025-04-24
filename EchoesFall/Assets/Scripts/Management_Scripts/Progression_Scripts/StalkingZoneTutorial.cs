using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkingZoneTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.statePause();
            GameManager.instance.menuActive = GameManager.instance.menuMisc;
            GameManager.instance.miscTitle.text = "Stalking Zones";
            GameManager.instance.miscText.text = "You can hide in stalking zones as long as you are crouched, they can be used to break the line of sight of your enemy for a time.";
            GameManager.instance.menuActive.SetActive(true);

            GameObject.Destroy(gameObject);
        }
    }
}
