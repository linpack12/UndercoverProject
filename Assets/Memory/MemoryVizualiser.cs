using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(MemoryModule))]
public class MemoryVisualizer : MonoBehaviour
{
    private MemoryModule memoryModule;
    private Renderer npcRenderer;

    private void Awake()
    {
        memoryModule = GetComponent<MemoryModule>();
        npcRenderer = GetComponentInChildren<Renderer>();

        if (npcRenderer == null)
        {
            Debug.LogWarning($"[{name}] No Renderer found for MemoryVisualizer");
        }

        memoryModule.OnMemoryAdded += OnMemoryChanged;
    }

    private void OnDestroy()
    {
        if (memoryModule != null)
            memoryModule.OnMemoryAdded -= OnMemoryChanged;
    }

    private void OnMemoryChanged(MemoryEntry memory)
    {
        if (npcRenderer == null) return;

        Color color;

        if (memory.WasGossiped)
        {
            color = Color.Lerp(Color.gray, Color.blue, memory.Confidence); // Blue for gossip
        }
        else
        {
            color = Color.Lerp(Color.red, Color.green, memory.Confidence); // Green for non gossip
        }

        npcRenderer.material.color = color;

        Debug.Log($"[MemoryVisualizer] Updated color based on memory: {memory.Confidence:F2} | Gossiped: {memory.WasGossiped}");
    }
}