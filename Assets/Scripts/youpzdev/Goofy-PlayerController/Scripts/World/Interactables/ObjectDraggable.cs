using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectDraggable : MonoBehaviour, IInteractable, IDraggable
{
    [SerializeField] private string itemName;
    [SerializeField] private float weight = 1f;

    [Header("Drag Settings")]
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float throwUpwardForce = 1f;
    [SerializeField] private float maxDistance = 0.45f;
    [SerializeField] private float springForce = 100f;
    [SerializeField] private float damping = 10f;

    private Rigidbody rb;
    private Transform grabPoint;
    private bool canPickUp = true;
    private float cooldownTimer;
    private bool canDrop;

    private const float PickupCooldown = 0.25f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.mass = weight;
        springForce /= weight;
        throwForce /= weight;
        throwUpwardForce /= weight;
    }

    void Update()
    {
        if (canPickUp) return;
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f) canPickUp = true;
    }

    void FixedUpdate()
    {
        if (grabPoint == null) return;

        Vector3 displacement = grabPoint.position - transform.position;
        rb.AddForce(displacement * (springForce / rb.mass) - rb.linearVelocity * damping, ForceMode.Acceleration);

        float dist = displacement.magnitude;
        if (dist <= 0.25f && !canDrop) canDrop = true;
        if (canDrop && dist > maxDistance) Drop();
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<PlayerInteraction>(out _)) return;
        Drop();
        SetPickupCooldown();
    }

    public void Grab(Transform point)
    {
        if (!canPickUp) return;
        grabPoint = point;
        rb.useGravity = false;
    }

    public void Drop()
    {
        grabPoint = null;
        rb.useGravity = true;
        canDrop = false;
    }

    public void Throw(Camera cam, float forceMultiplier = 1f)
    {
        if (grabPoint == null) return;
        Drop();
        rb.AddForce((cam.transform.forward * throwForce + Vector3.up * (throwUpwardForce / 2f)) * forceMultiplier, ForceMode.VelocityChange);
        SetPickupCooldown();
    }

    public void ShowOutline(bool show) { }

    public bool RequiresHold => false;
    public float HoldDuration => 0f;
    public string GetPromptText() => itemName;
    public void OnInteract() { }
    public void OnHoldInteract() { }

    private void SetPickupCooldown()
    {
        canPickUp = false;
        cooldownTimer = PickupCooldown;
    }
}