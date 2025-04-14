using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("---- Bools ----")]
    public bool playerInRange;
    public bool canSeePlayer;
    public bool isSearching;

    [Header("---- Components ----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Animator anim;

    [Header("----- Stats -----")]
    [SerializeField] float HP;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    [Header("---- Movement Components ----")]
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] float angleToPlayer;
    [SerializeField] float searchTimer;

    [SerializeField] Transform headPos;
    [SerializeField] Transform[] waypoints;
    
    Vector3 playerDir;

    int currentWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        searchTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            playerDir = (GameManager.instance.player.transform.position + Vector3.up * .5f) - headPos.position;
            angleToPlayer = Vector3.Angle(playerDir, transform.forward);

            Debug.DrawRay(headPos.position, playerDir, Color.yellow);

            RaycastHit hit;
            if(Physics.Raycast(headPos.position, playerDir, out hit))
            {
                if(hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            if (canSeePlayer)
            {
                FollowPlayer();
            }
            else if(searchTimer == 0)
            {
                if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
                {
                    Patrol();
                }
            }
        }
        else if(searchTimer == 0)
        {
            if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                Patrol();
            }
        }
        else if(searchTimer > 0)
        {
            //Will implement later
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
    //Code for moving from point to point.
    void Patrol()
    {
        navAgent.stoppingDistance = 0;
        navAgent.speed = 2;

        if (waypoints.Length == 0)
        {
            return;
        }
        else
        {
            navAgent.destination = waypoints[currentWaypoint].position;
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
    //Code for following the player after spotting the player.
    void FollowPlayer()
    {
        navAgent.stoppingDistance = 2;
        navAgent.speed = 3.5f;

        navAgent.SetDestination(GameManager.instance.player.transform.position);
        faceTarget();
    }
    void SearchingForPlayer()
    {
        //Code for searching for the player after the player breaks line of sight.
    }
    //Facing the player
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    //Coroutine for a timer to stop SearchingForPlayer and return to Patrol

    //Attack Functions

    //Stat Tracking
}
