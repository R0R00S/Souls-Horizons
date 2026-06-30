using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TextMeshProUGUI timerText;
    public Image[] lifeIcons;
    public GameObject hudCanvas; // drag your HUD Canvas here in Inspector

    [Header("Screens")]
    public GameObject gameOverScreen;
    public GameObject winScreen;

    [Header("Win Screen")]
    public TextMeshProUGUI winScoreText; // drag in Inspector

    private Camera cam;

    [Header("Pause")]
    public GameObject pausePanel;
    public GameObject pauseButton; // the in-game pause button on the HUD

    private bool isPaused = false;

    [Header("Time Modifier Notification")]
    public TextMeshProUGUI timeModifierText;  // small text sitting below the timer
    public float timeModifierDisplayDuration = 1.2f;

    private Coroutine timeModifierCoroutine;

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

    public void ShowWinScreen(int score)
    {
        if (winScreen != null) winScreen.SetActive(true);

        if (winScoreText != null)
            winScoreText.text = "Score: " + score;
    }

    public void OnPausePressed()
    {
        AudioManager.Instance.PlayButtonClick();

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            if (hudCanvas != null) hudCanvas.SetActive(false);
            GameManager.Instance.isGameActive = false;
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            if (hudCanvas != null) hudCanvas.SetActive(true);
            GameManager.Instance.isGameActive = true;
        }
    }

    // Hook to Resume button on pause panel
    public void OnResumePressed()
    {
        OnPausePressed(); // toggles back off (already plays the click sound)
    }

    // Hook to Level Select button on pause panel
    public void OnPauseLevelSelectPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToLevelSelect();
    }

    // Hook to Main Menu button on pause panel
    public void OnPauseMainMenuPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToMainMenu();
    }

    // Replace the old OnRetryPressed
    public void OnRetryPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.ReloadCurrentLevel();
    }

    // New — hook to Next Level button on win screen
    public void OnNextLevelPressed()
    {
        AudioManager.Instance.PlayButtonClick();

        LevelData next = GetNextLevel();
        if (next != null)
            SceneLoader.Instance.LoadLevel(next);
        else
            SceneLoader.Instance.GoToLevelSelect(); // no next level, go back to select
    }

    // Hook to Level Select button on win/lose screens
    public void OnLevelSelectPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToLevelSelect();
    }

    // Hook to Main Menu button on lose screen
    public void OnMainMenuPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToMainMenu();
    }

    LevelData GetNextLevel()
    {
        // SceneLoader knows which level was loaded — ask it for the next one
        LevelSelectUI levelSelect = FindObjectOfType<LevelSelectUI>();

        // Since LevelSelectUI is in a different scene, store level order on SceneLoader instead
        return SceneLoader.Instance.GetNextLevel();
    }

    public void ShowTimeModifierNotification(string text)
    {
        if (timeModifierText == null) return;

        // Cancel any currently showing notification before starting a new one
        if (timeModifierCoroutine != null)
            StopCoroutine(timeModifierCoroutine);

        timeModifierCoroutine = StartCoroutine(ShowTimeModifier(text));
    }

    IEnumerator ShowTimeModifier(string text)
    {
        timeModifierText.text = text;
        timeModifierText.gameObject.SetActive(true);

        // Optional: color code — red for punishment, green for reward
        timeModifierText.color = text.StartsWith("+") ? Color.red : Color.green;

        yield return new WaitForSecondsRealtime(timeModifierDisplayDuration);

        timeModifierText.gameObject.SetActive(false);
        timeModifierCoroutine = null;
    }
}