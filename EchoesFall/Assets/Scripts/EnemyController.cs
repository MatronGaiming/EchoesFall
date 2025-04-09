using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("---- Bools ----")]
    public bool playerInRange;
    public bool isSearching;

    [Header("---- Components ----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Animator anim;

    [Header("----- Stats -----")]
    [SerializeField] float HP;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    [Header("---- Movement Components ----")]
    [SerializeField] Transform[] waypoints;

    int currentWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange)
        {
            FollowPlayer();
        }
        else if (isSearching)
        {
            SearchingForPlayer();
        }
        else
        {
            if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                Patrol();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
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
            navAgent.stoppingDistance = 0;
        }
    }
    
    //Movement Functions
    void Patrol()
    {
        //Code for moving from point to point.
        if(waypoints.Length == 0)
        {
            return;
        }
        else
        {
            navAgent.destination = waypoints[currentWaypoint].position;
            currentWaypoint = (currentWaypoint +1) % waypoints.Length;
        }
    }
    void FollowPlayer()
    {
        //Code for following the player after spotting the player.
    }
    void SearchingForPlayer()
    {
        //Code for searching for the player after the player breaks line of sight.
    }
    //Coroutine for a timer to stop SearchingForPlayer and return to Patrol

    //Attack Functions

    //Stat Tracking
}
