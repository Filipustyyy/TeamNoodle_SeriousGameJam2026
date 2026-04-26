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
