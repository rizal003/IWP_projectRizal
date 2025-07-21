using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBobSimple : MonoBehaviour
{
    Vector3 startPos;
    public float amplitude = 0.12f; // up/down distance
    public float frequency = 2.2f;  // speed

    void Start() => startPos = transform.position;

    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + Vector3.up * offset;
    }
}
