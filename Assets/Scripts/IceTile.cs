using UnityEngine;

public class IceTile : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Color iceColor = new Color(0.7f, 0.8f, 1f, 1f); // Light blue
    [SerializeField] private GameObject frostParticles; // Optional

    private void Start()
    {
        // Tint the tile
        GetComponent<SpriteRenderer>().color = iceColor;

        // Optional particles
        if (frostParticles != null)
        {
            Instantiate(frostParticles, transform.position, Quaternion.identity, transform);
        }
    }
}