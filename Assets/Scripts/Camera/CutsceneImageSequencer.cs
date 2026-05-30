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
    [Tooltip("Drag your converted PNG Sprites here in the order they should play.")]
    [SerializeField] private Sprite[] frames;

    [Header("Timings")]
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float fadeOutTime = 1f;

    [Header("Level Transition")]
    [Tooltip("The Build Index of the gameplay scene to load when the cutscene finishes.")]
    [SerializeField] private int nextSceneIndex = 1;

    private void Start()
    {
        if (frames != null && frames.Length > 0)
        {
            StartCoroutine(PlayCutscene());
        }
        else
        {
            Debug.LogWarning("No frames assigned to the Cutscene Sequencer!");
        }
    }

    private IEnumerator PlayCutscene()
    {
        SetOverlayAlpha(1f);
        
        for (int i = 0; i < frames.Length; i++)
        {
            displayImage.sprite = frames[i];
            
            yield return StartCoroutine(FadeOverlay(1f, 0f, fadeInTime));
            
            yield return new WaitForSeconds(displayTime);
            
            yield return StartCoroutine(FadeOverlay(0f, 1f, fadeOutTime));
        }
        
        SceneTransition.Instance.LoadScene("MainGame");
    }

    // --- FADE LOGIC ---
    private IEnumerator FadeOverlay(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color overlayColor = fadeOverlay.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            // Smoothly calculate the new transparency
            overlayColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadeOverlay.color = overlayColor;
            
            yield return null; // Wait for the next frame
        }

        // Guarantee we hit the exact target alpha at the end
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