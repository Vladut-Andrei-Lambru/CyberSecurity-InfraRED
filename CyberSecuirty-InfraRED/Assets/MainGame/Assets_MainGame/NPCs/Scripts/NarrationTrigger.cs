using UnityEngine;

public class NarrationTrigger : MonoBehaviour
{
    public enum TriggerMode
    {
        StartOnly,   
        ListenEvent  
    }

    [Header("Trigger")]
    [SerializeField] private TriggerMode mode = TriggerMode.StartOnly;

    [Tooltip("Event id to consume and play narration for.")]
    [SerializeField] private string triggerEventId = "INTRO";

    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private NarrationSequenceSO sequence;

    [Header("Behavior")]
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool disableAfterTrigger = true;

    [Header("Listen Mode Settings")]
    [Tooltip("If ListenEvent, keep checking for this long (unscaled).")]
    [SerializeField, Min(0.25f)] private float listenWindowSeconds = 999f;

    private bool triggered;
    private float enabledAt;

    private void OnEnable()
    {
        enabledAt = Time.unscaledTime;
        
        SaveManager.EnsureLoaded();
        
        if (mode == TriggerMode.ListenEvent)
        {
            SaveManager.OnSaveChanged -= OnSaveChanged;
            SaveManager.OnSaveChanged += OnSaveChanged;
        }
    }

    private void OnDisable()
    {
        SaveManager.OnSaveChanged -= OnSaveChanged;
    }

    private void Start()
    {
        if (mode == TriggerMode.StartOnly)
            TryTrigger();
    }

    private void Update()
    {
        if (mode != TriggerMode.ListenEvent) return;
        if (triggerOnce && triggered) return;
        
        if (listenWindowSeconds > 0f && (Time.unscaledTime - enabledAt) > listenWindowSeconds)
            return;
        
        TryTrigger();
    }

    private void OnSaveChanged()
    {
        if (mode == TriggerMode.ListenEvent)
            TryTrigger();
    }

    private void TryTrigger()
    {
        if (triggerOnce && triggered) return;

        string key = string.IsNullOrWhiteSpace(triggerEventId) ? "" : triggerEventId.Trim();
        if (string.IsNullOrEmpty(key)) return;
        
        if (dialogueUI == null)
            dialogueUI = FindFirstObjectByType<DialogueUI>();

        if (dialogueUI == null || sequence == null)
            return;

        if (!SaveManager.HasMainEvent(key))
            return;

        triggered = true;
        
        SaveManager.RemoveMainEvent(key);

        dialogueUI.OpenNarration(sequence);

        if (disableAfterTrigger)
            gameObject.SetActive(false);
    }
}