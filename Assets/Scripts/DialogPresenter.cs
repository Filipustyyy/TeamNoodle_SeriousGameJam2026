using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogPresenter : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference advanceAction;

    [Header("Style")]
    [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.85f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private int speakerFontSize = 28;
    [SerializeField] private int lineFontSize = 22;
    [SerializeField] private int hintFontSize = 14;

    private CanvasGroup group;
    private Text speakerText;
    private Text lineText;
    private Text hintText;

    private readonly Queue<DialogRequest> queue = new();
    private DialogRequest current;
    private int lineIndex;
    private bool open;

    private PlayerInteractor playerInteractor;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        BuildUi();
        SetVisible(false);
    }

    private void OnEnable()
    {
        DialogBus.OnRequest += OnRequest;
        if (advanceAction != null)
        {
            advanceAction.action.performed += OnAdvance;
            advanceAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        DialogBus.OnRequest -= OnRequest;
        if (advanceAction != null)
            advanceAction.action.performed -= OnAdvance;
    }

    private void OnRequest(DialogRequest req)
    {
        queue.Enqueue(req);
        if (!open) StartNext();
    }

    private void OnAdvance(InputAction.CallbackContext _)
    {
        if (!open) return;
        lineIndex++;
        if (current.Lines == null || lineIndex >= current.Lines.Length)
            StartNext();
        else
            Render();
    }

    private void StartNext()
    {
        if (queue.Count == 0)
        {
            Close();
            return;
        }
        current = queue.Dequeue();
        lineIndex = 0;
        open = true;
        SetVisible(true);
        LockPlayerInput(true);
        Render();
    }

    private void Close()
    {
        open = false;
        SetVisible(false);
        LockPlayerInput(false);
    }

    private void Render()
    {
        speakerText.text = string.IsNullOrEmpty(current.Speaker) ? "" : current.Speaker;
        lineText.text = current.Lines[lineIndex];
        bool last = lineIndex == current.Lines.Length - 1 && queue.Count == 0;
        hintText.text = last ? "Press E to close" : "Press E to continue";
    }

    private void SetVisible(bool v)
    {
        group.alpha = v ? 1f : 0f;
        group.blocksRaycasts = v;
        group.interactable = v;
    }

    private void LockPlayerInput(bool locked)
    {
        if (playerInteractor == null)
            playerInteractor = FindAnyObjectByType<PlayerInteractor>();
        if (playerMovement == null)
            playerMovement = FindAnyObjectByType<PlayerMovement>();

        if (playerInteractor != null)
            playerInteractor.enabled = !locked;
        if (playerMovement != null)
        {
            playerMovement.enabled = !locked;
            if (locked)
            {
                var rb = playerMovement.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
    }

    private void BuildUi()
    {
        var canvasGo = new GameObject("DialogCanvas");
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();
        group = canvasGo.AddComponent<CanvasGroup>();

        var panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasGo.transform, false);
        var panelRt = (RectTransform)panel.transform;
        panelRt.anchorMin = new Vector2(0.1f, 0.05f);
        panelRt.anchorMax = new Vector2(0.9f, 0.3f);
        panelRt.offsetMin = Vector2.zero;
        panelRt.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = panelColor;

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        speakerText = MakeText("Speaker", panel.transform, font, speakerFontSize, FontStyle.Bold,
            new Vector2(0.03f, 0.7f), new Vector2(0.97f, 0.95f), TextAnchor.UpperLeft);
        lineText = MakeText("Line", panel.transform, font, lineFontSize, FontStyle.Normal,
            new Vector2(0.03f, 0.2f), new Vector2(0.97f, 0.7f), TextAnchor.UpperLeft);
        hintText = MakeText("Hint", panel.transform, font, hintFontSize, FontStyle.Italic,
            new Vector2(0.5f, 0.03f), new Vector2(0.97f, 0.2f), TextAnchor.LowerRight);
    }

    private Text MakeText(string name, Transform parent, Font font, int size, FontStyle style,
        Vector2 amin, Vector2 amax, TextAnchor align)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = amin;
        rt.anchorMax = amax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var t = go.AddComponent<Text>();
        t.font = font;
        t.fontSize = size;
        t.fontStyle = style;
        t.color = textColor;
        t.alignment = align;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.raycastTarget = false;
        return t;
    }
}