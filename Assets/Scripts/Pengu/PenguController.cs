using UnityEngine;
using System.Collections;
using TMPro;

public class PenguController : MonoBehaviour
{
    [Header("Speed settings")]
    public float moveSpeed = 5f;
    public float gravity = -15f;
    public float rotationSpeed = 50f;
    public float slideSpeed = 15f;
    public float slideDuration = 1.0f;
    public float slideHeight = 1.0f;
    public float pushPower = 5.0f;

    [Header("Key Bindings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.LeftShift;
    public string jumpButton = "Jump";
    // public string slideButton = "Fire1";

    [Header("Ice Settings")]
    public float iceAcceleration = 5f;
    public float iceMaxSpeed = 15f;
    public LayerMask iceLayer;

    [Header("Jump Settings")]
    public float airControlSpeed = 4f;
    public float jumpForce = 10f;
    public float doubleJumpForce = 6f;
    private float groundedTimer = 0f;
    public float groundedGraceTime = 0.15f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;

    [Header("Slide Settings")]
    public int maxSlideCharges = 5;
    public bool limitSlideCharges = false;
    public float slideCooldownTime = 5f;
    private int currentSlideCharges;
    private float slideCooldownTimer = 0f;
    private bool isOnCooldown = false; 
    

    private bool canDoubleJump = false;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private Vector3 airVelocity;
    private bool isSliding = false;
    private bool isOnIce = false;
    private float currentSpeed;
    private float originalHeight;
    private Vector3 originalCenter;

    private bool isJumping = false;
    private bool wasGrounded = true;
    private float airTimer = 0f;
    private float minAirTime = 0.15f;
    private float maxUpVelocity = 0f;

    public TextMeshProUGUI countText;
    public static int count { get; private set; }

    private bool isDead = false;
    private bool isInvincible = false;
    public bool IsInvincible => isInvincible;

    [Header("Respawn Invincibility")]
    public float invincibilityDuration = 3f;
    public float blinkInterval = 0.15f;

    private GameObject checkpoint;
    private GameObject currentFish;
    private Transform fishTarget;
    private bool isInteracting = false;

    private bool isRotatingToFish = false;
    public float interactRotationSpeed = 20f;

    private int pendingFishScore = 0;

    private bool cancelSlide = false;

    private bool IsFalling = false;


    public bool IsOnCooldown => isOnCooldown;
    public float SlideCooldownTimer => slideCooldownTimer;
    public int CurrentSlideCharges => currentSlideCharges;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true; 

        originalHeight = controller.height;
        originalCenter = controller.center;
        currentSpeed = moveSpeed;
        currentSlideCharges = maxSlideCharges;

        count = 0;
    }

    void Update()
    {
        RaycastHit hit;
        isOnIce = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 2f, iceLayer);

        IsFalling = !controller.isGrounded && velocity.y < -2f && !isJumping;

        if (IsFalling && airVelocity == Vector3.zero && !isJumping)
        {
            float vertical = Input.GetAxis("Vertical");
            airVelocity = transform.forward * vertical*currentSpeed;
        }

        HandleInput();
    }

    void OnAnimatorMove()
    {
        
        // IsFalling = !controller.isGrounded && velocity.y < -2f && !isJumping;
        // Debug.Log("Pengu is Falling");
        animator.SetBool("IsFalling", IsFalling);

        if (IsFalling)
        {
            Debug.Log("Pengu is Falling");
        }

        if (isSliding) return;


        bool grounded = controller.isGrounded;

        if (!grounded)
            airTimer += Time.deltaTime;

        if (grounded && !wasGrounded && airTimer >= minAirTime)
        {
            isJumping = false;
            canDoubleJump = false;
            IsFalling = false;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            airVelocity = Vector3.zero;
            airTimer = 0f;
        }

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 rootDelta = animator.deltaPosition;
        rootDelta.y = 0f;
        float vertical1 = Input.GetAxis("Vertical");
        float direction = vertical1 < -0.1f ? -1f : 1f ;
        Vector3 scaledDelta = new Vector3(rootDelta.x * currentSpeed * direction, 0f, rootDelta.z * currentSpeed * direction);

        Vector3 xzMove;
        if (grounded && !isJumping)
        {
            xzMove = scaledDelta;
            if (!Input.GetKey(jumpKey))
            {
                velocity.y = -2f;
            }
        }
        else
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector3 airInput = transform.forward * vertical + transform.right * horizontal;
            if (airInput.magnitude > 0.1f)
                airVelocity = airInput * currentSpeed * (airControlSpeed / 10f);
            else
                airVelocity = Vector3.Lerp(airVelocity, Vector3.zero, Time.deltaTime * 5f);

            xzMove = airVelocity * Time.deltaTime;
        }

