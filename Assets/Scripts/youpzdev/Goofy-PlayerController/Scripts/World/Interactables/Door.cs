using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;

    public string GetPromptText() => isOpen ? "Закрыть дверь" : "Открыть дверь";
    public bool RequiresHold => true;
    public float HoldDuration => 1.2f;

    public void OnInteract()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "Дверь открыта" : "Дверь закрыта");
    }

    public void OnHoldInteract()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "Дверь открыта" : "Дверь закрыта");
    }
}