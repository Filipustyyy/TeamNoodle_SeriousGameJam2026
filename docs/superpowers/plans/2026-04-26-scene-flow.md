# Gameplay Scene Flow Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Wire MainMenu → MainGame → EndScreen → Quit with a persistent fade-to-black transition system.

**Architecture:** A `SceneTransition` singleton (DontDestroyOnLoad, bootstrapped via `RuntimeInitializeOnLoadMethod`) owns a full-screen black `CanvasGroup` and exposes `LoadScene(name)` / `QuitGame()`. `MainMenuManager` is updated to use it. A new invisible `EndZoneTrigger` in MainGame routes to a new `EndScreen` scene whose `EndScreenManager` handles Quit.

**Tech Stack:** Unity (2D), C#, FMOD (audio passthrough only), TextMeshPro.

**Spec:** `docs/superpowers/specs/2026-04-26-scene-flow-design.md`

**Testing reality:** This is a Unity jam project with no PlayMode/EditMode test setup. The spec explicitly opts out of unit tests. Each task ends with manual editor verification + commit.

---

## File Structure

**New code:**
- `Assets/Scripts/Core/SceneTransition.cs` — persistent fade + scene-load singleton
- `Assets/Scripts/Core/EndZoneTrigger.cs` — invisible 2D trigger that loads EndScreen
- `Assets/Scripts/UI/EndScreenManager.cs` — wires EndScreen Quit button

**New scene:**
- `Assets/Scenes/EndScreen.unity`

**Modified:**
- `Assets/Scripts/MainMenuManager.cs` — Play/Quit route through `SceneTransition`
- `ProjectSettings/EditorBuildSettings.asset` — add EndScreen at index 2
- `Assets/Scenes/Levels/MainGame.unity` — add EndZone GameObject

---

### Task 1: SceneTransition singleton

**Files:**
- Create: `Assets/Scripts/Core/SceneTransition.cs`

- [ ] **Step 1: Create folder + script**

If `Assets/Scripts/Core/` does not exist, create it. Add this file:

```csharp
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
```

- [ ] **Step 2: Compile in Unity**

Switch to Unity, wait for asset DB refresh + compile. Expected: no errors in Console.

- [ ] **Step 3: Manual smoke test**

Open `Assets/Scenes/MainMenu.unity`, press Play. Expected:
- Screen starts black, fades to MainMenu over ~0.4s
- No errors/warnings about missing Canvas/EventSystem
- Console shows GameObject `[SceneTransition]` in DontDestroyOnLoad scene (Hierarchy view)

Stop play.

- [ ] **Step 4: Commit**

```bash
git add Assets/Scripts/Core/SceneTransition.cs Assets/Scripts/Core.meta Assets/Scripts/Core/SceneTransition.cs.meta
git commit -m "feat: add SceneTransition fade singleton"
```

(Unity generates `.meta` files automatically — include whichever ones appear under `git status`.)

---

### Task 2: Route MainMenu Play/Quit through SceneTransition

**Files:**
- Modify: `Assets/Scripts/MainMenuManager.cs`

- [ ] **Step 1: Replace PlayButton + ExitGame**

In `MainMenuManager.cs`, replace the existing `PlayButton()` method:

```csharp
public void PlayButton()
{
    PlayButtonSound();
    _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    SceneTransition.Instance.LoadScene("MainGame");
}
```

And replace `ExitGame()`:

```csharp
public void ExitGame()
{
    PlayButtonSound();
    _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    SceneTransition.Instance.QuitGame();
}
```

Remove the `using UnityEngine.SceneManagement;` line if it is no longer referenced anywhere else in the file.

- [ ] **Step 2: Compile**

Switch to Unity. Expected: no Console errors.

- [ ] **Step 3: Manual test — Play**

Open `Assets/Scenes/MainMenu.unity`, press Play. Click Play button. Expected:
- Menu music starts fading out
- Screen fades to black over ~0.4s
- `MainGame` scene loads
- Screen fades from black to gameplay view
- No NullReferenceException in Console

Stop play.

- [ ] **Step 4: Manual test — Quit from menu**

