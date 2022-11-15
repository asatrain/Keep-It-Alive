using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private float freezeTime;
    [SerializeField] private float moveFromGroundSpeed;

    public bool pickable;
    public Rigidbody itemRigidbody;
    public ItemInfo itemInfo;
    private int bridgeLayer;

    private float freezeTimeLeft;

    private int groundLayer;
    private int treeLayer;

    private void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        bridgeLayer = LayerMask.NameToLayer("Bridge");
        treeLayer = LayerMask.NameToLayer("Tree");

        pickable = false;
        freezeTimeLeft = freezeTime;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;

        if (freezeTimeLeft > 0)
            freezeTimeLeft -= Time.deltaTime;
        else
            pickable = true;
    }

    private void OnTriggerStay(Collider other)
    {
        var layer = other.gameObject.layer;
        if (layer == treeLayer)
        {
            var constraints = itemRigidbody.constraints;
            constraints |= RigidbodyConstraints.FreezePositionX;
            constraints |= RigidbodyConstraints.FreezePositionZ;
            itemRigidbody.constraints = constraints;
        }
        else if (layer == groundLayer || layer == bridgeLayer)
        {
            itemRigidbody.isKinematic = true;
            var itemTransform = transform;
            var position = itemTransform.position;
            position.y += moveFromGroundSpeed * Time.fixedDeltaTime;
            itemRigidbody.MovePosition(position);
        }
    }
}