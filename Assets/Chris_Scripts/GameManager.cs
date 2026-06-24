using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelData currentLevel;

    public int lives = 3;
    public float timeRemaining;
    public float timeElapsed = 0f;

    public bool isGameActive = true;

    private int lastDisplayedSecond = -1;

    void Awake()
    {
        Instance = this;


        if (SceneLoader.Instance != null && SceneLoader.Instance.selectedLevel != null)
        {
            currentLevel = SceneLoader.Instance.selectedLevel;
        }

        // currentLevel might already be assigned in the Inspector as a fallback
        // for when you start directly from this scene in the Editor
        if (currentLevel == null)
        {
            Debug.LogWarning("No LevelData from SceneLoader — using Inspector fallback.");
        }

        if (currentLevel == null)
        {
            Debug.LogError("No LevelData assigned anywhere. Assign one in the Inspector.");
            return;
        }

        timeRemaining = currentLevel.levelDuration;
    }

    void Start()
    {
        // Don't start gameplay yet — wait for opening dialogue to finish
        isGameActive = false;

        LevelDialogueData dialogueData = currentLevel.dialogueData;

        DialogueSequence opening = dialogueData != null ? dialogueData.openingSequence : null;

        DialogueManager.Instance.PlayOpeningSequence(opening, OnOpeningDialogueComplete);
    }

    void Update()
    {
        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;
        timeElapsed += Time.deltaTime;

        // Check mid-level notifications
        CheckNotifications();

        int currentSecond = Mathf.CeilToInt(timeRemaining);
        if (currentSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = currentSecond;
            UIManager.Instance.UpdateTimer(currentSecond.ToString());
        }

        if (timeRemaining <= 0f) LevelWin();
    }

    public void WrongPit()
    {
        lives--;
        UIManager.Instance.UpdateLives(lives);
        Debug.Log("Wrong pit! Lives remaining: " + lives);

        if (lives <= 0)
            GameOver();
    }

    public void CorrectPit(GameObject box)
    {
        Debug.Log("Correct!");
        BoxSpawner.Instance.BoxSorted();
        BoxPool.Instance.ReturnBox(box);
    }
    void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0;
        BoxSpawner.Instance.StopSpawning();
        FlameSpawner.Instance.StopSpawning();

        // Stop all pit spawners
        foreach (PitSpawner ps in FindObjectsOfType<PitSpawner>())
            ps.StopSpawning();

        UIManager.Instance.ShowGameOverScreen();
    }

    void LevelWin()
    {
        isGameActive = false;
        Time.timeScale = 0;
        BoxSpawner.Instance.StopSpawning();
        FlameSpawner.Instance.StopSpawning();

        foreach (PitSpawner ps in FindObjectsOfType<PitSpawner>())
            ps.StopSpawning();

        UIManager.Instance.ShowWinScreen();
    }

    public void Retry()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    void OnOpeningDialogueComplete()
    {
        // Dialogue finished — now actually start the level
        Time.timeScale = 1;
        isGameActive = true;

        // Optional: trigger your door opening animation here before setting isGameActive
        // For now just starts immediately
        Debug.Log("Level started");
    }

    void CheckNotifications()
    {
        if (currentLevel.dialogueData == null) return;

        foreach (LevelNotification notification in currentLevel.dialogueData.notifications)
        {
            if (!notification.hasTriggered && timeElapsed >= notification.triggerAtTime)
            {
                notification.hasTriggered = true;
                NotificationManager.Instance.ShowNotification(
                    notification.message,
                    notification.displayDuration
                );
            }
        }
    }

}