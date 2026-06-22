using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    // Scene names — match these exactly to your Build Settings scene names
    [Header("Scene Names")]
    public string startSceneName = "StartScene";
    public string gameSceneName = "GameScene";

    // All LevelData assets in order — slot 0 = Level 1, slot 1 = Level 2 etc.
    [Header("Levels")]
    public LevelData[] levels;

    void Awake()
    {
        // Persist across scene loads so level data carries into the game scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Level index out of range: " + levelIndex);
            return;
        }

        SelectedLevel = levels[levelIndex];
        Time.timeScale = 1; // always unpause before loading
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadStartScreen()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(startSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit called"); // visible in Editor since Application.Quit doesn't work there
        Application.Quit();
    }

    // Carries the selected level into the game scene
    public LevelData SelectedLevel { get; private set; }
}