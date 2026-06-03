// FInishPlatform.cs
using UnityEngine;

/// <summary>
/// Legacy finish trigger (optional).
/// In the new security-driven flow, the level ends when Security hits 100%.

/// </summary>
[DisallowMultipleComponent]
public sealed class FInishPlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance?.Win();
    }
}