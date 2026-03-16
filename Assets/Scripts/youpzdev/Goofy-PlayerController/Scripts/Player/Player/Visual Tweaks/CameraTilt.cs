using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private HeadBob headBob;

    [Header("Tilt Settings")]
    [SerializeField] private float horizontalTiltAngle = 0.025f;
    [SerializeField] private float verticalTiltAngle = 0.025f;
    [SerializeField] private float tiltSpeed = 5f;

    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        headBob.DoHeadBob(playerController.CurrentSpeedPercent);
        HandleCameraTilt();
    }

    private void HandleCameraTilt()
    {
        float horizontalInput = InputManager.Instance.Look.x;
        float verticalInput = InputManager.Instance.Look.y;

        float targetZRotation = -horizontalInput * horizontalTiltAngle / 100;
        float targetXRotation = verticalInput * verticalTiltAngle / 100;

        Quaternion targetRotation = Quaternion.Euler(targetXRotation, 0f, targetZRotation);
        cameraHolder.localRotation = Quaternion.Lerp(cameraHolder.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}
