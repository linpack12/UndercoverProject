using UnityEngine;

public class AutoScan : MonoBehaviour
{
    private PerceptionModule _perception;

    private void Awake()
    {
        _perception = GetComponent<PerceptionModule>();
        InvokeRepeating(nameof(TryScan), 1f, 2f);
    }

    private void TryScan()
    {
        _perception?.Scan();
    }
}