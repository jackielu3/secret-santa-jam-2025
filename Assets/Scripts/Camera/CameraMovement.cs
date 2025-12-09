using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform followTarget;

    [SerializeField] float rotationSpeed = 200f;
    [SerializeField] float bottomClamp = -40f;
    [SerializeField] float topClamp = 70f;

    private float cinemachineTargetPitch;
    private float cinemachineTargetYaw;

    private void Start()
    {
        var euler = followTarget.rotation.eulerAngles;
        cinemachineTargetYaw = euler.y;
        cinemachineTargetPitch = euler.x;
    }

    private void LateUpdate()
    {
        CameraLogic();
    }

    private void CameraLogic()
    {
        float mouseX = GetMouseInput("Mouse X");
        float mouseY = GetMouseInput("Mouse Y");

        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, mouseY, bottomClamp, topClamp, true);
        cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotation(cinemachineTargetPitch, cinemachineTargetYaw);
    }

    private void ApplyRotation(float pitch, float yaw)
    {
        // Smooth the rotation using Slerp for more responsiveness
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, followTarget.eulerAngles.z);
        followTarget.rotation = Quaternion.Slerp(followTarget.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private float UpdateRotation(float currentRotation, float input, float min, float max, bool invertInput)
    {
        currentRotation += invertInput ? -input : input;
        return Mathf.Clamp(currentRotation, min, max);
    }

    private float GetMouseInput(string axis)
    {
        return Input.GetAxisRaw(axis) * rotationSpeed; // Use raw input for better responsiveness
    }
}
