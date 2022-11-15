using UnityEngine;

public class TreeStageController : MonoBehaviour
{
    [SerializeField] private GameObject[] stages;
    [SerializeField] private MeshFilter[] stageMeshFilters;
    [SerializeField] private int currentStageInd;
    [SerializeField] private MeshCollider treeCollider;
    [SerializeField] private ItemInfo itemForChopping;
    [SerializeField] private float timeToGrow;
    [SerializeField] private float minGrowDistance;
    [SerializeField] private float timeToGrowDelay;
    [SerializeField] private AudioSource audioSource;
    private Transform playerTransform;

    private float timeToGrowLeft;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SetStage(currentStageInd);
        timeToGrowLeft = timeToGrow;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;

        audioSource.volume = PlayerPrefs.GetFloat("soundVolume", 1);
        UpdateTimeToGrow();
    }

    private void UpdateTimeToGrow()
    {
        if (currentStageInd >= stages.Length - 1) return;

        if (timeToGrowLeft > 0)
        {
            timeToGrowLeft -= Time.deltaTime;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) >= minGrowDistance)
        {
            SetStage(currentStageInd + 1);
            timeToGrowLeft = timeToGrow;
        }
        else
        {
            timeToGrowLeft = timeToGrowDelay;
        }
    }

    private void SetStage(int stageInd)
    {
        stages[currentStageInd].SetActive(false);
        treeCollider.sharedMesh = null;
        timeToGrowLeft = timeToGrow;
        if (stageInd >= 0 && stageInd < stages.Length)
        {
            currentStageInd = stageInd;
            stages[currentStageInd].SetActive(true);
            treeCollider.sharedMesh = stageMeshFilters[currentStageInd].mesh;
        }
    }

    public ItemInfo ChopTree(int chopPower, out int resultCount)
    {
        resultCount = Mathf.Min(currentStageInd + 1, chopPower);
        SetStage(currentStageInd - chopPower);
        audioSource.Play();
        return itemForChopping;
    }
}