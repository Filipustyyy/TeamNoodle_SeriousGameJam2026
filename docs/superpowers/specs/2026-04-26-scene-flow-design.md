# Gameplay Scene Flow Design

**Date:** 2026-04-26
**Status:** Approved (pending spec review)

## Goal

Wire the full game flow: **MainMenu → MainGame → EndScreen → Quit**. Player enters EndScreen by walking into an invisible trigger area in MainGame. All scene transitions use a fade-to-black effect.

## Scope

- New `EndScreen.unity` scene (Quit button only).
- Persistent fade-transition system shared across all scene loads.
- Invisible end-zone trigger placed in MainGame.
- MainMenu Play/Quit routed through the same transition system.

Out of scope: pause menu, settings, in-game audio handoff between scenes, restart/back-to-menu from end screen, multiple end zones.

## Scene Flow

```
MainMenu.unity ──Play──▶ Levels/MainGame.unity ──EndZone trigger──▶ EndScreen.unity ──Quit──▶ Application.Quit
                                                                          │
MainMenu Quit ──────────────────────────────────────────────────────────▶ ┘
```

Build settings:

| Index | Path |
|-------|------|
| 0 | `Assets/Scenes/MainMenu.unity` (existing) |
| 1 | `Assets/Scenes/Levels/MainGame.unity` (existing) |
| 2 | `Assets/Scenes/EndScreen.unity` (new) |

Scene loads reference scenes by **string name**, not buildIndex. Replaces current `SceneManager.LoadScene(activeScene.buildIndex + 1)` pattern in `MainMenuManager`.

## Components

### New: `Assets/Scripts/Core/SceneTransition.cs`

Persistent singleton MonoBehaviour responsible for all scene loads and quit calls.

- Bootstrapped via `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]` — spawns once before first scene loads.
- Builds its own `Canvas` (RenderMode = ScreenSpaceOverlay, sortOrder = 32767) + full-screen black `Image` + `CanvasGroup`. No prefab; constructed in code.
- `DontDestroyOnLoad` on the root.
- Singleton: `public static SceneTransition Instance`. Awake guard destroys duplicates.
- Subscribes to `SceneManager.sceneLoaded` in `OnEnable`, unsubscribes in `OnDisable`.

**Public API:**

```csharp
public void LoadScene(string sceneName);
public void QuitGame();
```

**Behavior:**

- `LoadScene(name)`: ignored if a transition is already in progress (`_isTransitioning` guard). Otherwise: fade alpha 0→1 over `fadeOutDuration`, then `SceneManager.LoadScene(name)`. Fade-in happens automatically on the `sceneLoaded` callback.
- `QuitGame()`: fade out, then `Application.Quit()`. In editor: also set `UnityEditor.EditorApplication.isPlaying = false` (wrapped in `#if UNITY_EDITOR`).
- Initial alpha = 1 (black) so the very first scene fades in from black on launch.
- `CanvasGroup.blocksRaycasts = true` while alpha > 0 — blocks button clicks during the fade.

**Serialized fields (with defaults):**

- `fadeOutDuration` = 0.4f
- `fadeInDuration` = 0.4f

