using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVisionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D globalDimLight;   
    [SerializeField] private Light2D visionConeLight;  
    [SerializeField] private Transform aimSource;      
    [SerializeField] private PlayerMovement movement;  
    [Header("Vision Settings")]
    [SerializeField] private float globalDimIntensity = 0.10f;
    [SerializeField] private float coneIntensity = 1.6f;
    [SerializeField] private float coneOuterRadius = 7.6f;
    [SerializeField] private float coneInnerRadius = 6.3f; 
    [SerializeField] private float coneInnerAngle = 36.0f;
    [SerializeField] private float coneOuterAngle = 41.0f;
    [SerializeField] private float turnSpeedDegPerSec = 720f; 

    private Vector2 lastAimDir = Vector2.down; 

    private void Awake()
    {
        if (aimSource == null) aimSource = transform;
        if (movement == null) movement = GetComponentInParent<PlayerMovement>();

        // Parent the cone to the player so position follows automatically
        if (visionConeLight != null && visionConeLight.transform.parent != aimSource)
            visionConeLight.transform.SetParent(aimSource, worldPositionStays: true);

        ConfigureDefaults();
    }

    private void Update()
    {
        if (visionConeLight == null || movement == null) return;

        // get the last non-zero movement direction as "facing"
        Vector2 dir = movement.GetLastDirection();
        if (dir.sqrMagnitude > 0.0001f)
            lastAimDir = dir.normalized;

        float targetAngle = Mathf.Atan2(lastAimDir.y, lastAimDir.x) * Mathf.Rad2Deg - 90f;

        // smooth turn
        float currentZ = visionConeLight.transform.eulerAngles.z;
        float newZ = Mathf.MoveTowardsAngle(currentZ, targetAngle, turnSpeedDegPerSec * Time.deltaTime);
        visionConeLight.transform.rotation = Quaternion.Euler(0, 0, newZ);
    }

    private void LateUpdate()
    {
        if (visionConeLight == null || aimSource == null) return;
        visionConeLight.transform.position = aimSource.position;
    }

    public void EnableVisionCone(bool enable)
    {
        if (globalDimLight != null)
        {
            globalDimLight.enabled = enable;
            globalDimLight.intensity = enable ? globalDimIntensity : 1f;
        }

        if (visionConeLight != null)
        {
            visionConeLight.enabled = enable;
            visionConeLight.intensity = enable ? coneIntensity : 0f;
        }
    }

    private void ConfigureDefaults()
    {
        if (visionConeLight != null)
        {
            visionConeLight.lightType = Light2D.LightType.Point;
            visionConeLight.pointLightOuterRadius = coneOuterRadius;
            visionConeLight.pointLightInnerRadius = coneInnerRadius;
            visionConeLight.pointLightInnerAngle = coneInnerAngle;
            visionConeLight.pointLightOuterAngle = coneOuterAngle;
            visionConeLight.intensity = coneIntensity;
            visionConeLight.enabled = false; // start off
        }

        if (globalDimLight != null)
        {
            globalDimLight.lightType = Light2D.LightType.Global;
            globalDimLight.intensity = 1f;   // full bright by default
            globalDimLight.enabled = false;  // start off
        }
    }
}
