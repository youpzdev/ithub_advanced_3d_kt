using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Crouch Settings")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float crouchSpeed = 1f;

    [Header("Movement Smoothing")]
    [SerializeField] private float accelerationTime = 0.1f;
    [SerializeField] private float decelerationTime = 0.15f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSpeed = 0.025f;
    [SerializeField] private float maxLookAngle = 80f;

    [SerializeField] private bool canMove = true;

    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 currentVelocity;
    private Vector3 velocitySmoothDamp;

    private float xRotation;
    private float startCrouchHeight = 1.8f;

    private bool isGrounded;
    private bool isCrouching;

    public bool IsRunning { get; private set; }
    public bool IsJumping => !isGrounded;
    public bool IsCrouching => isCrouching;
    public float CurrentSpeedPercent { get; private set; }
    public bool CanMove => canMove;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        startCrouchHeight = controller.height;
    }

    private void Update()
    {
        if (!canMove) return;
        HandleLook();
        HandleMovement();
        HandleCrouch();
    }

    private void HandleLook()
    {
        Vector2 lookInput = InputManager.Instance.Look;

        float mouseX = lookInput.x * lookSpeed / 100;
        float mouseY = lookInput.y * lookSpeed / 100;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector2 moveInput = InputManager.Instance.Move;
        bool jumpInput = InputManager.Instance.Jump;
        bool runInput = isCrouching ? false : InputManager.Instance.Run;

        Vector3 inputDirection = transform.right * moveInput.x +
                                 transform.forward * moveInput.y;
        inputDirection.Normalize();

        IsRunning = runInput && inputDirection.magnitude > 0.1f;

        float targetSpeed = IsRunning ? runSpeed : walkSpeed;
        Vector3 targetVelocity = inputDirection * targetSpeed;

        float smoothTime = inputDirection.magnitude > 0.1f ? accelerationTime : decelerationTime;

        currentVelocity = Vector3.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref velocitySmoothDamp,
            smoothTime
        );

        controller.Move(currentVelocity * Time.deltaTime);

        if (jumpInput && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        float horizontalSpeed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;
        CurrentSpeedPercent = Mathf.Clamp01(horizontalSpeed / runSpeed);

        InputManager.Instance.ConsumeInputs();
    }

    private void HandleCrouch()
    {
        isCrouching = InputManager.Instance.Crouch;
        float targetHeight = isCrouching ? crouchHeight : startCrouchHeight;

        if (Mathf.Abs(controller.height - targetHeight) > 0.001f)
        {
            float lastHeight = controller.height;
            float lerpStep = 1f - Mathf.Exp(-crouchSpeed * Time.deltaTime);
            controller.height = Mathf.Lerp(controller.height, targetHeight, lerpStep);

            float heightDifference = controller.height - lastHeight;

            controller.center += new Vector3(0, heightDifference / 2f, 0);

            Vector3 visualPos = visualRoot.localPosition;
            visualPos.y += heightDifference / 2f;
            visualRoot.localPosition = visualPos;
        }
        else
        {
            controller.height = targetHeight;
        }
    }
}