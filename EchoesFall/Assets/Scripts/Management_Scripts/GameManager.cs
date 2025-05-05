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
    public GameObject loadScreen;
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
    public TMP_Text lockpickText;
    public TMP_Text objectiveText;

    [Header("---- Scene Management ----")]
    [Header("Cave Scene")]
    Vector3 playerSpawn;
    public GameObject caveScene;
    Cave_PrisonTransition _caveScene;
    public Transform cavePlayerSpawn;
    [Header("Prison Scene")]
    public GameObject prisonScene;
    public Transform prisonPlayerSpawn;

    [Header("---- Floats/Ints ----")]
    float timeScaleOrig;

    [Header("---- Menu's ----")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] public GameObject menuPause;
    [SerializeField] public GameObject menuWin;
    [SerializeField] public GameObject menuLose;
    [SerializeField] public GameObject menuStart;
    [SerializeField] public GameObject menuMisc;

    [Header("---- Quests ----")]
    [Header("Tutorial")]
    public bool gameHasStarted;
    public bool combatTutorialStarted;
    public bool assassinationTutorialStarted;
    public bool stalkingZoneTutorialStarted;
    public bool climbingAreaTutorialStarted;
    public bool pickpocetTutorialStarted;
    public bool isKeyCollected;
    public int enemyCount;
    [Header("Main Quest")]
    public bool isLeaderKilled;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerCam = GameObject.Find("CameraPos");
        _caveScene = caveScene.GetComponent<Cave_PrisonTransition>();
        playerScript.controller.enabled = false;
        player.transform.position = cavePlayerSpawn.transform.position;
        playerScript.controller.enabled = true;

        statePause();
        gameHasStarted = true;
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
        objectiveIcon.SetActive(false);

        //Tutorials
        if(gameHasStarted == true)
        {
            menuActive = menuMisc;
            miscTitle.text = "Story So Far";
            miscText.text = "You've are an elite assassin who has been assigned to take out the Watch Commander! Ruthless and cunning, he uses the law to abuse those he believes lesser than himself.\n"
                + "\n"
                + "---- Controls ----"
                +"\n"
                + "Use WSAD to move around. Moving the scrollwheel will increase/decrease your move speed.\n"
                + "C will allow you to crouch.";
            menuActive.SetActive(true);
            gameHasStarted = false;
        }
        if(gameHasStarted == false && stalkingZoneTutorialStarted == true)
        {
            statePause();
            menuActive = menuMisc;
            miscTitle.text = "Stalking Zones";
            miscText.text = "You can hide in stalking zones as long as you are crouched, they can be used to break the line of sight of your enemy for a time.";
            menuActive.SetActive(true);
            stalkingZoneTutorialStarted = false;
        }
        if(gameHasStarted == false && climbingAreaTutorialStarted == true)
        {
            statePause();
            menuActive = menuMisc;
            miscTitle.text = "Climbing";
            miscText.text = "You can run up walls by pressing the SPACEBAR." 
                + "  In some areas of the wall where it's broken you can press the SPACEBAR again to climb.";
            menuActive.SetActive(true);
            climbingAreaTutorialStarted = false;
        }
        if(gameHasStarted == false && combatTutorialStarted == true)
        {
            statePause();
            menuActive = menuMisc;
            miscTitle.text = "Combat";
            miscText.text = "\n"
                + "You can fight back with your shortword by pressing the Left Mouse Button."
                + "\n"
                + "Since you are an assassin, you can Press F on unaware enemies to take them down quietly.";
            menuActive.SetActive(true);
            combatTutorialStarted = false;
        }
        //Objectives
    }

    //Player Dialogue
    public IEnumerator playerDialogue(string text)
    {
        dialogueFrame.SetActive(true);
        dialogueText.text = text;
        yield return new WaitForSeconds(2f);
        dialogueFrame.SetActive(false);
        dialogueText.text = "-----";
    }

    //Scene Management
    public IEnumerator SwitchToPrison()
    {
        
        loadScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        playerScript.isPlayerLocked = true;
        playerScript.controller.enabled = false;

        caveScene.SetActive(false);
        prisonScene.SetActive(true);
        player.transform.position = prisonPlayerSpawn.position;
        playerScript.controller.enabled = true;

        //yield return new WaitForSeconds(1.5f);
        playerScript.isPlayerLocked = false;
        loadScreen.SetActive(false);
               
    }
}
