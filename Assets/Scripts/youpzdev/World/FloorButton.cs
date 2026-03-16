using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FloorButton : MonoBehaviour
{
    [SerializeField] private LayerMask objectLayer;

    public event Action OnPressed;
    public event Action OnReleased;

    private BoxCollider _collider;
    private bool _isPressed;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    private void FixedUpdate()
    {
        Vector3 worldCenter = transform.TransformPoint(_collider.center);
        Vector3 halfExtents = Vector3.Scale(_collider.size, transform.lossyScale) / 2f;

        bool occupied = Physics.CheckBox(worldCenter, halfExtents, transform.rotation, objectLayer);

        if (occupied && !_isPressed)
        {
            _isPressed = true;
            OnPressed?.Invoke();
        }
        else if (!occupied && _isPressed)
        {
            _isPressed = false;
            OnReleased?.Invoke();
        }
    }
}