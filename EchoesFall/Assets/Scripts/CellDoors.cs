using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellDoors : MonoBehaviour
{
    [Header("---- Bools ----")]
    public bool isLocked;
    public bool isOpen;
    public bool isClosed;
    public bool playerInRange;

    [Header("---- Components ----")]
    PlayerController playerController;
    public Animator animCtrlr;
    public GameObject lockedImage;
    public GameObject unlockedImage;



    // Start is called before the first frame update
    void Start()
    {
        playerController = GameManager.instance.playerScript;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocked == true)
        {
            lockedImage.SetActive(true);
            unlockedImage.SetActive(false);
        }
        else
        {
            lockedImage.SetActive(false);
            unlockedImage.SetActive(true);
        }
        if (playerInRange == true && isLocked == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (playerController.lockpickCount > 0)
                {
                    playerController.lockpickCount -= 1;
                    GameManager.instance.lockpickText.text = playerController.lockpickCount.ToString();

                    isLocked = false;
                }
                else
                {
                    StartCoroutine(GameManager.instance.playerDialogue("I need to find more lockpicks"));
                }
            }           
        }
        if(playerInRange == true)
        {
            if (Input.GetKeyDown(KeyCode.E) && isLocked == false)
            {
                isOpen = !isOpen;
            }
            else if(Input.GetKeyDown(KeyCode.E) && isLocked == true)
            {
                StartCoroutine(GameManager.instance.playerDialogue("It's locked"));
            }
        }
        if(isOpen == true)
        {
            animCtrlr.SetBool("IsOpen", true);
            animCtrlr.SetBool("IsClosed", false);
        }
        else
        {
            animCtrlr.SetBool("IsOpen", false);
            animCtrlr.SetBool("IsClosed", true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
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
