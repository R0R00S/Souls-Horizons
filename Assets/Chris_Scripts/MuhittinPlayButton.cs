using UnityEngine;
using UnityEngine.SceneManagement;

public class MuhittinPlayButton : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}