using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _scaleFactor = 0.85f;
    [SerializeField] private float _duration = 0.25f;

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(_scaleFactor, _duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1f, _duration).SetEase(Ease.OutBack);
    }

    public void PlayClickAnimation()
    {
        transform.DOKill();
        transform.DOScale(_scaleFactor, _duration / 2).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOScale(1f, _duration / 2).SetEase(Ease.OutBack);
        });
    }
}