using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public const string MAIN_MENU = "StartScreen";
    public const string LEVEL_SELECT = "LevelSelect";

    [Header("Level Order — same order as Level Select buttons")]
    public LevelData[] levelOrder;

    void Awake()
    {
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

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(MAIN_MENU);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(LEVEL_SELECT);
    }

    public void LoadLevel(LevelData levelData)
    {
        selectedLevel = levelData;
        Time.timeScale = 1;
        SceneManager.LoadScene(levelData.sceneName);
    }

    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1;

        if (selectedLevel != null)
            SceneManager.LoadScene(selectedLevel.sceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // fallback
    }

    public void QuitGame()
    {
        Application.Quit();
        
    }

    public LevelData GetNextLevel()
    {
        if (selectedLevel == null || levelOrder == null) return null;

        for (int i = 0; i < levelOrder.Length - 1; i++)
        {
            if (levelOrder[i] == selectedLevel)
                return levelOrder[i + 1];
        }

        return null;
    }

    [HideInInspector] public LevelData selectedLevel;
}