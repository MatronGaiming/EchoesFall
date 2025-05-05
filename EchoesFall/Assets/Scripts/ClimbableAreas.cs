using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableAreas : MonoBehaviour
{
    public bool canClimb;

    private void Update()
    {
        if (canClimb)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.instance.playerScript.isClimbing = !GameManager.instance.playerScript.isClimbing;
                GameManager.instance.playerScript.currentClimbableObject = gameObject;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canClimb = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canClimb = false;
            GameManager.instance.playerScript.isClimbing = false;
        }
    }
}
