using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    [Header("Wall Running")]
    public bool isWallRunning;
    public bool isNearWall;

    [Header("---- Player Stats ----")]
    [Header("Health")]
    [SerializeField] public float maxHP = 100;
    [SerializeField] float currentHP;
    [SerializeField] public int potionCount;

    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float speedStep;
    [SerializeField] float jumpSpeed;
    private Vector3 movement;

    [Header("---- Ground Check ----")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] LayerMask groundMask;

    [Header("---- Jumping/Physics ----")]
    [SerializeField] float gravity;
    [SerializeField] float jumpForce;
    private Vector3 velocity;

    [Header("---- Wall Running/Ledge Grabbing -----")]
    [SerializeField] float wallRunDuration;
    [SerializeField] float wallRunSpeed;

    [Header("---- Components ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animCtrlr;
    [SerializeField] GameObject shortSwordTrigger;
    [SerializeField] GameObject daggerTrigger;
    [SerializeField] GameObject shortSwordModel;
    [SerializeField] GameObject daggerModel;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        controller = GetComponent<CharacterController>();
        animCtrlr = GetComponent<Animator>();

        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerLocked == false)
        {
            Movement();
            if (isGearCollected == true)
            {
                Combat();
            }
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!isGrounded)
        {
            controller.Move(velocity * Time.deltaTime);
            velocity.y -= gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //isGrounded = controller.isGrounded;

        //Move Input
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        // Player Movement
        if (mouseScroll != 0)
        {
            moveSpeed += mouseScroll * speedStep;
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
        }

        if (isGrounded)
        {
            Vector3 move = new Vector3(movement.x, 0, movement.z);
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        // Rotate to face direction of movement
        if (movement != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = rot;
        }

        //Toggle Crouching
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouched = !isCrouched;
        }
        if (isCrouched)
        {
            controller.height = 1.0f;
            controller.center = new Vector3(0, 0.55f, 0);
        }
        else
        {
            controller.height = 1.75f;
            controller.center = new Vector3(0, 0.9f, 0);
        }

        //Wall Running
        RaycastHit wallHit;
        Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward, Color.yellow);

        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out wallHit, 1.5f))
        {
            if(wallHit.collider.CompareTag("Wall"))
            {
                isNearWall = true;
            }    
        }
        else
        {
            isNearWall = false;
        }
        if(Input.GetKeyDown(KeyCode.Space) && isNearWall)
        {
            StartCoroutine(WallRunCoroutine());
        }

        //Ledge Detection
        if (isWallRunning)
        {
            RaycastHit ledgeHit;
            Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;

            if(Physics.Raycast(rayOrigin, transform.forward, out ledgeHit, 1.0f))
            {
                if (ledgeHit.collider.CompareTag("Ledge"))
                {

                    LedgeGrabe(ledgeHit.point);
                }
            }
        }
        
    }
    void WallRunMovement()
    {
        Vector3 wallRunDirection = new Vector3(movement.x, 1, movement.y);
        controller.Move(wallRunDirection * wallRunSpeed * Time.deltaTime);
    }
    void LedgeGrabe(Vector3 ledgePoint)
    {
        isWallRunning = false;

        transform.position = ledgePoint;
    }

    void AnimationStates()
    {
        animCtrlr.SetBool("isCrouched", isCrouched);
    }

    // Combat Functions
    void Combat()
    {
        GameObject targetEnemy;
        if (Input.GetButtonDown("Fire1") && isAttacking == false)
        {
            shortSwordModel.SetActive(true);
            daggerModel.SetActive(false);

            isCrouched = false;
            isAttacking = true;
            
            int randomAnim = Random.Range(1, 3);
            animCtrlr.SetBool("shortSword1", randomAnim == 1);
            animCtrlr.SetBool("shortSword2", randomAnim == 2);

            if(randomAnim == 1 ||  randomAnim == 2)
            {
                StartCoroutine(BladeAnimationState(randomAnim));
            }
        }

        if(Input.GetKeyDown(KeyCode.F) && isAttacking == false)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2.0f))
            {
                EnemyController enemyController = hit.collider.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.canSeePlayer)
                {
                    targetEnemy = hit.collider.gameObject;

                    daggerModel.SetActive(true);
                    shortSwordModel.SetActive(false);

                    isAttacking = true;

                    animCtrlr.SetBool("assassin1", true);

                    StartCoroutine(AssassinAnimationState(targetEnemy));
                }
            }
        }
    }

    // Health and Damage Tracking
    public void TakeDamage(float damageAmount)
    {
        currentHP -= damageAmount;
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
            }
            else if(potionCount == 0)
            {
                StartCoroutine(gm.playerDialogue("I need to find some pointions"));
            }
        }
    }

    //IEnumerators
    IEnumerator BladeAnimationState(int index)
    {
        shortSwordTrigger.GetComponent<ShortSwordBase>().EnableCollider();
        yield return new WaitForSeconds(animCtrlr.GetCurrentAnimatorClipInfo(0).Length);
        shortSwordTrigger.GetComponent<ShortSwordBase>().DisableCollider();

        animCtrlr.SetBool("shortSword1", index == 1 && false);
        animCtrlr.SetBool("shortSword2", index == 2 && false);
        isAttacking = false;
    }
    IEnumerator AssassinAnimationState(GameObject enemy)
    {
        enemy.GetComponent<EnemyController>().beingAssassinated = true;
        daggerTrigger.GetComponent<DaggerBase>().EnableCollider();
        yield return new WaitForSeconds(animCtrlr.GetCurrentAnimatorClipInfo(0).Length);
        daggerTrigger.GetComponent<DaggerBase>().DisableCollider();
        enemy.GetComponent<EnemyController>().beingAssassinated = false;


        animCtrlr.SetBool("assassin1", false);
        isAttacking = false;
    }
    IEnumerator WallRunCoroutine()
    {
        isWallRunning = true;

        float elapsedTime = 0f;

        while(elapsedTime < wallRunDuration)
        {
            WallRunMovement();
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isWallRunning = false;
    }
}
