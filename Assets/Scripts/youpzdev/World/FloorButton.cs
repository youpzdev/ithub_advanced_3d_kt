using UnityEngine;

public class FloorButton : MonoBehaviour
{
    [SerializeField] private GameObject target;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        target.SetActive(false);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        target.SetActive(true);
    }
}
