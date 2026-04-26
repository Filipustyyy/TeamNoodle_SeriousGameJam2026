using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        Application.Quit();
    }

    private void PlayButtonSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.button, transform.position);
    }
}