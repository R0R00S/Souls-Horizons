using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Event")]
    [SerializeField] EventReference gameplayMusicEvent;

    [Header("Parameter")]
    [SerializeField] string progressionParam = "MusicState"; // match exact FMOD parameter name

    EventInstance musicInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(gameplayMusicEvent);
        musicInstance.start();
    }

    public void SetMusicState(float value)
    {
        musicInstance.setParameterByName(progressionParam, value);
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}