Open `MainMenu.unity`, press Play. Click Quit button. Expected:
- Screen fades to black
- Editor exits play mode (fade-out completes first)

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/MainMenuManager.cs
git commit -m "feat: route MainMenu Play/Quit through SceneTransition"
```

---

### Task 3: EndZoneTrigger script

**Files:**
- Create: `Assets/Scripts/Core/EndZoneTrigger.cs`

- [ ] **Step 1: Create script**

```csharp
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EndZoneTrigger : MonoBehaviour
{
    [SerializeField] private string targetScene = "EndScreen";

    private bool _triggered;

    private void Reset()
    {
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;
        _triggered = true;
        SceneTransition.Instance.LoadScene(targetScene);
    }
}
```

- [ ] **Step 2: Compile in Unity**

Expected: no Console errors.

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Core/EndZoneTrigger.cs
git commit -m "feat: add EndZoneTrigger component"
```

(End-zone scene wiring happens in Task 6, after EndScreen exists and is in build settings.)

---

### Task 4: EndScreenManager script

**Files:**
- Create: `Assets/Scripts/UI/EndScreenManager.cs`

- [ ] **Step 1: Create folder + script**

If `Assets/Scripts/UI/` does not exist, create it.

```csharp
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    public void QuitGame()
    {
        if (AudioManager.instance != null && FMODEvents.instance != null)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.button, transform.position);
        }
        SceneTransition.Instance.QuitGame();
    }
}
```

The null guards on AudioManager/FMODEvents protect against running EndScreen standalone in the editor without going through MainMenu first (singletons may not have spawned).

- [ ] **Step 2: Compile**

