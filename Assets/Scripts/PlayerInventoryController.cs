using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private float dropFrequency;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private Transform dropDirectionPoint;
    [SerializeField] private float dropPower;
    [SerializeField] private PlayerMovementController playerMovementController;

    private Inventory inventory;
    private int worldItemLayer;

    private float dropDelay;

    private void Start()
    {
        inventory = inventoryView.Inventory;
        worldItemLayer = LayerMask.NameToLayer("WorldItem");
        dropDelay = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;

        HandleInputs();
    }

    private void HandleInputs()
    {
        if (dropDelay > 0) dropDelay -= Time.deltaTime;


        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0) inventory.SelectPrevItem();

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0) inventory.SelectNextItem();

        if (Input.GetKeyDown(KeyCode.Alpha1)) inventory.SelectItem(0);

        if (Input.GetKeyDown(KeyCode.Alpha2)) inventory.SelectItem(1);

        if (Input.GetKeyDown(KeyCode.Alpha3)) inventory.SelectItem(2);

        if (Input.GetKeyDown(KeyCode.Alpha4)) inventory.SelectItem(3);

        if (Input.GetKeyDown(KeyCode.F)) UseSelectedItem(inventory.GetSelectedItem());

        if (dropDelay <= 0 && Input.GetKey(KeyCode.G))
        {
            DropItem(inventory.PopSelectedItem());
            dropDelay = 1 / dropFrequency;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == worldItemLayer)
        {
            var worldItem = other.GetComponent<WorldItem>();
            if (worldItem.pickable && inventory.AddItem(worldItem.itemInfo)) Destroy(other.gameObject);
        }
    }

    private void DropItem(ItemInfo item)
    {
        if (!ReferenceEquals(item, null))
        {
            var dropPointPosition = dropPoint.position;
            var worldItem = Instantiate(item.worldItem, dropPointPosition,
                dropPoint.rotation);
            var direction = (dropDirectionPoint.position - dropPointPosition).normalized;
            worldItem.itemRigidbody.AddForce(direction * dropPower, ForceMode.Impulse);
        }
    }

    private void UseSelectedItem(ItemInfo item)
    {
        if (!ReferenceEquals(item, null) && playerMovementController.UseStaminaBooster(item))
            inventory.PopSelectedItem();
    }

    public void TakeItem(ItemInfo item)
    {
        if (!inventory.AddItem(item)) DropItem(item);
    }
}