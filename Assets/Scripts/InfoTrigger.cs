using UnityEngine;

public class InfoTrigger : MonoBehaviour
{
    [SerializeField] private FloorButton floorButton;

    [SerializeField][TextArea(5, 10)] private string textToShow;

    private void Start()
    {
        floorButton.OnPressed += Show;
        floorButton.OnReleased += Hide;
    }

    private void OnDestroy()
    {
        floorButton.OnPressed -= Show;
        floorButton.OnReleased -= Hide;
    }


    private void Show() => InformationPopup.Instance.Show(textToShow);
    private void Hide() => InformationPopup.Instance.Hide();

}
