using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    // Scene name constants — update these to match your actual scene names
    public const string MAIN_MENU = "Chris_StartScreen";
    public const string LEVEL_SELECT = "Chris_LevelSelect";
    public const string GAME = "Chris_Game";

    [Header("Level Order — same order as Level Select buttons")]
    public LevelData[] levelOrder; // drag Level1, Level2, Level3 in order


    void Awake()
    {
        // Persist across scene loads so LevelData selection survives the transition
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
        // Store which level to load, then load the Game scene
        selectedLevel = levelData;
        Time.timeScale = 1;
        SceneManager.LoadScene(GAME);
    }

    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(GAME);
    }

    public void QuitGame()
    {
        Application.Quit();
        // In Editor this does nothing — the log below confirms it fired
        Debug.Log("Quit called");
    }

    public LevelData GetNextLevel()
    {
        if (selectedLevel == null || levelOrder == null) return null;

        for (int i = 0; i < levelOrder.Length - 1; i++)
        {
            if (levelOrder[i] == selectedLevel)
                return levelOrder[i + 1];
        }

        return null; // already on last level
    }

    // Holds the chosen LevelData between scenes
    // GameManager reads this in Awake to know which level to run
    [HideInInspector] public LevelData selectedLevel;
}