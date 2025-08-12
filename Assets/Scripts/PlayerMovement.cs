using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
   [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private LayerMask pushableLayer;
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private GameObject afterimagePrefab;
    [SerializeField] private float afterimageInterval = 0.04f;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 1.2f, 0);
    [SerializeField] private GameObject healthInteractPrompt; 
    [SerializeField] private Vector3 healthPromptOffset = new Vector3(0, 1.0f, 0);
    [SerializeField] private GameObject keyInteractPrompt;
    [SerializeField] private Vector3 keyPromptOffset = new Vector3(0, 1.0f, 0);
    [SerializeField] private ParticleSystem dustParticles; 
    [SerializeField] private float walkEmissionRate = 10f;
    [SerializeField] private float dashEmissionRate = 30f;
    [SerializeField] private float iceSpeedMultiplier = 1.4f;  // Increased speed when sliding
    [SerializeField] private float iceDrag = 0.2f;           // Lower = more slippery
    [SerializeField] private float iceAngularDrag = 0.05f;
    private bool isIceFloor = false;
    private ParticleSystem.EmissionModule dustEmission;
    float footTimer;
    [SerializeField] float footInterval = 0.28f; // tweak per feel

    private Transform keyPromptTarget;
    private KeyPickup currentKeyInRange;
    private Transform promptTarget;
    private Transform healthPromptTarget;
    private HealthPickup currentHealthInRange;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Vector2 lastDirection = Vector2.down;
    private bool isKnockedBack;
    private bool wasMovingLastFrame = false;

    public PlayerStats playerStats;
    private Chest currentChestInRange;
    private bool isDashing = false;
    private float lastDashTime = -10f;
    private CameraShake _cameraShake;
   private void Awake()
    {
        if (FindObjectsOfType<PlayerMovement>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _cameraShake = Camera.main?.GetComponent<CameraShake>();

        if (dustParticles != null)
        {
            dustEmission = dustParticles.emission;
            dustEmission.rateOverTime = 0f;
        }

        if (RoomManager.Instance != null)
        {
            HandleEnvironmentChanged(RoomManager.Instance.environmentType);
        }
        else
        {
            bool iceNow = GameManager.Instance &&
                          GameManager.Instance.floorNumber >= 3 &&
                          GameManager.Instance.floorNumber <= 4;
            ApplyIcePhysics(iceNow);
        }
    }
    private void HandleEnvironmentChanged(EnvironmentType env)
    {
        bool iceNow = (env == EnvironmentType.Ice);
        ApplyIcePhysics(iceNow);
    }

    private void ApplyIcePhysics(bool enable)
    {
        isIceFloor = enable;
        if (rb == null) return;

        if (enable)
        {
            // kill carry-over momentum so ice starts from the same baseline
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.drag = Mathf.Clamp(iceDrag, 0.02f, 0.2f);
            rb.angularDrag = iceAngularDrag;
            Debug.Log($"[Player] Ice ON drag={rb.drag} vel={rb.velocity}");
        }
        else
        {
            rb.drag = 0.5f;        // your normal values
            rb.angularDrag = 0.05f;
            Debug.Log($"[Player] Ice OFF drag={rb.drag}");
        }
    }

    void Update()
    {
        if (isKnockedBack || isDashing) return;

        // Calculate movement parameters
        float speed = playerStats.moveSpeed * (isIceFloor ? iceSpeedMultiplier : 1f);
        Vector2 moveDirection = moveInput.normalized;
        Vector2 targetVelocity = moveInput * speed;

        UpdateDust();

        if (moveInput != Vector2.zero)
        {
            // Check for pushable objects first
            RaycastHit2D hit = Physics2D.Raycast(rb.position, moveDirection, 0.6f, pushableLayer);
            if (hit.collider != null && hit.collider.CompareTag("Pushable"))
            {
                // Pushing behavior (unchanged from your original)
                Rigidbody2D pushRb = hit.collider.attachedRigidbody;
                if (pushRb != null)
                {
                    float pushSpeed = 2f;
                    Vector2 pushDir = moveDirection;
                    pushRb.velocity = pushDir * pushSpeed;
                    rb.velocity = targetVelocity * 0.9f;
                }
            }
            else
            {
                // ICE PHYSICS
                if (isIceFloor && moveInput != Vector2.zero)
                {
                    rb.AddForce(targetVelocity * 7f * Time.deltaTime * 60f); // Framerate-independent
                    if (rb.velocity.magnitude > speed * 1.8f)
                    {
                        rb.velocity = rb.velocity.normalized * speed * 1.8f;
                    }
                }
                // NORMAL PHYSICS
                else
                {
                    rb.velocity = targetVelocity;
                }
            }
        }
        else
        {
            // ICE PHYSICS: Gradual slowdown
            if (isIceFloor)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.03f); // Smooth decay
            }
            // NORMAL PHYSICS: Instant stop
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (isKnockedBack == false)
        {
            animator.SetBool("isWalking", true);

            if (context.canceled)
            {
                animator.SetBool("isWalking", false);
                animator.SetFloat("LastInputX", moveInput.x);
                animator.SetFloat("LastInputY", moveInput.y);
            }

            moveInput = context.ReadValue<Vector2>();
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);

            if (moveInput != Vector2.zero)
            {
                lastDirection = moveInput;
            }
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    public Vector2 GetLastDirection()
    {
        return lastDirection;
    }


    private void UpdateDust()
    {
        if (dustParticles == null) return;

        bool isMoving = moveInput != Vector2.zero && !isKnockedBack && !isDashing;

        // Play or stop particles as needed
        if (isMoving)
        {
            if (!dustParticles.isPlaying)
                dustParticles.Play();
            dustEmission.rateOverTime = walkEmissionRate;

            // Emit a small burst if player JUST started moving
            if (!wasMovingLastFrame)
            {
                dustParticles.Emit(5);
            }
        }
        else if (isDashing)
        {
            if (!dustParticles.isPlaying)
                dustParticles.Play();
            dustEmission.rateOverTime = dashEmissionRate;
        }
        else
        {
            dustEmission.rateOverTime = 0f;
            if (dustParticles.isPlaying)
                dustParticles.Stop();
        }

        wasMovingLastFrame = isMoving;
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        Debug.Log("Knockback direction: " + direction);
        rb.velocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    public void SetNearbyKey(Transform keyTransform, KeyPickup keyScript)
    {
        keyPromptTarget = keyTransform;
        currentKeyInRange = keyScript;

        if (keyTransform != null && keyInteractPrompt != null)
            keyInteractPrompt.SetActive(true);
        else if (keyInteractPrompt != null)
            keyInteractPrompt.SetActive(false);
    }

    public void SetNearbyChest(Transform chestTransform, Chest chestScript)
    {
        promptTarget = chestTransform;
        currentChestInRange = chestScript;

        if (chestTransform != null && interactPrompt != null)
            interactPrompt.SetActive(true);
        else if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    public void SetNearbyHealth(Transform pickupTransform, HealthPickup healthScript)
    {
        healthPromptTarget = pickupTransform;
        currentHealthInRange = healthScript;

        if (pickupTransform != null && healthInteractPrompt != null)
            healthInteractPrompt.SetActive(true);
        else if (healthInteractPrompt != null)
            healthInteractPrompt.SetActive(false);
    }

    void LateUpdate()
    {
        // Chest Prompt
        if (interactPrompt != null && interactPrompt.activeSelf && promptTarget != null)
        {
            Vector3 worldPos = promptTarget.position + promptOffset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransform promptRect = interactPrompt.GetComponent<RectTransform>();
            promptRect.position = screenPos;
        }

        // Health Prompt
        if (healthInteractPrompt != null && healthInteractPrompt.activeSelf && healthPromptTarget != null)
        {
            Vector3 worldPos = healthPromptTarget.position + healthPromptOffset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransform promptRect = healthInteractPrompt.GetComponent<RectTransform>();
            promptRect.position = screenPos;
        }

        if (keyInteractPrompt != null && keyInteractPrompt.activeSelf && keyPromptTarget != null)
        {
            Vector3 worldPos = keyPromptTarget.position + keyPromptOffset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransform promptRect = keyInteractPrompt.GetComponent<RectTransform>();
            promptRect.position = screenPos;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentChestInRange != null)
            {
                currentChestInRange.TryOpenChest(gameObject);
                if (interactPrompt != null) interactPrompt.SetActive(false);
                promptTarget = null;
                currentChestInRange = null;
            }
            else if (currentHealthInRange != null)
            {
                currentHealthInRange.TryPickup(gameObject);
                if (healthInteractPrompt != null) healthInteractPrompt.SetActive(false);
                healthPromptTarget = null;
                currentHealthInRange = null;
            }
            else if (currentKeyInRange != null)
            {
                currentKeyInRange.TryPickup(gameObject);
                if (keyInteractPrompt != null) keyInteractPrompt.SetActive(false);
                keyPromptTarget = null;
                currentKeyInRange = null;
            }
        }
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    IEnumerator Dash()
    {
        AudioManager.I?.PlayOneShot(AudioManager.I?.dash, 0.9f, 1.0f);
        // Store original movement state
        bool wasOnIce = isIceFloor;
        float originalDrag = rb.drag;

        // Start dash
        isDashing = true;
        lastDashTime = Time.time;

        // Cancel ice physics during dash
        if (wasOnIce) rb.drag = 1f; // Normal friction during dash

        // Visual/audio effects
        if (_cameraShake != null)
            _cameraShake.Shake(0.2f, 0.22f);

        if (dustParticles != null)
            dustParticles.Emit(15);

        // Safety collision ignore
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        Vector2 dashDir = lastDirection.normalized;
        float elapsed = 0f;
        float afterimageTimer = 0f;

        // Dash movement
        while (elapsed < dashDuration)
        {
            rb.velocity = dashDir * dashDistance / dashDuration;

            // Afterimages
            afterimageTimer += Time.deltaTime;
            if (afterimageTimer >= afterimageInterval)
            {
                CreateAfterimage();
                afterimageTimer = 0f;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // End dash
        rb.velocity = Vector2.zero;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

        // Restore ice physics if needed
        if (wasOnIce) rb.drag = iceDrag;

        isDashing = false;
    }
    private void OnEnable()
    {
        RoomManager.OnEnvironmentApplied += HandleEnvironmentChanged;
    }

    private void OnDisable()
    {
        RoomManager.OnEnvironmentApplied -= HandleEnvironmentChanged;
    }
    void CreateAfterimage()
    {
        if (afterimagePrefab == null) return;
        var obj = Instantiate(afterimagePrefab, transform.position, Quaternion.identity);
        var sr = GetComponent<SpriteRenderer>();
        var ghost = obj.GetComponent<PlayerDashAfterimage>();
        ghost.Set(sr.sprite, transform.position, sr.flipX, new Color(1f, 1f, 1f, 0.45f));
    }

}
