using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableAreas : MonoBehaviour
{
    bool canClimb;

    private void Update()
    {
        if (canClimb)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.instance.playerScript.isClimbing = !GameManager.instance.playerScript.isClimbing;
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

            //if(other.transform.position.y > transform.position.y + (transform.localScale.y / 2))
            //{
            //    Debug.Log("Player exited from the top!");
            //    Vector3 ledgePos = transform.position;
            //    Vector3 targetPos = new Vector3(ledgePos.x, ledgePos.y + 1f, ledgePos.z + 1f);

            //    StartCoroutine(GameManager.instance.playerScript.MoveToLedge(targetPos));
            //}
        }
    }
}
