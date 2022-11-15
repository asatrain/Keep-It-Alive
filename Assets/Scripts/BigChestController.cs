using System.Collections;
using UnityEngine;

public class BigChestController : ChestController
{
    [SerializeField] private int defaultHealth;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float dropPower;
    [SerializeField] private ItemInfo jerrycan;
    [SerializeField] private ItemInfo wine;
    [SerializeField] private ItemInfo whiskey;
    [SerializeField] private float whiskeyDropProbability;
    [SerializeField] private float resetTime;
    [SerializeField] private float resetTimeRandomDelta;
    [SerializeField] private float resetDelay;
    [SerializeField] private float minResetDistance;

    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Collider chestCollider;
    [SerializeField] private GameObject chestGraphics;

    private readonly int chestHitAnimation = Animator.StringToHash("Chest Hit");

    private int health;
    private Transform playerTransform;

    private void Start()
    {
        health = defaultHealth;
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        audioSource.volume = PlayerPrefs.GetFloat("soundVolume", 1);
    }

    public override void Hit()
    {
        --health;
        audioSource.Play();
        if (health > 0)
        {
            animator.Play(chestHitAnimation, -1, 0);
        }
        else
        {
            Drop(jerrycan);
            Drop(Random.Range(0f, 1f) <= whiskeyDropProbability ? whiskey : wine);

            StartCoroutine(RequireReset());
            chestCollider.enabled = false;
            chestGraphics.SetActive(false);
        }
    }

    private IEnumerator RequireReset()
    {
        yield return new WaitForSeconds(
            Random.Range(resetTime - resetTimeRandomDelta, resetTime + resetTimeRandomDelta));
        while (Vector3.Distance(transform.position, playerTransform.position) <= minResetDistance)
            yield return new WaitForSeconds(resetDelay);

        health = defaultHealth;
        chestCollider.enabled = true;
        chestGraphics.SetActive(true);
    }

    private void Drop(ItemInfo item)
    {
        var rotation = Random.rotation;
        rotation.x = 0;
        rotation.z = 0;
        var dropPosition = dropPoint.position;
        var worldItem = Instantiate(item.worldItem, dropPosition, rotation);
        worldItem.itemRigidbody.AddForce(rotation * Vector3.forward * dropPower, ForceMode.Impulse);
    }
}