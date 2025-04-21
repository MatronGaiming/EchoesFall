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
    public bool isVisible;
    public bool inHiddenObject;
    public bool isSeen;
    public bool isAttacking;
    public bool isGearCollected;

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
    [SerializeField] float sprintSpeed;
    private Vector3 movement;

    [Header("---- Jumping/Physics ----")]
    [SerializeField] float gravity;
    [SerializeField] float jumpForce;
    private Vector3 velocity;

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
        Movement();
        if(isGearCollected == true)
        {
            Combat();
        }
        AnimationStates();

        if (currentHP <= 0)
        {
            gm.YouLose();
        }
    }

    //Movement Functions
    private void Movement()
    {
        if (!isGrounded)
        {
            controller.Move(velocity * Time.deltaTime);
            velocity.y -= gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
        }
        isGrounded = controller.isGrounded;

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

        Vector3 move = new Vector3(movement.x, 0, movement.z);

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

        if (Input.GetButton("Jump"))
        {
            isCrouched = false;
            controller.Move(move * sprintSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
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
        if(potionCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                potionCount -= 1;
                currentHP = maxHP;
            }
        }
        else
        {
            
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
}
