using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TextMeshProUGUI timerText;
    public Image[] lifeIcons;

    [Header("Screens")]
    public GameObject gameOverScreen;
    public GameObject winScreen;

    private Camera cam;

    [Header("Pause")]
    public GameObject pausePanel;
    public GameObject pauseButton; // the in-game pause button on the HUD

    private bool isPaused = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (winScreen != null) winScreen.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void UpdateTimer(string time)
    {
        if (timerText == null) return;
        timerText.text = time;

        // Flash red in last 10 seconds
        timerText.color = int.Parse(time) <= 10 ? Color.red : Color.white;
    }

    public void UpdateLives(int currentLives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].enabled = (i < currentLives);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }

    public void ShowWinScreen()
    {
        if (winScreen != null) winScreen.SetActive(true);
    }

    public void OnPausePressed()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            GameManager.Instance.isGameActive = false;
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            GameManager.Instance.isGameActive = true;
        }
    }

    // Hook to Resume button on pause panel
    public void OnResumePressed()
    {
        OnPausePressed(); // toggles back off
    }

    // Hook to Level Select button on pause panel
    public void OnPauseLevelSelectPressed()
    {
        SceneLoader.Instance.GoToLevelSelect();
    }

    // Hook to Main Menu button on pause panel
    public void OnPauseMainMenuPressed()
    {
        SceneLoader.Instance.GoToMainMenu();
    }

    // Replace the old OnRetryPressed
    public void OnRetryPressed()
    {
        SceneLoader.Instance.ReloadCurrentLevel();
    }

    // New — hook to Next Level button on win screen
    public void OnNextLevelPressed()
    {
        LevelData next = GetNextLevel();
        if (next != null)
            SceneLoader.Instance.LoadLevel(next);
        else
            SceneLoader.Instance.GoToLevelSelect(); // no next level, go back to select
    }

    // Hook to Level Select button on win/lose screens
    public void OnLevelSelectPressed()
    {
        SceneLoader.Instance.GoToLevelSelect();
    }

    // Hook to Main Menu button on lose screen
    public void OnMainMenuPressed()
    {
        SceneLoader.Instance.GoToMainMenu();
    }

    LevelData GetNextLevel()
    {
        // SceneLoader knows which level was loaded — ask it for the next one
        LevelSelectUI levelSelect = FindObjectOfType<LevelSelectUI>();

        // Since LevelSelectUI is in a different scene, store level order on SceneLoader instead
        return SceneLoader.Instance.GetNextLevel();
    }
}