using UnityEngine;

public class TreeStageController : MonoBehaviour
{
    [SerializeField] private GameObject[] stages;
    [SerializeField] private MeshFilter[] stageMeshFilters;
    [SerializeField] private int currentStageInd;
    [SerializeField] private MeshCollider treeCollider;

    private void Start()
    {
        SetStage(currentStageInd);
    }

    private void SetStage(int stageInd)
    {
        stages[currentStageInd].SetActive(false);
        treeCollider.sharedMesh = null;
        if (stageInd >= 0 && stageInd < stages.Length)
        {
            currentStageInd = stageInd;
            stages[currentStageInd].SetActive(true);
            treeCollider.sharedMesh = stageMeshFilters[currentStageInd].mesh;
        }
    }

    public void ChopTree()
    {
        SetStage(currentStageInd - 1);
    }
}