        Vector3 finalMove = xzMove + new Vector3(0f, velocity.y * Time.deltaTime, 0f);
        controller.Move(finalMove);

        wasGrounded = grounded;
    }

   
    void HandleInput()
    {
        if (isInteracting || isRotatingToFish) return;

        if (!isSliding)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) > 0.1f)
                transform.Rotate(0f, horizontal * rotationSpeed * Time.deltaTime, 0f);

            if (isOnIce)
            {
                currentSpeed += iceAcceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, iceMaxSpeed);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * 5f);
            }

            if ((Input.GetKeyDown(jumpKey)) && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                maxUpVelocity = velocity.y;
                airVelocity = transform.forward * vertical * currentSpeed;
                isJumping = true;
                canDoubleJump = true;
                animator.SetBool("IsJumping", true);
                airTimer = 0f;
            }
            else if ((Input.GetKeyDown(jumpKey)) && IsFalling && !isJumping)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                isJumping = true;
                canDoubleJump = true;
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsJumping", true);

                // Force the jump state to replay from the beginning
                animator.Play("Jump", 0, 0f);
                airTimer = 0f;

            }
            else if ((Input.GetKeyDown(jumpKey)) && canDoubleJump  && !controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(doubleJumpForce * -2f * gravity);
                maxUpVelocity = velocity.y;
                canDoubleJump = false;

                // Force the jump state to replay from the beginning
                animator.Play("Jump", 0, 0f);

            }


            animator.SetFloat("Speed", Mathf.Abs(vertical));

            //made this fix to hande D 
            // animator.SetFloat("Speed", vertical);
        }

        if (isOnCooldown)
        {
            slideCooldownTimer -= Time.deltaTime;
            if (slideCooldownTimer <= 0f)
            {
                slideCooldownTimer = 0f;
                isOnCooldown = false;
            }
        }
        
        bool chargesAvailable = !limitSlideCharges || currentSlideCharges > 0;

        if (Input.GetKeyDown(slideKey) && controller.isGrounded && !isSliding && !isOnCooldown && chargesAvailable)
        {
            if (limitSlideCharges)
                currentSlideCharges--;
            slideCooldownTimer = slideCooldownTime;
            isOnCooldown = true;
            StartCoroutine(SlideCoroutine());
        }
    }

    void OnCaughtByEnemy()
    {
        if (isDead || isInvincible) return;
        isDead = true;

        isInteracting = false;
        isRotatingToFish = false;
        isSliding = false;
        isJumping = false;
        canDoubleJump = false;
        StopAllCoroutines();

        animator.SetBool("IsJumping", false);
        animator.SetBool("IsSliding", false);
        animator.SetTrigger("Die");

        StartCoroutine(RespawnDelay());
    }
    IEnumerator SlideCoroutine()
    {
        isSliding = true;
        cancelSlide = false;
        animator.SetBool("IsSliding", true);

        controller.height = slideHeight;
        controller.center = new Vector3(originalCenter.x, originalCenter.y - (originalHeight - slideHeight) / 2f, originalCenter.z);
        velocity.y = -2f;
        float timer = 0f;

        while (timer < slideDuration && !cancelSlide)
        {
            if (controller.isGrounded)
                velocity.y = -2f;
            else
                velocity.y += gravity * Time.deltaTime;

            Vector3 slideMove = transform.forward * slideSpeed * Time.deltaTime;
            slideMove.y = velocity.y * Time.deltaTime;

            controller.Move(slideMove);
            timer += Time.deltaTime;
            yield return null;
        }

        controller.height = originalHeight;
        controller.center = originalCenter;
        animator.SetBool("IsSliding", false);
        isSliding = false;
        cancelSlide = false;
    }

    void SetCountText()
    {
        countText.text = "Score: " + count.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isInteracting || isRotatingToFish) return;

        if (other.gameObject.CompareTag("Collectable"))
        {
            StartFishInteraction(other.gameObject, scoreValue: 1);
        }
        else if (other.gameObject.CompareTag("BonusFish"))
        {
            StartFishInteraction(other.gameObject, scoreValue: 2);
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            checkpoint = other.gameObject;
        }
    }

    void StartFishInteraction(GameObject fish, int scoreValue)
    {
        if (isSliding)
            cancelSlide = true;
        
        fishTarget = fish.transform;
        currentFish = fish;
        pendingFishScore = scoreValue;
        isInteracting = true;
        isRotatingToFish = true;

        currentSpeed = 0f;
        velocity = Vector3.zero;
        airVelocity = Vector3.zero;
        animator.SetFloat("Speed", 0f);
        // StartCoroutine(RotateAndInteract());

        InteractWithFish();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (isInteracting && fishTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, fishTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, fishTarget.rotation);
        }
    }

    public void FinishInteraction()
    {
        if (currentFish != null)
        {
            currentFish.SetActive(false);
            count += pendingFishScore;
            SetCountText();
        }

        isInteracting = false;
        isRotatingToFish = false;
        fishTarget = null;
        currentFish = null;
        pendingFishScore = 0;
    }

    public void CollectFish()
    {
        if (currentFish != null)
        {
            currentFish.SetActive(false);
            count += 1;
            SetCountText();
            currentFish = null;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy") && !isDead && !isInvincible)
        {
            isDead = true;

            // Cancel any in-progress states so death isn't blocked
            isInteracting = false;
            isRotatingToFish = false;
            isSliding = false;
            isJumping = false;
            canDoubleJump = false;
            StopAllCoroutines();

            animator.SetBool("IsJumping", false);
            animator.SetBool("IsSliding", false);

            animator.SetTrigger("Die");

            StartCoroutine(RespawnDelay());
        }

        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        body.linearVelocity = pushDir * pushPower;
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(2f);
        RespawnToCheckpoint();
        animator.Play("Idle");
        isDead = false;
        StartCoroutine(InvincibilityBlink());
    }

    IEnumerator InvincibilityBlink()
    {
        isInvincible = true;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            bool visible = Mathf.FloorToInt(elapsed / blinkInterval) % 2 == 0;
            foreach (Renderer r in renderers)
                r.enabled = visible;

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (Renderer r in renderers)
            r.enabled = true;

        isInvincible = false;
    }

    public void RespawnToCheckpoint()
    {
        StopAllCoroutines();
        isInteracting = false;
        isRotatingToFish = false;
        isSliding = false;
        fishTarget = null;
        currentFish = null;

        velocity = Vector3.zero;
        airVelocity = Vector3.zero;

        // spawn slightly above ground
        Vector3 spawnPos = checkpoint.transform.position + Vector3.up * 2f;

        // enable and disable control instead of enabling and disabling the entire game object
        controller.enabled = false;
        transform.position = spawnPos;

        // Snap to ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f))
        {
            transform.position = hit.point;
        }

        controller.enabled = true;

    }

    void InteractWithFish()
    {

        if (fishTarget == null)
        {
            animator.SetTrigger("Interact");
            isRotatingToFish = false;
            return;
        }

        float angleDiff = Vector3.Angle(transform.forward, (fishTarget.position - transform.position).normalized);
        if (angleDiff > 90f)
        {
            if (currentFish!=null)
            {
                currentFish.SetActive(false);
                count += pendingFishScore;
                SetCountText();
            }
            isInteracting = false;
            isRotatingToFish = false;
            currentFish = null;
            fishTarget = null;
            pendingFishScore = 0;
            return;
        }

        // Quaternion targetRotation = Quaternion.LookRotation(directionToFish);

        // while (rotateTimer < rotateDuration && fishTarget !=null)
        // {
        //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, interactRotationSpeed * 100f * Time.deltaTime);
        //     rotateTimer += Time.deltaTime;
        //     yield return null;
        // }

        isRotatingToFish = false;
        animator.SetTrigger("Interact");
    }

    // IEnumerator RotateAndInteract()
    // {
    //     float rotateTimer = 0f;
    //     float rotateDuration = 0.15f;

    //     if (fishTarget == null)
    //     {
    //         animator.SetTrigger("Interact");
    //         isRotatingToFish = false;
    //         yield break;
    //     }

    //     Vector3 directionToFish = (fishTarget.position - transform.position).normalized;
    //     directionToFish.y = 0f;

    //     float angleDiff = Vector3.Angle(transform.forward, (fishTarget.position - transform.position).normalized);
    //     if (angleDiff > 90f)
    //     {
    //         if (currentFish!=null)
    //         {
    //             currentFish.SetActive(false);
    //             count += pendingFishScore;
    //             SetCountText();
    //         }
    //         isInteracting = false;
    //         isRotatingToFish = false;
    //         currentFish = null;
    //         fishTarget = null;
    //         pendingFishScore = 0;
    //         yield break;
    //     }

    //     // Quaternion targetRotation = Quaternion.LookRotation(directionToFish);

    //     // while (rotateTimer < rotateDuration && fishTarget !=null)
    //     // {
    //     //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, interactRotationSpeed * 100f * Time.deltaTime);
    //     //     rotateTimer += Time.deltaTime;
    //     //     yield return null;
    //     // }

    //     animator.SetTrigger("Interact");
    //     isRotatingToFish = false;
    // }
}