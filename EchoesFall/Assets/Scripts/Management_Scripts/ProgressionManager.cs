using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    GameManager gm;
    [Header("---- Bools ----")]
    public bool isLeaderKilled;

    public enum GameState
    {
        Tutorial,
        MainStory
    }
    
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        if(Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
