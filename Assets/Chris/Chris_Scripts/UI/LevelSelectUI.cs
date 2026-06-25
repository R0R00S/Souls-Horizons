using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    [Header("Level Data Assets — drag from Project window")]
    public LevelData level1Data;
    public LevelData level2Data;
    public LevelData level3Data;

    // Hook each button to its matching method

    public void OnLevel1Pressed()
    {
        SceneLoader.Instance.LoadLevel(level1Data);
    }

    public void OnLevel2Pressed()
    {
        SceneLoader.Instance.LoadLevel(level2Data);
    }

    public void OnLevel3Pressed()
    {
        SceneLoader.Instance.LoadLevel(level3Data);
    }

    public void OnBackPressed()
    {
        SceneLoader.Instance.GoToMainMenu();
    }
}