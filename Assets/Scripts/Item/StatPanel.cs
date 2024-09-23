using UnityEngine;

public class StatPanel : MonoBehaviour
{
    [SerializeField] StatDisplay[] statDisplays;

    private void OnValidate()
    {
        statDisplays = GetComponentsInChildren<StatDisplay>();
    }
}
