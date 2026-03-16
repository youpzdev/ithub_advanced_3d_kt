using UnityEngine;

public interface IInteractable
{
    
    string GetPromptText();
    void OnInteract();
    bool RequiresHold { get; }
    float HoldDuration { get; }
    void OnHoldInteract();
}

public interface IDraggable
{
    void Grab(Transform grabPoint);
    void Drop();
    void Throw(Camera cam, float forceMultiplier);
    void ShowOutline(bool show);
}