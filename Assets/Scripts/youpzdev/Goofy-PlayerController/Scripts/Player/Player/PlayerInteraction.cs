// PlayerInteraction.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactCooldown = 0.5f; // КД после взаимодействия

    [Header("UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject holdProgressPanel;
    [SerializeField] private Image holdProgressBar;

    private IInteractable currentInteractable;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private float cooldownTimer = 0f; // текущий КД

    private bool IsOnCooldown => cooldownTimer > 0f;

    void Update()
    {
        // Тикаем КД независимо от всего
        if (IsOnCooldown)
            cooldownTimer -= Time.deltaTime;

        DetectInteractable();
        HandleInput();
        UpdateHoldUI();
    }

    void DetectInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                ShowPrompt(interactable);
                return;
            }
        }

        currentInteractable = null;
        HidePrompt();
        ResetHold();
    }

    void HandleInput()
    {
        if (currentInteractable == null || IsOnCooldown)
        {
            ResetHold();
            return;
        }

        if (currentInteractable.RequiresHold)
        {
            if (InputManager.Instance.Interact)
            {
                isHolding = true;
                holdTimer += Time.deltaTime;

                if (holdTimer >= currentInteractable.HoldDuration)
                {
                    currentInteractable.OnHoldInteract();
                    StartCooldown();
                    ResetHold();
                }
            }
            else
            {
                ResetHold();
            }
        }
        else
        {
            if (InputManager.Instance.Interact)
            {
                currentInteractable.OnInteract();
                InputManager.Instance.ConsumeInteract();
                StartCooldown();
            }
        }
    }

    void UpdateHoldUI()
    {
        if (holdProgressPanel == null) return;

        bool showHoldUI = isHolding && currentInteractable != null
                          && currentInteractable.RequiresHold && !IsOnCooldown;

        holdProgressPanel.SetActive(showHoldUI);

        if (showHoldUI && holdProgressBar != null)
            holdProgressBar.fillAmount = holdTimer / currentInteractable.HoldDuration;
    }

    void ShowPrompt(IInteractable interactable)
    {
        if (promptPanel != null) promptPanel.SetActive(true);

        if (promptText != null)
        {
            // Во время КД показываем отдельный текст
            if (IsOnCooldown)
            {
                promptText.text = $"...({cooldownTimer:F1}с)";
                return;
            }

            string hint = interactable.RequiresHold
                ? $"[Зажмите E]\n{interactable.GetPromptText()}\n({interactable.HoldDuration}с)"
                : $"[E] {interactable.GetPromptText()}";

            promptText.text = hint;
        }
    }

    void HidePrompt()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    void ResetHold()
    {
        holdTimer = 0f;
        isHolding = false;
    }

    void StartCooldown()
    {
        cooldownTimer = interactCooldown;
    }
}