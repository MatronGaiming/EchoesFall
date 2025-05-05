using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbZoneTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.climbingAreaTutorialStarted = true;
            Destroy(gameObject);
        }
    }
}
