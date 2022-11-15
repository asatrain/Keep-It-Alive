using System;
using TMPro;
using UnityEngine;
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
    [SerializeField] private Image healthBarFillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Color minHealthFillImageColor;
    [SerializeField] private Color maxHealthFillImageColor;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Transform checkItemThrownPoint;
    [SerializeField] private float checkItemThrownRadius;
    private readonly Collider[] overlapResults = new Collider[10];

    private float fireLightStartPositionY;
    private LayerMask worldItemMask;

    private void Start()
    {
        fireLightStartPositionY = fireLight.transform.position.y;
        worldItemMask = LayerMask.GetMask("WorldItem");
        GameManager.Instance.OnGameStateChanged += GameStateChangeHandler;
    }

    private void GameStateChangeHandler(GameState prevState, GameState newState)
    {
        if (newState == GameState.Pause) audioSource.volume = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;

        audioSource.volume = PlayerPrefs.GetFloat("soundVolume", 1) * health / MaxHealth;
        AddBonfireHealth(-healthDecreaseSpeedCurve.Evaluate(GameManager.Instance.GameTime) * Time.deltaTime);
        UpdateBonfireLight();
        UpdateBonfireState();
        CheckItemThrown();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(checkItemThrownPoint.position, checkItemThrownRadius);
    }

    private void AddBonfireHealth(float healthToAdd)
    {
        health += healthToAdd;
        if (health < 0)
            health = 0;
        else if (health > MaxHealth)
            health = MaxHealth;

        healthBarSlider.value = health / MaxHealth;
        healthText.text = $"{Mathf.CeilToInt(health)} / {MaxHealth}";
    }

    private void UpdateBonfireLight()
    {
        var fireLightPosition = fireLight.transform.position;
        fireLightPosition.Set(fireLightPosition.x,
            fireLightStartPositionY + Mathf.Sin(GameManager.Instance.GameTime * Mathf.PI) * fireLightOffset,
            fireLightPosition.z);
        fireLight.transform.position = fireLightPosition;
    }

    private void UpdateBonfireState()
    {
        var activeCount = 0;
        for (var i = 0; i < logs.Length; i++)
        {
            var active = (float) (logs.Length - 1 - i) / logs.Length < health / MaxHealth;
            if (active)
                ++activeCount;

            logs[i].SetActive(active);
            switch (active)
            {
                case false when fireParticles[i].isPlaying:
                    fireParticles[i].Stop();
                    break;
                case true when !fireParticles[i].isPlaying:
                    fireParticles[i].Play();
                    break;
            }
        }

        if (activeCount == 0)
        {
            foreach (var fireParticle in fireParticles) fireParticle.Clear();
            fireLight.intensity = 0;
        }
        else
        {
            fireLight.intensity = Mathf.Lerp(fireLight.intensity,
                fireLightMinIntensity + (fireLightMaxIntensity - fireLightMinIntensity) * (activeCount - 1) /
                (logs.Length - 1), Time.deltaTime);
        }

        healthBarFillImage.color =
            Color.Lerp(minHealthFillImageColor, maxHealthFillImageColor, health / MaxHealth);
        if (health <= 0) GameManager.Instance.GameState = GameState.GameOver;
    }

    private void CheckItemThrown()
    {
        var size = Physics.OverlapSphereNonAlloc(checkItemThrownPoint.position, checkItemThrownRadius, overlapResults,
            worldItemMask);
        for (var i = 0; i < size; i++)
        {
            var worldItem = overlapResults[i].GetComponent<WorldItem>();
            AddBonfireHealth(worldItem.itemInfo.bonfireHealthAddition);
            Destroy(worldItem.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (!ReferenceEquals(GameManager.Instance, null))
            GameManager.Instance.OnGameStateChanged -= GameStateChangeHandler;
    }
}