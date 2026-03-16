using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private float interactCooldown = 0.5f;

    [Header("UI")]
    [SerializeField] private CanvasGroup interactableCanvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private Image holdProgressBar;
    [SerializeField] private Image BackgroundProgressBar;

    private IInteractable currentInteractable;
    private IDraggable currentDraggable;
    private float holdTimer;
    private bool isHolding;
    private float cooldownTimer;
    private bool uiVisible;
    private Coroutine fadeCoroutine;

    private bool IsOnCooldown => cooldownTimer > 0f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        SetUIActive(false);
        interactableCanvasGroup.alpha = 0;
    }

    void Update()
    {
        if (IsOnCooldown) cooldownTimer -= Time.deltaTime;
        DetectInteractable();
        HandleInput();
        UpdateHoldUI();
    }

    private void DetectInteractable()
    {
        if (PlayerPickUpDrop.Instance.IsGrabbing)
        {
            if (currentInteractable != null) ResetCurrent();
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        bool found = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer) && IsVisible(hit))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                found = true;
                if (interactable != currentInteractable)
                {
                    ResetCurrent();
                    currentInteractable = interactable;
                    currentDraggable = hit.collider.GetComponentInParent<IDraggable>();
                    currentDraggable?.ShowOutline(true);
                    ShowPrompt(currentInteractable);
                }
            }
        }

        if (!found && currentInteractable != null) ResetCurrent();
        if (found != uiVisible) { uiVisible = found; FadeUI(uiVisible); }
    }

    private void HandleInput()
    {
        if (currentInteractable == null || IsOnCooldown) return;

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
        else if (InputManager.Instance.Interact)
        {
            currentInteractable.OnInteract();
            InputManager.Instance.ConsumeInteract();
            StartCooldown();
            ResetCurrent();
        }
    }

    private void UpdateHoldUI()
    {
        if (holdProgressBar == null) return;
        bool show = isHolding && currentInteractable != null && currentInteractable.RequiresHold && !IsOnCooldown;
        holdProgressBar.gameObject.SetActive(show);
        BackgroundProgressBar.gameObject.SetActive(show);
        if (show) holdProgressBar.fillAmount = holdTimer / currentInteractable.HoldDuration;
    }

    private void ShowPrompt(IInteractable interactable)
    {
        if (itemNameText != null) itemNameText.text = interactable.GetPromptText();
        if (interactionText != null) interactionText.text = interactable.RequiresHold
            ? $"[Держать E] {interactable.GetPromptText()} ({interactable.HoldDuration}с)"
            : $"[E] {interactable.GetPromptText()}";
    }

    private void ResetCurrent()
    {
        currentDraggable?.ShowOutline(false);
        currentDraggable = null;
        currentInteractable = null;
        ResetHold();
        SetUIActive(false);
    }

    private void ResetHold()
    {
        holdTimer = 0f;
        isHolding = false;
    }

    private void StartCooldown() => cooldownTimer = interactCooldown;

    private bool IsVisible(RaycastHit hit)
    {
        Vector3 dir = (hit.point - playerCamera.transform.position).normalized;
        float dist = Vector3.Distance(playerCamera.transform.position, hit.point);
        return !Physics.Raycast(playerCamera.transform.position, dir, dist, obstacleLayer);
    }

    private void SetUIActive(bool active)
    {
        if (itemNameText != null) itemNameText.gameObject.SetActive(active);
        if (interactionText != null) interactionText.gameObject.SetActive(active);
    }

    private void FadeUI(bool fadeIn)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCoroutine(fadeIn));
    }

    private IEnumerator FadeCoroutine(bool fadeIn)
    {
        float target = fadeIn ? 1f : 0f;
        float start = interactableCanvasGroup.alpha;
        float t = 0f;
        if (fadeIn) SetUIActive(true);
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            interactableCanvasGroup.alpha = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / fadeDuration));
            yield return null;
        }
        interactableCanvasGroup.alpha = target;
        if (!fadeIn) SetUIActive(false);
        fadeCoroutine = null;
    }

    public void ForceHide()
    {
        ResetCurrent();
        FadeUI(false);
        uiVisible = false;
    }
}