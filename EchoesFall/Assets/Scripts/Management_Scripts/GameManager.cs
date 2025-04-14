using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---- Bools ----")]
    public bool isPaused;
    public bool gameStart;

    [Header("---- Assets ----")]
    [Header("Player")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerCam;

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
        
        if(playerScript.isVisible == true)
        {
            eyeVisible.SetActive(true);
            eyeHidden.SetActive(false);
        }
        else
        {
            eyeVisible.SetActive(false);
            eyeHidden.SetActive(true);
        }
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
        
    }
}
