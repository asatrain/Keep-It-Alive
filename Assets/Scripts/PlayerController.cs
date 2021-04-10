using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private CharacterController characterController;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float staminaMaxDuration;
    [SerializeField] private float staminaMinRequiredToRun;
    [SerializeField] private float runSpeedMultiplier;
    [SerializeField] private float rotationSpeedDegrees;
    [SerializeField] private Transform cameraTransform;

    [Header("Chopping")] [SerializeField] private PlayerAnimationController playerAnimationController;
    [SerializeField] private float chopDelay;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitCapsuleHeight;
    [SerializeField] private float hitCapsuleRadius;

    [Header("UI")] [SerializeField] private Slider staminaSlider;
    [SerializeField] private RectTransform sliderRectTransform;
    [SerializeField] private RectTransform sliderSeparatorRectTransform;

    private float staminaTimeLeft;
    private bool wasRunningPrevFrame;
    private bool choppingRequired;
    private bool choppingStarted;

    private readonly Collider[] overlapResults = new Collider[10];

    private void Start()
    {
        staminaTimeLeft = staminaMaxDuration;
        float separatorCoefficient = staminaMinRequiredToRun / staminaMaxDuration;
        float sliderWidth = sliderRectTransform.rect.width;
        float separatorPosX = sliderWidth * separatorCoefficient - sliderWidth / 2;
        Vector3 separatorLocalPosition = sliderSeparatorRectTransform.localPosition;
        separatorLocalPosition = new Vector3(separatorPosX, separatorLocalPosition.y, separatorLocalPosition.z);
        sliderSeparatorRectTransform.localPosition = separatorLocalPosition;
    }

    private void Update()
    {
        if (choppingRequired)
        {
            if (playerAnimationController.IsChopping()) // Chop in process
            {
                choppingStarted = true;
                AddStaminaTimeLeft(Time.deltaTime);
                return;
            }

            if (choppingStarted) // Chop ended
            {
                choppingRequired = false;
                choppingStarted = false;
            }
            else // Chop preparing
            {
                AddStaminaTimeLeft(Time.deltaTime);
                return;
            }

            wasRunningPrevFrame = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            RequireChop();
        }

        ControlMovement();
    }

    private void AddStaminaTimeLeft(float timeToAdd)
    {
        staminaTimeLeft += timeToAdd;
        staminaTimeLeft = Mathf.Max(0, Mathf.Min(staminaMaxDuration, staminaTimeLeft));
        staminaSlider.value = staminaTimeLeft / staminaMaxDuration;
    }

    private void ControlMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
        Vector3 inputAxisDirection = new Vector3(horizontal, 0, vertical).normalized;
        if (inputAxisDirection.magnitude != 0)
        {
            if (shiftPressed && (staminaTimeLeft > 0 && wasRunningPrevFrame ||
                                 staminaTimeLeft >= staminaMinRequiredToRun))
            {
                playerAnimationController.SetState(PlayerAnimationController.State.Run);
                Move(inputAxisDirection, walkSpeed * runSpeedMultiplier);
                AddStaminaTimeLeft(-Time.deltaTime);
                wasRunningPrevFrame = true;
            }
            else
            {
                playerAnimationController.SetState(PlayerAnimationController.State.Walk);
                Move(inputAxisDirection, walkSpeed);
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
        float targetAngle = Mathf.Atan2(inputAxisDirection.x, inputAxisDirection.z) * Mathf.Rad2Deg +
                            cameraTransform.eulerAngles.y;
        Vector3 moveDirection = (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized;
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
        Vector3 hitPointPosition = hitPoint.position;
        Vector3 point0 = hitPointPosition;
        point0.y -= hitCapsuleHeight / 2;
        Vector3 point1 = hitPointPosition;
        point1.y += hitCapsuleHeight / 2;
        int size = Physics.OverlapCapsuleNonAlloc(point0, point1, hitCapsuleRadius, overlapResults);
        for (int i = 0; i < size; i++)
        {
            if (overlapResults[i].CompareTag("Tree"))
            {
                overlapResults[i].GetComponent<TreeStageController>().ChopTree();
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(hitPoint.position, hitCapsuleRadius);
    }
}