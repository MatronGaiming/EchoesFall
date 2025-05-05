using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkingZoneTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.stalkingZoneTutorialStarted = true;
            GameObject.Destroy(gameObject);
        }
    }
}
