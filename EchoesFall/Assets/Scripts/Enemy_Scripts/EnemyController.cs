using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, iDamageable
{
    [Header("---- Bools ----")]
    public bool playerInRange;
    public bool canSeePlayer;
    public bool isSearching;
    public bool beingAssassinated;

    [Header("---- Components ----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] GameObject hpFrame;
    [SerializeField] Image hpImage;
    [SerializeField] public GameObject assassinationImage;

    Color colorOrigin;

    [Header("----- Stats -----")]
    [Header("Health Stats")]
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;

    [Header("Attack Stats")]
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;
    [SerializeField] float distanceToPlayer;
    [SerializeField] GameObject weaponTrigger;
    [SerializeField] float nextAttackTime;

    float lastAttackTime;

    [Header("---- Movement Components ----")]
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] public float angleToPlayer;
    [SerializeField] float searchTimer;

    [SerializeField] Transform headPos;
    [SerializeField] Transform[] waypoints;
    
    Vector3 playerDir;

    int currentWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim.GetComponent<Animator>();
        searchTimer = 0f;

        colorOrigin = model.material.color;

        currentHP = maxHP;
        hpImage.fillAmount = currentHP / maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (beingAssassinated == false)
        {
            if (playerInRange)
            {
                playerDir = (GameManager.instance.player.transform.position + Vector3.up * .5f) - headPos.position;
                angleToPlayer = Vector3.Angle(playerDir, transform.forward);

                Debug.DrawRay(headPos.position, playerDir, Color.yellow);

                RaycastHit hit;
                if (Physics.Raycast(headPos.position, playerDir, out hit))
                {
                    if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
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
                    distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

                    if (distanceToPlayer <= attackRange && nextAttackTime <= 0)
                    {
                        StartCoroutine(EnemyAttackState());
                    }
                }
                else if (searchTimer == 0)
                {
                    if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
                    {
                        Patrol();
                    }
                }
            }
            else if (searchTimer == 0)
            {
                if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
                {
                    Patrol();
                }
            }
            else if (searchTimer > 0)
            {
                //Will implement later
            }
        }
        else
        {
            transform.rotation = GameManager.instance.player.transform.rotation;

            Vector3 offset = GameManager.instance.player.transform.forward * 1.25f;
            transform.position = GameManager.instance.player.transform.position + offset;

            anim.SetBool("BeingAssassinated", true);
            StartCoroutine(BeingAssassinated());
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

        if (waypoints.Length == 0 || waypoints == null)
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
        navAgent.stoppingDistance = 2.5f;
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
    IEnumerator EnemyAttackState()
    {
        //lastAttackTime = Time.time;
        anim.SetBool("Attack1", true);

        weaponTrigger.GetComponent<EnemySwordBase>().EnableCollider();
        //yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            // normalizedTime is relative to the clip length; 1.0f signifies completion.
            return stateInfo.normalizedTime >= 1.0f;
        });
        weaponTrigger.GetComponent<EnemySwordBase>().DisableCollider();

        anim.SetBool("Attack1", false);

        float randomAttackDelay = Random.Range(1.0f, 10.0f);
        nextAttackTime -= Time.deltaTime;
    }

    //Stat Tracking
    public void TakeDamage(float damageAmount)
    {
        currentHP -= damageAmount;
        canSeePlayer = true;
        navAgent.SetDestination(GameManager.instance.player.transform.position);
        hpImage.fillAmount = currentHP / maxHP;

        StartCoroutine(FlashRed());

        if(currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrigin;
    }
    IEnumerator BeingAssassinated()
    {

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject);
    }
}
