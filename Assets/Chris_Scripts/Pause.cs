using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    public Button pauseButton;
    private bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(TogglePause);
    }

    void Update()
    {
       
  
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
}
