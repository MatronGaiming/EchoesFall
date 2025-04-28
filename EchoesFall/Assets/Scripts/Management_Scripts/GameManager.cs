using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---- Bools ----")]
    [Header("Game States")]
    public bool isPaused;
    public bool gameStart;

    [Header("Game Progression")]
    public bool isLeaderKilled;
    public GameObject chest;
    public GameObject enemyLeader;
    public GameObject levelExit;

    [Header("---- Assets ----")]
    [Header("Player")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerCam;
    public GameObject[] enemies;

    [Header("UI")]
    public GameObject eyeVisible;
    public GameObject eyeHidden;
    public GameObject dialogueFrame;
    public GameObject playerDamageQue;
    public GameObject objectiveIcon;
    public Image playerHPBar;
    public TMP_Text dialogueText;
    public TMP_Text miscTitle;
    public TMP_Text miscText;
    public TMP_Text potionText;
    public TMP_Text objectiveText;

    [Header("Managers")]
    public ProgressionManager progressManager;

    [Header("---- Floats/Ints ----")]
    float timeScaleOrig;

    [Header("---- Menu's ----")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] public GameObject menuPause;
    [SerializeField] public GameObject menuWin;
    [SerializeField] public GameObject menuLose;
    [SerializeField] public GameObject menuStart;
    [SerializeField] public GameObject menuMisc;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerCam = GameObject.Find("CameraPos");
        progressManager = GetComponent<ProgressionManager>();

        statePause();
        menuActive = menuMisc;
        miscTitle.text = "Game Start";
        miscText.text = "You've recently been captured on a mission gone wrong. Luckily you've unlocked your cell and can escape to finish your job. Collect your gear and assassinate your target!\n"
            +"\n" 
            + "Controls: WSAD to move around. C will allow you to crouch. Moving the scrollwheel will increase/decrease your move speed";
        menuActive.SetActive(true);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else
            {
                stateUnpause();
            }
        }

        PlayerVisibility();
        GameProgression();
    }

    //Paused and Unpaused States
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    //Win/Lose States
    public void YouWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }
    public void YouLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    //Visibility
    void PlayerVisibility()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool anyEnemyCanSeePlayer = false;

        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if(enemyController != null && enemyController.canSeePlayer)
            {
                anyEnemyCanSeePlayer = true;
                break;
            }
        }

        if (playerScript.inHiddenObject == true && playerScript.isCrouched == true && anyEnemyCanSeePlayer == false)
        {
            eyeHidden.SetActive(true);
            eyeVisible.SetActive(false);
        }
        else
        {
            eyeHidden.SetActive(false);
            eyeVisible.SetActive(true);
        }
    }

    //Progression
    void GameProgression()
    {
        if(playerScript.isGearCollected == false)
        {
            objectiveText.text = "Collect your gear";
            objectiveIcon.transform.position = chest.transform.position;

        }
        if(playerScript.isGearCollected == true)
        {
            objectiveText.text = "Kill your target!";
            objectiveIcon.transform.position = new Vector3(0, 0, 0);
        }
        if(enemyLeader == null && playerScript.isGearCollected == true)
        {
            isLeaderKilled = true;
            objectiveText.text = "Escape!";
            objectiveIcon.transform.position = levelExit.transform.position;
        }
    }

    //Player Dialogue
    public IEnumerator playerDialogue(string text)
    {
        dialogueFrame.SetActive(true);
        dialogueText.text = text;
        yield return new WaitForSeconds(1.5f);
        dialogueFrame.SetActive(false);
        dialogueText.text = "-----";
    }
}
