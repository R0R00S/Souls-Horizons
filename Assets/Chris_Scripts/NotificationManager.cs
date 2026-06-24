using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("UI References")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        notificationPanel.SetActive(false);
    }

    public void ShowNotification(string message, float duration)
    {
        StartCoroutine(DisplayNotification(message, duration));
    }

    IEnumerator DisplayNotification(string message, float duration)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);

        // Uses unscaled time so it works even if something pauses timeScale
        yield return new WaitForSecondsRealtime(duration);

        notificationPanel.SetActive(false);
    }
}