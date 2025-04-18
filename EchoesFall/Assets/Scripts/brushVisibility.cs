using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brushVisibility : MonoBehaviour
{
    public GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gm.playerScript.isVisible = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gm.playerScript.isCrouched == true)
            {
                gm.playerScript.isVisible = false;
            }
            else if (gm.playerScript.isCrouched == false)
            {
                gm.playerScript.isVisible = true;
            }
        }
    }
}
