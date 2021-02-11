using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeedDegrees;
    [SerializeField] private Transform cameraTransform;
    
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputAxisDirection = new Vector3(horizontal, 0, vertical).normalized;
        if (inputAxisDirection.magnitude != 0)
        {
            float targetAngle = Mathf.Atan2(inputAxisDirection.x, inputAxisDirection.z) * Mathf.Rad2Deg +
                                cameraTransform.eulerAngles.y;
            Vector3 moveDirection = (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized;
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection),
                    Time.deltaTime * rotationSpeedDegrees);
            characterController.Move((moveDirection + Physics.gravity) * (speed * Time.deltaTime));
        }
    }
}