using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD Elements")]
    public TextMeshProUGUI timerText;
    public Image[] lifeIcons; // drag your 3 heart images here in the Inspector

    [Header("Screens")]
    public GameObject gameOverScreen;
    public GameObject winScreen;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Make sure screens are hidden at the start
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (winScreen != null) winScreen.SetActive(false);
    }

    // Called by GameManager every Update frame
    public void UpdateTimer(string time)
    {
        if (timerText != null)
            timerText.text = time;
    }

    // Called by GameManager whenever lives change
    public void UpdateLives(int currentLives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            // Icons at index >= currentLives get hidden
            lifeIcons[i].enabled = (i < currentLives);
        }
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }

    public void ShowWinScreen()
    {
        if (winScreen != null) winScreen.SetActive(true);
    }

    // Called by the Retry button's OnClick event in the Inspector
    public void OnRetryPressed()
    {
        Time.timeScale = 1; // unpause before reloading
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}