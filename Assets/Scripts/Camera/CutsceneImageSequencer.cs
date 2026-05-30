using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneImageSequencer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image displayImage;
    [SerializeField] private Image fadeOverlay;

    [Header("Cutscene Frames")]
    [SerializeField] private Sprite[] frames;

    [Header("Timings")]
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1.5f;
    
    private bool _readyForNextImage = false;

    // Subscribe to the Camera's event
    private void OnEnable()
    {
        CutsceneCamera.OnNextImageRequested += HandleImageRequest;
    }

    private void OnDisable()
    {
        CutsceneCamera.OnNextImageRequested -= HandleImageRequest;
    }

    private void Start()
    {
        if (frames != null && frames.Length > 0)
        {
            StartCoroutine(PlayCutscene());
        }
    }
    
    private void HandleImageRequest()
    {
        _readyForNextImage = true;
    }

    private IEnumerator PlayCutscene()
    {
        SetOverlayAlpha(1f);

        for (int i = 0; i < frames.Length; i++)
        {
            displayImage.sprite = frames[i];

            // FADE IN
            yield return StartCoroutine(FadeOverlay(1f, 0f, fadeInTime));

            // WAIT FOR CAMERA
            // This pauses the Coroutine infinitely until the camera flips the flag
            _readyForNextImage = false;
            yield return new WaitUntil(() => _readyForNextImage);

            // FADE OUT
            yield return StartCoroutine(FadeOverlay(0f, 1f, fadeOutTime));
        }
        
        SceneTransition.Instance.LoadScene("MainGame");
    }

    private IEnumerator FadeOverlay(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color overlayColor = fadeOverlay.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            overlayColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadeOverlay.color = overlayColor;
            yield return null; 
        }

        overlayColor.a = endAlpha;
        fadeOverlay.color = overlayColor;
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color c = fadeOverlay.color;
        c.a = alpha;
        fadeOverlay.color = c;
    }
}