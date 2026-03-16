using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    public static PlayerPickUpDrop Instance { get; private set; }

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float pickupDistance = 2f;

    private IDraggable grabbed;

    public bool IsGrabbing => grabbed != null;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (InputManager.Instance.Interact && grabbed == null)
        {
            TryGrab();
        }

        if (!InputManager.Instance.Interact && grabbed != null)
        {
            grabbed.Drop();
            grabbed = null;
        }

        if (InputManager.Instance.Fire && grabbed != null)
        {
            grabbed.Throw(playerCamera, 1f);
            grabbed = null;
            InputManager.Instance.ConsumeInputs();
        }
    }

    private void TryGrab()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, pickupDistance, pickupLayer)) return;

        IDraggable draggable = hit.collider.GetComponentInParent<IDraggable>();
        if (draggable == null) return;

        grabbed = draggable;
        grabbed.Grab(grabPoint);
        PlayerInteraction.Instance.ForceHide();
    }

    public void ForceRelease()
    {
        if (grabbed == null) return;
        grabbed.Drop();
        grabbed = null;
    }
}
