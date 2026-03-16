using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(FloorButton))]
public class KillZone : MonoBehaviour
{
    [SerializeField] private float deathDelay = 1.5f;
    [SerializeField] private string deathMessage = "lmao you stupid";

    private void Awake()
    {
        GetComponent<FloorButton>().OnPressed += HandleFall;
    }

    private void HandleFall()
    {
        InformationPopup.Instance?.Show(deathMessage);

        DOVirtual.DelayedCall(deathDelay, () => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }
}