(Both initialized in code since this object is instantiated, not placed in a scene. Serialized markers kept so they're discoverable if the system grows into a prefab later.)

### New: `Assets/Scripts/Core/EndZoneTrigger.cs`

Component placed on a GameObject in `MainGame.unity` with a `BoxCollider2D` configured as `isTrigger = true`. The collider is the invisible end zone — no `SpriteRenderer`, just the collider.

```csharp
private bool _triggered;

void OnTriggerEnter2D(Collider2D other)
{
    if (_triggered) return;
    if (!other.CompareTag("Player")) return;
    _triggered = true;
    SceneTransition.Instance.LoadScene("EndScreen");
}
```

Guard prevents double-fire if the player exits and re-enters during the fade.

### New: `Assets/Scripts/UI/EndScreenManager.cs`

Lives in `EndScreen.unity` on a manager GameObject. References the Quit button (or wired via UnityEvent in inspector).

```csharp
public void QuitGame()
{
    AudioManager.instance.PlayOneShot(FMODEvents.instance.button, transform.position);
    SceneTransition.Instance.QuitGame();
}
```

### New scene: `Assets/Scenes/EndScreen.unity`

- `Canvas` (Screen Space - Overlay)
  - `TextMeshProUGUI` — "The End" or similar copy (placeholder text; final copy is a content decision, not a code one)
  - `Button` — "Quit", wired to `EndScreenManager.QuitGame`
- `EventSystem`
- `EndScreenManager` GameObject

No camera needed (Overlay canvas). No background music required for the jam scope.

### Modified: `Assets/Scripts/MainMenuManager.cs`

`PlayButton()` and `ExitGame()` updated to route through `SceneTransition`:

```csharp
public void PlayButton()
{
    PlayButtonSound();
    _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    SceneTransition.Instance.LoadScene("MainGame");
}

public void ExitGame()
{
    PlayButtonSound();
    _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    SceneTransition.Instance.QuitGame();
}
```

The existing `_musicInstance.stop(ALLOWFADEOUT)` calls are preserved — FMOD music fade overlaps with the visual fade, which is fine.

## Data Flow

**Transition sequence (`LoadScene`):**

1. Caller invokes `SceneTransition.Instance.LoadScene("X")`.
2. If `_isTransitioning` already true → return (no-op).
3. Set `_isTransitioning = true`.
4. Coroutine lerps `CanvasGroup.alpha` 0→1 over `fadeOutDuration` (unscaled time).
5. `SceneManager.LoadScene("X")` (synchronous load — simpler than async; scenes are small).
6. `sceneLoaded` callback fires → start fade-in coroutine.
7. Coroutine lerps alpha 1→0 over `fadeInDuration`.
8. Set `_isTransitioning = false`.

**Quit sequence (`QuitGame`):**

1. Caller invokes `SceneTransition.Instance.QuitGame()`.
2. Coroutine lerps alpha 0→1.
3. `Application.Quit()` (and `EditorApplication.isPlaying = false` in editor).

## Edge Cases

| Case | Handling |
|------|----------|
| Player re-enters end zone before scene unloads | `_triggered` bool on `EndZoneTrigger` |
| Multiple `LoadScene` calls during transition | `_isTransitioning` guard ignores subsequent calls |
| Buttons clicked during fade | `CanvasGroup.blocksRaycasts = true` blocks input |
| Player keeps moving during fade-out | Allowed — fade is short (0.4s), `Time.timeScale` not modified |
| First scene launch | CanvasGroup starts at alpha 1, fades in on first `sceneLoaded` |
| Player object missing `Player` tag | Trigger silently no-ops; verify tag during integration testing |

## Prerequisites

- Player root GameObject has tag `Player`.
- TextMeshPro essentials imported (already present, used by Dialog system).
- New scenes added to Build Settings before testing builds.

## Testing

Manual test plan (no unit tests — MonoBehaviour-heavy jam project):

1. **Launch**: open MainMenu in editor → screen fades in from black.
2. **Play**: click Play → fade-out → MainGame loads → fade-in.
3. **End zone**: walk player into trigger → fade-out → EndScreen loads → fade-in.
4. **End screen quit**: click Quit → fade-out → editor exits play mode (build: app closes).
5. **Menu quit**: from MainMenu, click Quit → fade-out → exit.
6. **Spam-click guard**: rapid-click Play repeatedly during fade — only one transition fires.
7. **Visual check**: full-screen black at multiple aspect ratios, no flicker/flash between scenes, buttons unclickable while fading.

## File Summary

**New:**
- `Assets/Scripts/Core/SceneTransition.cs`
- `Assets/Scripts/Core/EndZoneTrigger.cs`
- `Assets/Scripts/UI/EndScreenManager.cs`
- `Assets/Scenes/EndScreen.unity`

**Modified:**
- `Assets/Scripts/MainMenuManager.cs` (Play + Quit routing)
- `ProjectSettings/EditorBuildSettings.asset` (add EndScreen at index 2)

**Scene editing required:**
- `Levels/MainGame.unity` — add EndZone GameObject with `BoxCollider2D` (trigger) + `EndZoneTrigger` script.
