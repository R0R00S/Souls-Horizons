using UnityEngine;
using System.Collections;

public class FMODAudioUnlocker : MonoBehaviour
{
    private static FMODAudioUnlocker instance;
    private bool unlocked = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (unlocked) return;

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            StartCoroutine(UnlockAudio());
        }
    }

    IEnumerator UnlockAudio()
    {
        unlocked = true;

        FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
        yield return new WaitForSecondsRealtime(0.1f);
        FMODUnity.RuntimeManager.CoreSystem.mixerResume();

        Debug.Log("FMOD audio context unlocked");
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
        }
        else
        {
            FMODUnity.RuntimeManager.CoreSystem.mixerResume();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
        }
        else
        {
            FMODUnity.RuntimeManager.CoreSystem.mixerResume();
        }
    }
}