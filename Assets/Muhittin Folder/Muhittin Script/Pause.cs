using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    public Button pauseButton;
    public Sprite pauseSprite;
    public Sprite resumeSprite;

    private bool isPaused = false;
    private Image pauseButtonImage;

    void Start()
    {
        pauseButtonImage = pauseButton.GetComponent<Image>();
        pauseButton.onClick.AddListener(TogglePause);
    }

    void Update()
    {
       
  
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseButtonImage.sprite = isPaused ? resumeSprite : pauseSprite;
    }
}
