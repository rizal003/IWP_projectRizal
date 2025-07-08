using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Vector3 originalRoomPosition;
    private float currentShakeTime;
    private bool isShaking;

    void LateUpdate()
    {
        if (isShaking)
        {
            if (currentShakeTime > 0)
            {
                // Apply shake while maintaining room center as reference
                transform.position = originalRoomPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
                currentShakeTime -= Time.deltaTime;
            }
            else
            {
                // Snap back exactly when shake ends
                transform.position = originalRoomPosition;
                isShaking = false;
            }
        }
    }

    public void SetRoomCenter(Vector3 roomCenter)
    {
        originalRoomPosition = roomCenter;
        if (!isShaking)
        {
            transform.position = originalRoomPosition;
        }
    }

    public void Shake(float duration = 0.15f, float magnitude = 0.1f)
    {
        if (!isShaking)
        {
            originalRoomPosition = transform.position;
        }
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        currentShakeTime = shakeDuration;
        isShaking = true;
    }
}