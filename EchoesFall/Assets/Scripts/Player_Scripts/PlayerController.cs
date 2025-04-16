using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, iDamageable
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

    [Header("---- Player Stats ----")]
    [Header("Health")]
    [SerializeField] public float maxHP = 100;
    [SerializeField] float currentHP;

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
        controller = GetComponent<CharacterController>();
        animCtrlr = GetComponent<Animator>();

        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Combat();
        AnimationStates();
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
            daggerModel.SetActive(true);
            shortSwordModel.SetActive(false);

            isAttacking = true;

            animCtrlr.SetBool("assassin1", true);

            StartCoroutine(AssassinAnimationState());
        }
    }

    // Health and Damage Tracking
    public void TakeDamage(float damageAmount)
    {
        currentHP -= damageAmount;

        if (currentHP <= 0)
        {
            gm.YouLose();
        }
    }

    //Detection
    void Detection()
    {

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
    IEnumerator AssassinAnimationState()
    {
        daggerTrigger.GetComponent<DaggerBase>().EnableCollider();
        yield return new WaitForSeconds(animCtrlr.GetCurrentAnimatorClipInfo(0).Length);
        daggerTrigger.GetComponent<DaggerBase>().DisableCollider();

        animCtrlr.SetBool("assassin1", false);
        isAttacking = false;
    }
}
