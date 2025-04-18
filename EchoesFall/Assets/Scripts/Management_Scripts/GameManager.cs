using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---- Bools ----")]
    [Header("Game States")]
    public bool isPaused;
    public bool gameStart;

    [Header("Game Progression")]
    public bool isLeaderKilled;

    [Header("---- Assets ----")]
    [Header("Player")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerCam;
    public GameObject enemyLeader;
    public GameObject[] enemies;

    [Header("UI")]
    public GameObject eyeVisible;
    public GameObject eyeHidden;

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

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerCam = GameObject.Find("CameraPos");
        progressManager = GetComponent<ProgressionManager>();

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
        if(enemyLeader == null)
        {
            isLeaderKilled = true;
        }
    }
}
