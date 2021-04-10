using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Bonfire : MonoBehaviour
{
    private const float MaxHealth = 100;

    [SerializeField] [Range(0, MaxHealth)] private float health;
    [SerializeField] private AnimationCurve healthDecreaseSpeedCurve;
    [SerializeField] private float fireLightMaxIntensity;
    [SerializeField] private float fireLightMinIntensity;
    [SerializeField] private float fireLightOffset;

    [SerializeField] private GameObject[] logs;
    [SerializeField] private ParticleSystem[] fireParticles;
    [SerializeField] private Light fireLight;

    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    private float fireLightStartPositionY;

    private void Start()
    {
        fireLightStartPositionY = fireLight.transform.position.y;
    }

    private void Update()
    {
        health -= healthDecreaseSpeedCurve.Evaluate(Time.timeSinceLevelLoad) * Time.deltaTime;
        healthBarSlider.value = health / MaxHealth;
        healthText.text = $"{Mathf.CeilToInt(health)} / {MaxHealth}";

        Vector3 fireLightPosition = fireLight.transform.position;
        fireLightPosition.Set(fireLightPosition.x,
            fireLightStartPositionY + Mathf.Sin(Time.timeSinceLevelLoad * Mathf.PI) * fireLightOffset,
            fireLightPosition.z);
        fireLight.transform.position = fireLightPosition;

        CheckBonfireState();
    }

    private void CheckBonfireState()
    {
        int activeCount = 0;
        for (int i = 0; i < logs.Length; i++)
        {
            bool active = (float) (logs.Length - 1 - i) / logs.Length < health / MaxHealth;
            if (active)
                ++activeCount;

            logs[i].SetActive(active);

            if (!active && fireParticles[i].isPlaying)
                fireParticles[i].Stop();
            else if (active && !fireParticles[i].isPlaying)
                fireParticles[i].Play();
        }

        if (activeCount == 0)
        {
            foreach (ParticleSystem fireParticle in fireParticles)
            {
                fireParticle.Clear();
            }

            fireLight.intensity = 0;
        }
        else
        {
            fireLight.intensity = Mathf.Lerp(fireLight.intensity,
                fireLightMinIntensity + (fireLightMaxIntensity - fireLightMinIntensity) * (activeCount - 1) /
                (logs.Length - 1), 5 * Time.deltaTime);
        }

        if (health <= 0)
        {
            Time.timeScale = 0;
        }
    }
}