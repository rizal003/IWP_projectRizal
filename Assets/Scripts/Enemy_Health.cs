using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy_Health : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 3;
    [SerializeField] public int currentHealth;

    [Header("Effects")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public GameObject deathEffect;

    [Header("Health Bar")]
    public Slider healthBarPrefab; // Assign in Inspector
    public Vector3 healthBarOffset = new Vector3(0, -0.5f, 0); // Position below enemy
    private Slider healthBar;
    private CanvasGroup healthBarCanvasGroup;
    public float healthBarFadeDelay = 1f; 
    public float healthBarFadeSpeed = 2f; 

    private Animator animator;
    public bool isDead = false;
    private CameraShake _cameraShake;
    private Color originalColor;

    void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        _cameraShake = Camera.main?.GetComponent<CameraShake>();

        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Initialize health bar
        if (healthBarPrefab != null)
        {
            healthBar = Instantiate(healthBarPrefab, FindObjectOfType<Canvas>().transform);
            healthBar.gameObject.SetActive(false);
            healthBarCanvasGroup = healthBar.GetComponent<CanvasGroup>();
            if (healthBarCanvasGroup == null)
                healthBarCanvasGroup = healthBar.gameObject.AddComponent<CanvasGroup>();

            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }

    void Update()
    {
        // Update health bar position to follow enemy
        if (healthBar != null && !isDead)
        {
            healthBar.transform.position =
                Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
     
        _cameraShake?.Shake(0.1f, 0.15f);
        currentHealth -= amount;

        // Show health bar on first hit
        if (healthBar != null && !healthBar.gameObject.activeSelf)
        {
            healthBar.gameObject.SetActive(true);
            if (healthBarCanvasGroup != null)
            {
                healthBarCanvasGroup.alpha = 1f;
            }
        }

        // Update health bar
        if (healthBar != null)
            healthBar.value = currentHealth;

        animator.SetTrigger("Hit");
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("die");

        // Notify room about enemy death
        Room parentRoom = GetComponentInParent<Room>();
        if (parentRoom != null)
            parentRoom.OnEnemyDied();

        // Hide health bar
        if (healthBar != null)
            Destroy(healthBar.gameObject);

        // Disable components
        GetComponent<Collider2D>().enabled = false;
        if (TryGetComponent<Rigidbody2D>(out var rb)) rb.simulated = false;
        if (TryGetComponent<Enemy_Movement>(out var move)) move.enabled = false;

        // Effects
        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Destroy after animation
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animLength);
    }

    IEnumerator FlashRed()
    {
        if (!spriteRenderer) yield break;

        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

}