Expected: no Console errors.

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/UI/EndScreenManager.cs
git commit -m "feat: add EndScreenManager"
```

---

### Task 5: Create EndScreen scene

**Files:**
- Create: `Assets/Scenes/EndScreen.unity`
- Modify: `ProjectSettings/EditorBuildSettings.asset`

This task is done in the Unity Editor. No script changes.

- [ ] **Step 1: Create new scene**

In Unity: `File > New Scene` → choose **Basic (Built-in)** template (gives Camera + Light). Save as `Assets/Scenes/EndScreen.unity`.

- [ ] **Step 2: Strip the default lighting**

Delete the default `Directional Light` (2D project, not needed). Keep `Main Camera` (its presence is harmless and prevents "no cameras rendering" warnings if Canvas is set to Camera-space later).

- [ ] **Step 3: Add Canvas + EventSystem**

`GameObject > UI > Canvas`. This auto-creates an EventSystem. Set Canvas Render Mode = `Screen Space - Overlay` (default). Set CanvasScaler `UI Scale Mode = Scale With Screen Size`, Reference Resolution `1920 x 1080`.

- [ ] **Step 4: Add "The End" text**

Inside Canvas: `GameObject > UI > Text - TextMeshPro` (accept TMP essentials import if prompted). Set:
- Text: `The End`
- Font Size: 120
- Alignment: Center / Middle
- Anchor: middle-center, position (0, 100)
- Width: 800, Height: 200

- [ ] **Step 5: Add Quit button**

Inside Canvas: `GameObject > UI > Button - TextMeshPro`. Set:
- Anchor: middle-center, position (0, -150)
- Width: 300, Height: 80
- Child Text: `Quit`

- [ ] **Step 6: Add EndScreenManager GameObject**

Right-click in Hierarchy → `Create Empty`. Rename to `EndScreenManager`. Add component `EndScreenManager` (the script from Task 4).

- [ ] **Step 7: Wire button → QuitGame**

Select the Quit Button. In the Inspector, under `Button > On Click ()`:
- Click `+`
- Drag the `EndScreenManager` GameObject into the object slot
- Set the function to `EndScreenManager > QuitGame()`

- [ ] **Step 8: Save scene + add to Build Settings**

`Ctrl/Cmd+S` to save. Then `File > Build Settings...` → `Add Open Scenes`. Drag `EndScreen` to position **2** (after MainGame). Confirm order:

| Index | Scene |
|-------|-------|
| 0 | Assets/Scenes/MainMenu.unity |
| 1 | Assets/Scenes/Levels/MainGame.unity |
| 2 | Assets/Scenes/EndScreen.unity |

Close Build Settings.

- [ ] **Step 9: Manual test — Quit button**

Open `EndScreen.unity`. Press Play. Expected:
- Screen fades from black to "The End" + Quit button (SceneTransition bootstraps)
- Click Quit
- Screen fades to black, editor exits play mode

If Quit fires instantly with no fade, check that the SceneTransition bootstrap fired (look in DontDestroyOnLoad in Hierarchy during play). If not, the script may have a compile error — check Console.

- [ ] **Step 10: Commit**

```bash
git add Assets/Scenes/EndScreen.unity Assets/Scenes/EndScreen.unity.meta ProjectSettings/EditorBuildSettings.asset
git commit -m "feat: add EndScreen scene"
```

---

### Task 6: Add EndZone trigger to MainGame

**Files:**
- Modify: `Assets/Scenes/Levels/MainGame.unity`

Editor task. No script changes.

- [ ] **Step 1: Open MainGame**

Open `Assets/Scenes/Levels/MainGame.unity`.

- [ ] **Step 2: Verify Player tag**

Locate the Player GameObject (or the prefab spawned by `PlayerSpawner` if used). In the Inspector, confirm Tag = `Player`. If it is `Untagged`, change it to `Player` (the tag should already exist; if not, create it).

- [ ] **Step 3: Create EndZone GameObject**

In Hierarchy: `Create Empty`. Rename to `EndZone`. Position it at the end of the level (somewhere the player can walk into — exact placement is a level-design call; pick a spot past the final puzzle/checkpoint).

- [ ] **Step 4: Add components**

With EndZone selected:
- `Add Component > Box Collider 2D`. Set `Is Trigger = true`. Resize `Size` so it spans a full screen height and ~2 units wide (adjust to taste).
- `Add Component > EndZoneTrigger`. The `Reset` method already set the BoxCollider2D's `isTrigger`, but verify it's still on.
- Leave `Target Scene` field as `EndScreen` (default).

No SpriteRenderer — the trigger is invisible.

- [ ] **Step 5: Save scene**

`Ctrl/Cmd+S`.

- [ ] **Step 6: End-to-end manual test**

Open `MainMenu.unity`. Press Play. Expected full flow:

1. Fade in → MainMenu visible.
2. Click Play → fade out → MainGame loads → fade in.
3. Walk Player into EndZone → fade out → EndScreen loads → fade in (you see "The End" + Quit).
4. Click Quit → fade out → editor exits play mode.

If the trigger doesn't fire: check Player has Rigidbody2D (it does per `PlayerMovement.cs`), Player tag is `Player`, EndZone collider is large enough to overlap.

- [ ] **Step 7: Commit**

```bash
git add Assets/Scenes/Levels/MainGame.unity
git commit -m "feat: add EndZone trigger to MainGame"
```

---

## Self-Review

**Spec coverage check:**
- ✅ Scene flow (MainMenu → MainGame → EndScreen → Quit) — Tasks 2, 3, 5, 6
- ✅ Build settings order — Task 5 step 8
- ✅ String-name scene loads — Task 1 (`LoadScene(string)`), Task 2 (`"MainGame"`), Task 3 (`"EndScreen"`)
- ✅ SceneTransition singleton + `RuntimeInitializeOnLoadMethod` — Task 1
- ✅ DontDestroyOnLoad — Task 1
- ✅ Black full-screen Image with stretched anchors — Task 1 `BuildOverlay()`
- ✅ Initial alpha = 1 → fade in on first scene — Task 1 (`_group.alpha = 1f` + `HandleSceneLoaded` fades in)
- ✅ `_isTransitioning` guard — Task 1
- ✅ `blocksRaycasts` while alpha > 0 — Task 1 `Fade()` end logic
- ✅ Editor quit handling (`#if UNITY_EDITOR`) — Task 1 `QuitRoutine`
- ✅ MainMenuManager refactor — Task 2
- ✅ EndZoneTrigger with `_triggered` guard + Player tag — Task 3
- ✅ Invisible 2D BoxCollider trigger — Task 6
- ✅ EndScreen scene with Quit only — Task 5
- ✅ EndScreenManager — Task 4
- ✅ FMOD button SFX preserved — Tasks 2, 4
- ✅ Music ALLOWFADEOUT preserved — Task 2

**Placeholder scan:** No TBD/TODO. Every code step has full code. Every editor step has exact menu paths.

**Type consistency:** `SceneTransition.Instance.LoadScene(string)` and `SceneTransition.Instance.QuitGame()` used consistently across Tasks 2, 3, 4. `_triggered` and `_isTransitioning` are local guard fields, no cross-task references. `targetScene` field on EndZoneTrigger defaults to `"EndScreen"`, matches scene file name in Task 5.

No gaps found.
