using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("UI References")]
    public Slider waterSlider;
    public Slider foodSlider;
    public Image damageOverlay; // Add a full-screen UI Image set to red color with 0 alpha initially

    [Header("Stats Settings")]
    public float maxWater = 100f;
    public float maxFood = 100f;
    public float foodDepletionRate = 0.5f;
    public float waterDepletionRate = 1f;
    public float damageRate = 10f;

    [Header("Damage Effects")]
    public float damageInterval = 2f;
    public float flashDuration = 0.5f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Red with 50% transparency

    private float currentWater;
    private float currentFood;
    private bool isTakingDamage = false;
    private Coroutine damageCoroutine;
    private Coroutine flashCoroutine;

    void Start()
    {
        currentWater = maxWater;
        currentFood = maxFood;

        waterSlider.maxValue = maxWater;
        foodSlider.maxValue = maxFood;

        // Initialize damage overlay
        if (damageOverlay != null)
        {
            damageOverlay.color = Color.clear;
        }
    }

    void Update()
    {
        // Update resource levels
        currentWater -= waterDepletionRate * Time.deltaTime;
        currentFood -= foodDepletionRate * Time.deltaTime;

        // Update UI
        waterSlider.value = currentWater;
        foodSlider.value = currentFood;

        // Handle damage state
        if ((currentWater <= 0 || currentFood <= 0) && !isTakingDamage)
        {
            damageCoroutine = StartCoroutine(DamageOverTime());
        }
        else if (currentWater > 0 && currentFood > 0 && isTakingDamage)
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                damageOverlay.color = Color.clear;
            }
            isTakingDamage = false;
        }

        // Clamp values
        currentWater = Mathf.Clamp(currentWater, 0, maxWater);
        currentFood = Mathf.Clamp(currentFood, 0, maxFood);
    }

    IEnumerator DamageOverTime()
    {
        isTakingDamage = true;
        while (currentWater <= 0 || currentFood <= 0)
        {
            // Apply damage
            GetComponent<PlayerHealth>().TakeDamage(damageRate);

            // Trigger flash effect
            if (damageOverlay != null)
            {
                if (flashCoroutine != null)
                {
                    StopCoroutine(flashCoroutine);
                }
                flashCoroutine = StartCoroutine(FlashEffect());
            }

            yield return new WaitForSeconds(damageInterval);
        }
        isTakingDamage = false;
    }

    IEnumerator FlashEffect()
    {
        // Flash in
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            damageOverlay.color = Color.Lerp(Color.clear, flashColor, elapsedTime / (flashDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Flash out
        elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            damageOverlay.color = Color.Lerp(flashColor, Color.clear, elapsedTime / (flashDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        damageOverlay.color = Color.clear;
    }

    public bool AddFood(float food)
    {
        if (currentFood >= maxFood)
        {
            return false;
        }
        if (currentFood + food >= maxFood)
            currentFood = maxFood;
        else
            currentFood += food;
        return true;
    }

    public bool AddWater(float water)
    {
        if (currentWater >= maxWater)
            return false;
        if (currentWater + water >= maxWater)
            currentWater = maxWater;
        else
            currentWater += water;
        return true;
    }
}