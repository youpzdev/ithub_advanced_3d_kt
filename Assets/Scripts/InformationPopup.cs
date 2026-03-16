using UnityEngine;
using TMPro;

public class InformationPopup : MonoBehaviour
{
    public static InformationPopup Instance { get; private set; }

    [SerializeField] private UIPanel panel;
    [SerializeField] private TMP_Text text;


    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Show(string message, float time = 0f)
    {
        text.text = message;
        panel.Show();

        if (time != 0) Timer.After(time, Hide);
    }

    public void Hide() => panel.Hide();

}