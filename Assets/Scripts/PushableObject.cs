using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    private Rigidbody2D rb;
    private ParticleSystem dustParticles;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        dustParticles = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            if (!dustParticles.isPlaying)
                dustParticles.Play();
        }
        else
        {
            if (dustParticles.isPlaying)
                dustParticles.Stop();
        }
    }
}

