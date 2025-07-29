using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
    public bool isLit = false;
    public Sprite litSprite;
    public Light2D torchLight;
    public ParticleSystem flameParticles;
    public TorchPuzzleRoom puzzleRoom; 

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        SetLit(false);


        // If puzzleRoom not set, auto-find it in parent
        if (puzzleRoom == null)
            puzzleRoom = GetComponentInParent<TorchPuzzleRoom>();
    }

    public void SetLit(bool lit)
    {

        isLit = lit;
        sr.sprite = litSprite;

        // Tint darker when unlit for visual feedback
        sr.color = isLit ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f);

        if (torchLight != null) torchLight.enabled = isLit;

        if (flameParticles != null)
        {
            if (isLit && !flameParticles.isPlaying) flameParticles.Play();
            else if (!isLit && flameParticles.isPlaying) flameParticles.Stop();
        }

        // ----- Notify the puzzle room here, at the very end! -----
        if (puzzleRoom == null)
            puzzleRoom = GetComponentInParent<TorchPuzzleRoom>();
        if (puzzleRoom != null)
            puzzleRoom.RegisterTorchLit(this);
    }

    // Call this to light the torch
    public void Light()
    {
        if (!isLit)
        {
            SetLit(true); // Don't call puzzleRoom.RegisterTorchLit here!
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isLit) return;
        // Check by layer instead of tag
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            Light();
        }
    }
}
