using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] private float appearRangeStart;
    [SerializeField] private float appearRangeEnd;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform bonfireTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Image arrowImage;

    private void Update()
    {
        UpdateCompassRotation();
    }

    private void UpdateCompassRotation()
    {
        var playerPosition = playerTransform.position;
        var bonfirePosition = bonfireTransform.position;
        var distance = (playerPosition - bonfirePosition).magnitude;
        var alpha = Mathf.Clamp01((distance - appearRangeStart) / (appearRangeEnd - appearRangeStart));
        var arrowImageColor = arrowImage.color;
        arrowImageColor.a = alpha;
        arrowImage.color = arrowImageColor;
        var cameraForwardDirection = cameraTransform.forward;
        var cameraUpDirection = cameraTransform.up;
        var bonfireDirectionProjected =
            Vector3.ProjectOnPlane(bonfirePosition - playerPosition, cameraForwardDirection);
        var angle = Vector3.SignedAngle(cameraUpDirection, bonfireDirectionProjected, cameraForwardDirection);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}