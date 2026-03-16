using DG.Tweening;
using UnityEngine;

public class SlidingDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private FloorButton floorButton;
    [SerializeField] private Vector3 openOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    public string GetPromptText() => "Heavy Door";
    public bool RequiresHold => true;
    public float HoldDuration => 1.2f;

    private Vector3 _closedPos;
    private bool _isOpen;

    private void Start()
    {
        _closedPos = transform.position;
        floorButton.OnPressed += OpenDoor;
        floorButton.OnReleased += CloseDoor;
    }

    private void OnDestroy()
    {
        floorButton.OnPressed -= OpenDoor;
        floorButton.OnReleased -= CloseDoor;
    }

    private void OpenDoor()
    {
        if (_isOpen) return;
        _isOpen = true;
        transform.DOMove(_closedPos + openOffset, duration).SetEase(ease);
    }

    private void CloseDoor()
    {
        if (!_isOpen) return;
        _isOpen = false;
        transform.DOMove(_closedPos, duration).SetEase(ease);
    }

    public void OnInteract() { }
    public void OnHoldInteract() { }
}