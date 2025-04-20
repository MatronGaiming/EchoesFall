using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkingZones : MonoBehaviour
{
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gm.playerScript.inHiddenObject = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gm.playerScript.inHiddenObject = false;
        }
    }
}
