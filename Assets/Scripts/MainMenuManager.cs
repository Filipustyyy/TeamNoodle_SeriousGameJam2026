using FMOD.Studio;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Audio")] [SerializeField] private FMODUnity.EventReference menuMusicEvent;

    private EventInstance _musicInstance;

    void Start()
    {
        ShowMainMenu();

        if (AudioManager.instance == null)
        {
            Debug.LogWarning("AudioManager is missing! Menu won't have sound.");
            return;
        }

        if (!menuMusicEvent.IsNull)
        {
            _musicInstance = AudioManager.instance.CreateEventInstance(menuMusicEvent);
            _musicInstance.start();
        }
    }

    public void BackToMenu()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void PlayButton()
    {
        PlayButtonSound();
        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SceneTransition.Instance.LoadScene("BeginningCutscene");
    }

    public void CreditsButton()
    {
        PlayButtonSound();
        ShowCredits();
    }

    public void ExitGame()
    {
        PlayButtonSound();
        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SceneTransition.Instance.QuitGame();
    }

    private void PlayButtonSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.button, transform.position);
    }
}