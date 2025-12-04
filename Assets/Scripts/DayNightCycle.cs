using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Light2D globalLight;  // 2D Light Component
    public float dayTimeDuration = 60f;  // Duration of the day phase in seconds
    public float nightTimeDuration = 30f;  // Duration of the night phase in seconds
    public float transitionDuration = 5f;  // Time for the transition between day and night
    public Gradient skyColors;       // Sky color at different times of day
    public AnimationCurve lightIntensity;  // Curve for light intensity over time
    public float minIntensity = 0.2f;  // Minimum light intensity during night
    public float maxIntensity = 1f;    // Maximum light intensity during day

    [Header("Shadow Settings")]
    public float dayShadowIntensity = 0.1f;  // Soft shadows during day
    public float nightShadowIntensity = 0.8f;  // Strong shadows during night

    public bool isNight;  // Boolean to check if it's night

    private float totalCycleDuration;
    private float timeOfDay;

    void Start()
    {
        totalCycleDuration = dayTimeDuration + nightTimeDuration + (2 * transitionDuration);
        timeOfDay = 0f;
        isNight = false;
    }

    void Update()
    {
        timeOfDay += Time.deltaTime / totalCycleDuration;
        timeOfDay %= 1f;

        float dayEnd = dayTimeDuration / totalCycleDuration;
        float transitionToNightEnd = dayEnd + (transitionDuration / totalCycleDuration);
        float nightEnd = transitionToNightEnd + (nightTimeDuration / totalCycleDuration);
        float transitionToDayEnd = nightEnd + (transitionDuration / totalCycleDuration);

        if (timeOfDay < dayEnd)
        {
            UpdateDay();
            isNight = false;
        }
        else if (timeOfDay < transitionToNightEnd)
        {
            UpdateTransition(true);
        }
        else if (timeOfDay < nightEnd)
        {
            UpdateNight();
            isNight = true;
        }
        else
        {
            UpdateTransition(false);
        }
    }

    void UpdateDay()
    {
        Camera.main.backgroundColor = skyColors.Evaluate(0f);
        globalLight.intensity = maxIntensity;
        globalLight.color = skyColors.Evaluate(0f);
        globalLight.shadowIntensity = dayShadowIntensity;
    }

    void UpdateNight()
    {
        Camera.main.backgroundColor = skyColors.Evaluate(1f);
        globalLight.intensity = minIntensity;
        globalLight.color = skyColors.Evaluate(1f);
        globalLight.shadowIntensity = nightShadowIntensity;
    }

    void UpdateTransition(bool toNight)
    {
        float start = toNight ? (dayTimeDuration / totalCycleDuration) : (1f - (transitionDuration / totalCycleDuration));
        float t = (timeOfDay - start) / (transitionDuration / totalCycleDuration);

        float intensity = Mathf.Lerp(maxIntensity, minIntensity, toNight ? t : 1f - t);
        globalLight.intensity = intensity;

        float shadow = Mathf.Lerp(dayShadowIntensity, nightShadowIntensity, toNight ? t : 1f - t);
        globalLight.shadowIntensity = shadow;

        Color targetColor = Color.Lerp(skyColors.Evaluate(0f), skyColors.Evaluate(1f), toNight ? t : 1f - t);
        Camera.main.backgroundColor = targetColor;
        globalLight.color = targetColor;
    }
}
