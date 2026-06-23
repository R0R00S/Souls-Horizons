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

        // Use whatever level was selected on the Level Select screen
        if (SceneLoader.Instance != null && SceneLoader.Instance.selectedLevel != null)
            currentLevel = SceneLoader.Instance.selectedLevel;

        // Fallback — if currentLevel is still null (e.g. running Game scene directly
        // in the Editor without going through Level Select), use whatever is
        // assigned in the Inspector as a default
        if (currentLevel == null)
            Debug.LogError("No LevelData assigned. Run from LevelSelect or assign a default.");

        timeRemaining = currentLevel.levelDuration;
    }

    void Update()
    {
        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;
        timeElapsed += Time.deltaTime;

        // Only update UI once per second to avoid string allocation every frame
        int currentSecond = Mathf.CeilToInt(timeRemaining);
        if (currentSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = currentSecond;
            UIManager.Instance.UpdateTimer(currentSecond.ToString());
        }

        if (timeRemaining <= 0f)
            LevelWin();
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
}