using UnityEngine;

public class Sway : MonoBehaviour
{
    [Header("Position Settings")]
    [SerializeField] private float swayClamp = 0.09f;
    [SerializeField] private float positionSmoothing = 3f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationClamp = 15f; 
    [SerializeField] private float rotationSmoothing = 6f;
    [SerializeField] private float rotationMultiplier = 15f;

    [Header("Movement Sway")]
    [SerializeField] private float moveSwayAmount = 0.05f;
    [SerializeField] private float jumpSwayAmount = 0.1f;
    [SerializeField] private float moveSwaySpeed = 6f;

    private Vector3 originPosition;
    private Quaternion originRotation;
    private Quaternion lastCameraRotation;
    private Vector2 rotationVelocity;
    private PlayerController PlayerController
;
    void Start()
    {
        PlayerController = GetComponentInParent<PlayerController>();
        originPosition = transform.localPosition;
        originRotation = transform.localRotation;
        lastCameraRotation = Camera.main.transform.rotation;
    }

    void Update()
    {
        HandleSway();
    }
    
    void HandleSway()
    {
        if (!PlayerController.CanMove) return;

        HandlePositionSway();
        HandleRotationSway();
    }


    void HandlePositionSway()
    {
        Vector2 input = rotationVelocity * 0.02f;

        input.x = Mathf.Clamp(input.x, -swayClamp, swayClamp);
        input.y = Mathf.Clamp(input.y, -swayClamp, swayClamp);

        Vector3 targetPosition = new Vector3(-input.x, -input.y, 0);

        Vector3 movementSway = Vector3.zero;

        if (PlayerController.IsRunning)
        {
            movementSway = new Vector3(
                Mathf.Sin(Time.time * moveSwaySpeed) * moveSwayAmount,
                Mathf.Cos(Time.time * moveSwaySpeed * 0.5f) * moveSwayAmount,
                0
            );
        }
        else if (PlayerController.IsJumping) movementSway = new Vector3(0, Mathf.Sin(Time.time * moveSwaySpeed) * jumpSwayAmount, 0);

        Vector3 finalPosition = originPosition + targetPosition + movementSway;
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * positionSmoothing);
    }

    void HandleRotationSway()
    {
        Quaternion currentRotation = Camera.main.transform.rotation;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(lastCameraRotation);
        lastCameraRotation = currentRotation;

        Vector3 deltaEuler = deltaRotation.eulerAngles;

        if (deltaEuler.x > 180) deltaEuler.x -= 360;
        if (deltaEuler.y > 180) deltaEuler.y -= 360;
        if (deltaEuler.z > 180) deltaEuler.z -= 360;

        rotationVelocity = new Vector2(deltaEuler.y, deltaEuler.x);

        Vector2 input = rotationVelocity * 1f;

        float rotX = Mathf.Clamp(input.y * rotationMultiplier, -rotationClamp, rotationClamp);
        float rotY = Mathf.Clamp(input.x * rotationMultiplier, -rotationClamp, rotationClamp);
        float rotZ = Mathf.Clamp(input.x * rotationMultiplier, -rotationClamp, rotationClamp);

        Quaternion targetRotation = Quaternion.Euler(rotX, rotY, rotZ);
        Quaternion finalRotation = originRotation * targetRotation;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation, Time.deltaTime * rotationSmoothing);
    }


}