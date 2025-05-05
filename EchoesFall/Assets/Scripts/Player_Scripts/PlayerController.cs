using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    GameManager gm;

    [Header("---- Bools ----")]
    [Header("Grounded Bools")]
    public bool isGrounded;
    public bool isCrouched;
    [Header("Visibility")]
    public bool isVisible;
    public bool inHiddenObject;
    public bool isSeen;
    [Header("Player States")]
    public bool isAttacking;
    public bool isGearCollected;
    public bool isPlayerLocked;
    [Header("Wall Physics")]
    public bool isWallRunning;
    public bool isNearWall;
    public bool isClimbing;
    public bool isLedgeMoving;
    [Header("Assassination")]
    public bool enemyInRange;
    [Header("Progression Objects")]
    public bool hasPrisonKey;

    [Header("---- Player Stats ----")]
    [Header("Health")]
    [SerializeField] public float maxHP = 100;
    [SerializeField] float currentHP;
    public int potionCount;
    public int lockpickCount;

    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float speedStep;
    [SerializeField] float rotationSpeed;
    private Vector3 movement;

    [Header("---- Jumping/Physics ----")]
    [SerializeField] float gravity;
    [SerializeField] float jumpForce;
    private Vector3 velocity;

    [Header("---- Wall Running/Climbing -----")]
    [SerializeField] float wallRunTimer;
    [SerializeField] float wallRunDuration;
    [SerializeField] float wallRunSpeed;
    private Bounds climbingBounds;
    public GameObject currentClimbableObject;
    private Vector3 ledgePos;

    [Header("---- Components ----")]
    [SerializeField] public CharacterController controller;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Animator animCtrlr;
    [SerializeField] GameObject shortSwordTrigger;
    [SerializeField] GameObject daggerTrigger;
    [SerializeField] GameObject shortSwordModel;
    [SerializeField] GameObject daggerModel;
    [SerializeField] Transform target;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Collider[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        controller = GetComponent<CharacterController>();
        //animCtrlr = GetComponent<Animator>();

        currentHP = maxHP;
        gm.playerHPBar.fillAmount = currentHP / maxHP;
        GameManager.instance.lockpickText.text = lockpickCount.ToString();
        GameManager.instance.potionText.text = potionCount.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerLocked == false)
        {
            Movement();
            Combat();           
            AnimationStates();
            Healing();
        }

        if (currentHP <= 0)
        {
            gm.YouLose();
        }
    }

    //Movement Functions
    private void Movement()
    {
        //Detect Ground
        RaycastHit hit;
        Vector3 startPoint = transform.position + Vector3.up * controller.radius;
        Vector3 endPoint = transform.position + Vector3.up * controller.height;
        Vector3 direction = Vector3.down;

        isGrounded = Physics.CapsuleCast(startPoint, endPoint, controller.radius, direction, out hit, 0.2f);

        if (isGrounded == false || isWallRunning == false || isClimbing == false)
        {
            controller.Move(velocity * Time.deltaTime);
            velocity.y -= gravity * Time.deltaTime;
        }

        //Move Input
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        // Player Ground Movement
        if (mouseScroll != 0)
        {
            moveSpeed += mouseScroll * speedStep;
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
        }

        if (isGrounded)
        {
            Vector3 move = new Vector3(movement.x, 0, movement.z);
            controller.Move(move * moveSpeed * Time.deltaTime);

            if (movement != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = rot;
            }

            wallRunTimer = 0;

            //Toggle Crouch
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCrouched = !isCrouched;
            }
            if (isCrouched)
            {
                maxSpeed = 5;
                controller.height = 1.0f;
                controller.center = new Vector3(0, 0.55f, 0);
            }
            else
            {
                maxSpeed = 7;
                controller.height = 1.75f;
                controller.center = new Vector3(0, 0.90f, 0);
            }
        }

        //Wall Movement
        if (isWallRunning)
        {
            velocity.y = 0;
            wallRunTimer += Time.deltaTime;

            Vector3 wallRunDirection = new Vector3(movement.x, 1, movement.z);
            controller.Move(wallRunDirection * wallRunSpeed * Time.deltaTime);
        }

        if (isClimbing)
        {
            isWallRunning = false;
            velocity.y = 0;

            Transform climbTransform = currentClimbableObject.transform;

            //Vector3 move = new Vector3(movement.x, movement.z, 0);
            Vector3 move = movement.x * climbTransform.right + movement.z * climbTransform.up;
            Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

            targetPos.x = Mathf.Clamp(targetPos.x, climbingBounds.min.x, climbingBounds.max.x);
            targetPos.y = Mathf.Clamp(targetPos.y, climbingBounds.min.y, climbingBounds.max.y);
            targetPos.z = Mathf.Clamp(targetPos.z, climbingBounds.min.z, climbingBounds.max.z);

            controller.Move(targetPos - transform.position);
        }

        WallPhysics();
    }

    void WallPhysics()
    {
        RaycastHit wallHit;

        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out wallHit, 1f))
        {
            if (wallHit.collider.CompareTag("Wall"))
            {
                isNearWall = true;
            }
            else
            {
                isNearWall = false;
                isClimbing = false;
                isWallRunning = false;
            }
        }
        else
        {
            isNearWall = false;
            isClimbing = false;
            isWallRunning = false;
        }
        if (isNearWall)
        {
            if (Input.GetKey(KeyCode.Space) && wallRunTimer < wallRunDuration)
            {
                //rb.useGravity = false;
                isWallRunning = true;
            }
            else
            {
                isWallRunning = false;
                //rb.useGravity = true;
            }
        }
    }
    void AnimationStates()
    {
        animCtrlr.SetBool("isCrouched", isCrouched);
    }

    // Combat Functions
    void Combat()
    {
        //GameObject targetEnemy;

        //Ray ray = new Ray(transform.position + Vector3.up * 1f, transform.forward);
        //RaycastHit hit;

        //float radius = 1.0f;
        //Gizmos.color = Color.green;
        //Vector3 origin = transform.position + Vector3.up * 1f;
        //Vector3 direction = transform.forward;
        //float maxDistance = 2f;

        //if (Physics.SphereCast(origin, radius, direction, out hit, maxDistance))
        //{
        //    EnemyController enemyController = hit.collider.GetComponent<EnemyController>();

        //    if(enemyController != null && enemyController.angleToPlayer >= 100)
        //    {
        //        enemyInRange = true;
        //        enemyController.assassinationImage.SetActive(true);
        //    }
        //    else
        //    {
        //        enemyInRange = false;
        //        //enemyController.assassinationImage.SetActive(false);
        //        enemyController = null;
        //    }
        //    //enemyInRange = true;
        //}
        //else
        //{
        //    enemyInRange = false;
        //}
        Transform target = GetNearestTarget();

        if (target != null)
        {
            enemyInRange = true;
            EnemyController enemyController = target.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.angleToPlayer >= 100)
            {
                enemyController.assassinationImage.SetActive(true);
            }
        }
        else
        {
            enemyInRange = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            shortSwordModel.SetActive(true);
            daggerModel.SetActive(false);

            isCrouched = false;
            //isAttacking = true;
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            int randomAnim = Random.Range(1, 3);
            animCtrlr.SetBool("shortSword1", randomAnim == 1);
            animCtrlr.SetBool("shortSword2", randomAnim == 2);

            if(randomAnim == 1 ||  randomAnim == 2)
            {
                StartCoroutine(BladeAnimationState(randomAnim));
            }
        }

        if(Input.GetKeyDown(KeyCode.F) && enemyInRange == true)
        {           
            if (enemyInRange == true)
            {
                GameObject targetEnemy = target.gameObject;

                daggerModel.SetActive(true);
                shortSwordModel.SetActive(false);

                //isAttacking = true;

                animCtrlr.SetBool("assassin1", true);

                StartCoroutine(AssassinAnimationState(targetEnemy));
            }           
        }
    }

    Transform GetNearestTarget()
    {
        Vector3 detectionOrigin = transform.position + Vector3.up;
        float detectionRadius = 2.5f;
        colliders = Physics.OverlapSphere(detectionOrigin, detectionRadius, enemyLayer);

        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach(Collider col in colliders)
        {
            if(!(col is CapsuleCollider))
            {
                continue;
            }
            EnemyController enemy = col.GetComponent<EnemyController>();
            if(enemy != null)
            {
                Vector3 directionToEnemy = enemy.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToEnemy);

                if(angle <= 45f)
                {
                    float distance = directionToEnemy.magnitude;
                    if(distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestTarget = enemy.transform;
                    }
                }
            }
        }
        return bestTarget;
    }

    // Health and Damage Tracking
    public void TakeDamage(float damageAmount)
    {
        currentHP -= damageAmount;
        gm.playerHPBar.fillAmount = currentHP / maxHP;
        StartCoroutine(flashScreen());
    }
    void Healing()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(potionCount > 0 && currentHP < maxHP)
            {
                potionCount -= 1;
                GameManager.instance.potionText.text = potionCount.ToString();
                currentHP = maxHP;
                gm.playerHPBar.fillAmount = currentHP / maxHP;
            }
            else if(potionCount == 0)
            {
                StartCoroutine(gm.playerDialogue("I need to find some potions"));
            }
        }
    }

    //IEnumerators
    IEnumerator BladeAnimationState(int index)
    {
        //isAttacking = true;
        shortSwordTrigger.GetComponent<ShortSwordBase>().EnableCollider();
        //yield return new WaitForSeconds(animCtrlr.GetCurrentAnimatorClipInfo(0).Length);

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = animCtrlr.GetCurrentAnimatorStateInfo(0);
            // normalizedTime is relative to the clip length; 1.0f signifies completion.
            return stateInfo.normalizedTime >= 1.0f;
        });

        shortSwordTrigger.GetComponent<ShortSwordBase>().DisableCollider();
        //isAttacking = false;

        animCtrlr.SetBool("shortSword1", index == 1 && false);
        animCtrlr.SetBool("shortSword2", index == 2 && false);
    }
    IEnumerator AssassinAnimationState(GameObject enemy)
    {
        //isAttacking = true;
        enemy.GetComponent<EnemyController>().beingAssassinated = true;
        if (target != null)
        {
            // Calculate the direction and smoothly rotate
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        daggerTrigger.GetComponent<DaggerBase>().EnableCollider();
        //yield return new WaitForSeconds(animCtrlr.GetCurrentAnimatorClipInfo(0).Length);

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = animCtrlr.GetCurrentAnimatorStateInfo(0);
            // normalizedTime is relative to the clip length; 1.0f signifies completion.
            return stateInfo.normalizedTime >= 1.0f;
        });
        daggerTrigger.GetComponent<DaggerBase>().DisableCollider();
        //enemy.GetComponent<EnemyController>().beingAssassinated = false;

        //isAttacking = false;
        animCtrlr.SetBool("assassin1", false);
    }
    public IEnumerator MoveToLedge(Vector3 targetPos)
    {
        isLedgeMoving = true;
        controller.enabled = false;
        velocity.y = 0;

        //Move player into desired position above the ledge and forward a little bit so the player doesn't fall down.
        float elapsedTime = 0f;
        float moveDuration = 0.5f;

        Vector3 startingPos = transform.position;

        while(elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        controller.enabled = true;
        isLedgeMoving = false;

        yield return null;
    }
    IEnumerator flashScreen()
    {
        gm.playerDamageQue.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gm.playerDamageQue.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            climbingBounds = other.bounds;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            climbingBounds = new Bounds();
        }
    }
}
