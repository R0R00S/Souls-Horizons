using UnityEngine;
using UnityEngine.UI;

public class LifeIconAudioHook : MonoBehaviour
{
    Image icon;
    bool wasEnabled;

    void Start()
    {
        icon = GetComponent<Image>();
        wasEnabled = icon.enabled;
    }

    void Update()
    {
        if (wasEnabled && !icon.enabled)
        {
            AudioManager.Instance.PlayLoseLife();
        }
        wasEnabled = icon.enabled;
    }
}