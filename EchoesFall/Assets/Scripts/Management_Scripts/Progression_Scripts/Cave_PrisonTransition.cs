using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave_PrisonTransition : MonoBehaviour
{
    GameManager gm;

    public bool playerInRange;
    public bool isLocked = true;
    public bool hasEnteredPrison;

    public int discovered = 0;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (isLocked == false)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hasEnteredPrison = true;
                    StartCoroutine(gm.SwitchToPrison());
                }
            }
            if(isLocked == true)
            {
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    isLocked = false;
                    StartCoroutine(gm.playerDialogue("It's unlocked!"));
                }
                if (gm.playerScript.hasPrisonKey == true && Input.GetKeyDown(KeyCode.E))
                {
                    isLocked = false;
                    StartCoroutine(gm.SwitchToPrison());
                }
                if (gm.playerScript.lockpickCount > 0 && Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(gm.playerDialogue("I need the key or I can pick the lock!"));
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if(discovered < 1)
            {
                StartCoroutine(gm.playerDialogue("This is the entrance!"));
                discovered += 1;
            }
            //StartCoroutine(gm.playerDialogue("This is the entrance!"));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
