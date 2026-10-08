using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class CutsceneSystem : MonoBehaviour
{
    public static CutsceneSystem Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup videoGroup;   // container for your RawImage + optional skip button
    [SerializeField] private RawImage videoImage;      // displays the cutscene RenderTexture (recommended)
    [SerializeField] private Image fadeImage;          // full-screen black image (alpha fade)

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;  // set Render Mode = Render Texture (recommended)
    [SerializeField] private AudioSource videoAudioSource; // optional, if you route video audio here

    [Header("Music (optional)")]
    [SerializeField] private AudioSource musicSource;  // music to pause during cutscene

    [Header("UI Click Lock")]
    [Tooltip("If assigned, this EventSystem will be disabled during cutscene to prevent UI clicks.")]
    [SerializeField] private EventSystem eventSystem;

    [Header("Fade")]
    [SerializeField, Min(0f)] private float fadeOutSeconds = 0.35f;
    [SerializeField, Min(0f)] private float fadeInSeconds = 0.35f;

    private bool isPlaying;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (videoGroup != null) videoGroup.alpha = 0f;
        SetFadeAlpha(0f);

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Plays a cutscene, then loads the target scene (Single).
    /// Disables UI clicks during playback by disabling EventSystem.
    /// </summary>
    public void PlayAndLoadScene(VideoClip clip, int sceneBuildIndex)
    {
        if (isPlaying) return;

        // No clip -> just load
        if (clip == null)
        {
            SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
            return;
        }

        StartCoroutine(CoPlayAndLoad(clip, sceneBuildIndex));
    }

    private IEnumerator CoPlayAndLoad(VideoClip clip, int sceneBuildIndex)
    {
        isPlaying = true;

        // Disable UI clicks
        SetUIClicksEnabled(false);

        // Pause ONLY music
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();

        // Fade to black
        yield return FadeTo(1f, fadeOutSeconds);

        // Show video UI
        if (videoGroup != null) videoGroup.alpha = 1f;

        // Configure video
        if (videoPlayer != null)
        {
            videoPlayer.clip = clip;

            
            if (videoAudioSource != null)
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                videoPlayer.SetTargetAudioSource(0, videoAudioSource);
            }

            // If using RenderTexture mode, RawImage should already have the same RenderTexture assigned.
            // Still, if targetTexture exists, enforce it to avoid "white" output.
            if (videoImage != null && videoPlayer.targetTexture != null)
                videoImage.texture = videoPlayer.targetTexture;

            videoPlayer.Prepare();
            while (videoPlayer != null && !videoPlayer.isPrepared)
                yield return null;

            videoPlayer.Play();
        }

        // Fade from black to video
        yield return FadeTo(0f, fadeInSeconds);

        // Wait for end
        while (videoPlayer != null && videoPlayer.isPlaying)
            yield return null;

        // Fade out before scene load
        yield return FadeTo(1f, fadeOutSeconds);

        // Hide video UI
        if (videoGroup != null) videoGroup.alpha = 0f;

        // Load next scene cleanly
        yield return SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);

        // Fade back in
        yield return FadeTo(0f, fadeInSeconds);

        // Resume music
        if (musicSource != null)
            musicSource.UnPause();

        // Re-enable UI clicks
        SetUIClicksEnabled(true);

        isPlaying = false;
    }

    private void SetUIClicksEnabled(bool enabled)
    {
        // Prefer assigned EventSystem, fallback to current.
        if (eventSystem == null)
            eventSystem = EventSystem.current != null ? EventSystem.current : FindFirstObjectByType<EventSystem>();

        if (eventSystem != null)
            eventSystem.enabled = enabled;
    }

    private IEnumerator FadeTo(float targetA, float seconds)
    {
        if (fadeImage == null)
            yield break;

        seconds = Mathf.Max(0.01f, seconds);
        float startA = fadeImage.color.a;

        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startA, targetA, t / seconds);
            SetFadeAlpha(a);
            yield return null;
        }

        SetFadeAlpha(targetA);
    }

    private void SetFadeAlpha(float a)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = Mathf.Clamp01(a);
        fadeImage.color = c;
    }
}