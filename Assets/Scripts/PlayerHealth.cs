using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public SpriteRenderer playerSr;
    public PlayerMovement playerMovement;
    public Animator animator;
    private const int DEATH_RIGHT = 0;
    private const int DEATH_LEFT = 1;
    private const int DEATH_UP = 2;
    private const int DEATH_DOWN = 3;
    private CameraShake _cameraShake;

    public PlayerStats playerStats;
    public float freezeFrameDuration = 0.05f;
    public HealthDisplay healthDisplay;
    public ScreenTint screenTint;
    private float nextHazardTickTime = 0f;
    private float hazardGraceUntil = 0f;
    private float _lastHazardTickTime;

    void Start()
    {
        _cameraShake = Camera.main?.GetComponent<CameraShake>();
        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();
        maxHealth = playerStats.maxHealth;
        currentHealth = maxHealth;
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeHealth(int amount)
    {
        AudioManager.I?.PlayOneShot(AudioManager.I?.playerHit, 0.9f, Random.Range(0.9f, 1.1f));
        if (amount < 0)
        {
            if (_cameraShake != null)
                _cameraShake.Shake(0.3f, 0.35f);

            StartCoroutine(FreezeFrame());
            if (screenTint != null) screenTint.Flash();
            int damagedHeart = (currentHealth - 1) / 2; // Which heart to pulse
            bool isRow1 = damagedHeart < 10;
            int heartIndex = isRow1 ? damagedHeart : damagedHeart - 10;
            StartCoroutine(healthDisplay.PulseHeart(heartIndex, isRow1));

        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (currentHealth <= 0)
        {
            AudioManager.I?.PlayOneShot(AudioManager.I?.death, 0.9f, Random.Range(0.9f, 1.1f));
            Die();
        }
    }

    void Die()
    {
        Vector2 lastDir = playerMovement.GetLastDirection();
        int deathDir = DEATH_DOWN;

        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            deathDir = lastDir.x > 0 ? DEATH_RIGHT : DEATH_LEFT;
        else
            deathDir = lastDir.y > 0 ? DEATH_UP : DEATH_DOWN;

        animator.SetInteger("DeathDirection", deathDir);
        animator.SetTrigger("Die");

        playerMovement.enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(ReturnToMainMenuAfterDelay(2f));
    }

    private IEnumerator ReturnToMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator FreezeFrame()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(freezeFrameDuration);
        Time.timeScale = 1f;
    }
    public bool TryApplyHazardTick(int halfHearts, float interval)
    {
        if (Time.time < hazardGraceUntil) return false;
        if (Time.time < nextHazardTickTime) return false;

        ChangeHealth(-Mathf.Abs(halfHearts));           
        nextHazardTickTime = Time.time + Mathf.Max(0.1f, interval);
        return true;
    }

    public void GrantHazardGrace(float seconds)
    {
        float until = Time.time + Mathf.Max(0f, seconds);
        hazardGraceUntil = Mathf.Max(hazardGraceUntil, until);
        nextHazardTickTime = Mathf.Max(nextHazardTickTime, hazardGraceUntil); 
    }
}