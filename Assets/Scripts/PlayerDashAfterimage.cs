using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashAfterimage : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color color;
    private float fadeSpeed = 2.5f; 

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color = color;

        if (color.a <= 0f)
            Destroy(gameObject);
    }

    // Call this to set the sprite, position, and flip state
    public void Set(Sprite sprite, Vector3 position, bool flipX, Color col)
    {
        sr.sprite = sprite;
        transform.position = position;
        sr.flipX = flipX;
        sr.color = col;
    }
}
