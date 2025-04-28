using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ledges : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameManager.instance.playerScript.isClimbing == true)
            {
                Vector3 ledgePos = transform.position;
                Vector3 targetPos = new Vector3(GameManager.instance.player.transform.position.x, ledgePos.y + 0.5f, ledgePos.z + 1f);

                StartCoroutine(GameManager.instance.playerScript.MoveToLedge(targetPos));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
