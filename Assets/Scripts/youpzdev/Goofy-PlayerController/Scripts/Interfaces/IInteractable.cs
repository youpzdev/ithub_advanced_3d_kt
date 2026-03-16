// InteractableObject.cs
using UnityEngine;

public interface IInteractable
{
    string GetPromptText();      // Текст подсказки как бы
    void OnInteract();           // Быстрое взаимодействие (нажать E)
    bool RequiresHold { get; }   // Нужно ли зажимать E?
    float HoldDuration { get; }  // Сколько секунд зажимать
    void OnHoldInteract();       // Взаимодействие по зажатию
}