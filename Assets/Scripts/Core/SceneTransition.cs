using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private float fadeOutDuration = 0.4f;
    [SerializeField] private float fadeInDuration = 0.4f;

    private CanvasGroup _group;
    private bool _isTransitioning;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("[SceneTransition]");
        DontDestroyOnLoad(go);
        go.AddComponent<SceneTransition>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BuildOverlay();
    }

    private void BuildOverlay()
    {
        var canvasGo = new GameObject("FadeCanvas");
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;

        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        var imageGo = new GameObject("Black");
        imageGo.transform.SetParent(canvasGo.transform, false);

        var image = imageGo.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = true;

        var rt = image.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        _group = imageGo.AddComponent<CanvasGroup>();
        _group.alpha = 1f;
        _group.blocksRaycasts = true;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        if (_isTransitioning) return;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    public void QuitGame()
    {
        if (_isTransitioning) return;
        StartCoroutine(QuitRoutine());
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        _isTransitioning = true;
        yield return Fade(0f, 1f, fadeOutDuration);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        yield return Fade(_group.alpha, 0f, fadeInDuration);
        _isTransitioning = false;
    }

    private IEnumerator QuitRoutine()
    {
        _isTransitioning = true;
        yield return Fade(0f, 1f, fadeOutDuration);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        _group.blocksRaycasts = true;
        if (duration <= 0f)
        {
            _group.alpha = to;
            _group.blocksRaycasts = to > 0f;
            yield break;
        }
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            _group.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        _group.alpha = to;
        _group.blocksRaycasts = to > 0f;
    }
}
