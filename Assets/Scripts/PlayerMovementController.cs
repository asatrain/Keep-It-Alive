using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private CharacterController characterController;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeedMultiplier;
    [SerializeField] private float rotationSpeedDegrees;
    [SerializeField] private Transform cameraTransform;

    [Header("Stamina")] [SerializeField] private float defaultStaminaMaxDuration;
    [SerializeField] private float defaultStaminaMinRequiredToRun;

    [Header("Chopping")] [SerializeField] private PlayerAnimationController playerAnimationController;
    [SerializeField] private PlayerInventoryController playerInventoryController;
    [SerializeField] private float chopDelay;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitCapsuleRadius;

    [Header("UI")] [SerializeField] private Slider staminaSlider;
    [SerializeField] private RectTransform sliderRectTransform;
    [SerializeField] private RectTransform sliderSeparatorRectTransform;
    [SerializeField] private Slider staminaDurationSlider;
    [SerializeField] private Image staminaDurationSliderFillImage;

    private readonly Collider[] overlapResults = new Collider[10];
    private int chestLayer;
    private LayerMask chopMask;
    private bool choppingRequired;
    private bool choppingStarted;
    private int chopPower;
    private float staminaBoostMultiplier;
    private float staminaBoostTime;
    private float staminaBoostTimeLeft;

    private float staminaTimeLeft;
    private int treeLayer;

    private bool wasRunningPrevFrame;

    private void Start()
    {
        treeLayer = LayerMask.NameToLayer("Tree");
        chestLayer = LayerMask.NameToLayer("Chest");
        chopMask = LayerMask.GetMask("Tree", "Chest");

        staminaTimeLeft = defaultStaminaMaxDuration;
        staminaBoostMultiplier = 1;
        staminaBoostTime = 0;
        staminaBoostTimeLeft = 0;
        chopPower = 5;
        var separatorCoefficient = defaultStaminaMinRequiredToRun / defaultStaminaMaxDuration;
        var sliderWidth = sliderRectTransform.rect.width;
        var separatorPosX = sliderWidth * separatorCoefficient - sliderWidth / 2;
        var separatorLocalPosition = sliderSeparatorRectTransform.localPosition;
        separatorLocalPosition = new Vector3(separatorPosX, separatorLocalPosition.y, separatorLocalPosition.z);
        sliderSeparatorRectTransform.localPosition = separatorLocalPosition;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;

        UpdateUsedStamina();
        if (UpdateChoppingState()) return;
        if (Input.GetKey(KeyCode.Space)) RequireChop();
        ControlMovement();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(hitPoint.position, hitCapsuleRadius);
    }

    private void UpdateUsedStamina()
    {
        if (staminaBoostTimeLeft > 0)
        {
            staminaBoostTimeLeft -= Time.deltaTime;
            staminaDurationSlider.value = staminaBoostTimeLeft / staminaBoostTime;
        }
        else
        {
            staminaBoostMultiplier = 1;
            chopPower = 1;
            staminaDurationSlider.gameObject.SetActive(false);
        }
    }

    private bool UpdateChoppingState()
    {
        if (choppingRequired)
        {
            if (playerAnimationController.IsChopping()) // Chop in process
            {
                choppingStarted = true;
                AddStaminaTimeLeft(Time.deltaTime);
                return true;
            }

            if (choppingStarted) // Chop ended
            {
                choppingRequired = false;
                choppingStarted = false;
            }
            else // Chop preparing
            {
                AddStaminaTimeLeft(Time.deltaTime);
                return true;
            }

            wasRunningPrevFrame = false;
        }

        return false;
    }

    private void AddStaminaTimeLeft(float timeToAdd)
    {
        staminaTimeLeft += timeToAdd;
        staminaTimeLeft = Mathf.Clamp(staminaTimeLeft, 0, defaultStaminaMaxDuration);
        staminaSlider.value = staminaTimeLeft / defaultStaminaMaxDuration;
    }

    private void ControlMovement()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var shiftPressed = Input.GetKey(KeyCode.LeftShift);
        var inputAxisDirection = new Vector3(horizontal, 0, vertical).normalized;
        if (inputAxisDirection.magnitude != 0)
        {
            if (shiftPressed && (staminaTimeLeft > 0 && wasRunningPrevFrame ||
                                 staminaTimeLeft >= defaultStaminaMinRequiredToRun))
            {
                playerAnimationController.SetState(PlayerAnimationController.State.Run);
                Move(inputAxisDirection, walkSpeed * runSpeedMultiplier * staminaBoostMultiplier);
                AddStaminaTimeLeft(-Time.deltaTime / staminaBoostMultiplier);
                wasRunningPrevFrame = true;
            }
            else
            {
                playerAnimationController.SetState(PlayerAnimationController.State.Walk);
                Move(inputAxisDirection, walkSpeed * staminaBoostMultiplier);
                AddStaminaTimeLeft(Time.deltaTime);
                wasRunningPrevFrame = false;
            }
        }
        else
        {
            playerAnimationController.SetState(PlayerAnimationController.State.Idle);
            AddStaminaTimeLeft(Time.deltaTime);
            wasRunningPrevFrame = false;
        }
    }

    private void Move(Vector3 inputAxisDirection, float moveSpeed)
    {
        var targetAngle = Mathf.Atan2(inputAxisDirection.x, inputAxisDirection.z) * Mathf.Rad2Deg +
                          cameraTransform.eulerAngles.y;
        var moveDirection = (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized;
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection),
                Time.deltaTime * rotationSpeedDegrees);
        characterController.Move((moveDirection + Physics.gravity) * (moveSpeed * Time.deltaTime));
    }

    private void RequireChop()
    {
        playerAnimationController.SetState(PlayerAnimationController.State.Chop);
        choppingRequired = true;
        Invoke(nameof(Chop), chopDelay);
    }

    private void Chop()
    {
        var size = Physics.OverlapSphereNonAlloc(hitPoint.position, hitCapsuleRadius, overlapResults, chopMask);
        for (var i = 0; i < size; i++)
        {
            var layer = overlapResults[i].gameObject.layer;
            if (layer == treeLayer)
            {
                var item = overlapResults[i].GetComponent<TreeStageController>()
                    .ChopTree(chopPower, out var resultCount);
                for (var j = 0; j < resultCount; j++)
                    playerInventoryController.TakeItem(item);
                return;
            }

            if (layer == chestLayer)
            {
                overlapResults[i].GetComponent<ChestController>().Hit();
                return;
            }
        }
    }

    public bool UseStaminaBooster(ItemInfo itemInfo)
    {
        if (!itemInfo.isStaminaBooster)
            return false;

        staminaBoostMultiplier = itemInfo.staminaBoostMultiplier;
        staminaBoostTime = itemInfo.staminaBoostTime;
        staminaBoostTimeLeft = staminaBoostTime;
        chopPower = itemInfo.chopPowerBoost;
        staminaDurationSlider.gameObject.SetActive(true);
        staminaDurationSliderFillImage.color = itemInfo.staminaSliderColor;
        staminaTimeLeft = defaultStaminaMaxDuration;
        return true;
    }
}