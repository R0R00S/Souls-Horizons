using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continuePromptText; // e.g. "Tap to continue"
    public GameObject skipButton;

    private List<string> currentLines;
    private int currentLineIndex = 0;
    private bool isWaitingForTap = false;
    private System.Action onSequenceComplete;

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

   

    // Called by GameManager at the start — plays opening dialogue then calls back
    public void PlayOpeningSequence(DialogueSequence sequence, System.Action onComplete)
    {
        if (sequence == null || !sequence.enabled || sequence.lines.Count == 0)
        {
            // No dialogue configured — start immediately
            onComplete?.Invoke();
            return;
        }

        onSequenceComplete = onComplete;
        currentLines = sequence.lines;
        currentLineIndex = 0;

        // Pause game while dialogue plays
        Time.timeScale = 0;
        GameManager.Instance.isGameActive = false;

        dialoguePanel.SetActive(true);
        ShowCurrentLine();

        dialoguePanel.SetActive(true);
        Debug.Log("activeSelf: " + dialoguePanel.activeSelf +
                  " | activeInHierarchy: " + dialoguePanel.activeInHierarchy);
    }

    void ShowCurrentLine()
    {
        if (currentLineIndex >= currentLines.Count)
        {
            FinishSequence();
            return;
        }

        dialogueText.text = currentLines[currentLineIndex];
        isWaitingForTap = true;

        // Show continue prompt on all lines except the last
        bool isLastLine = currentLineIndex == currentLines.Count - 1;
        continuePromptText.text = isLastLine ? "Tap to start" : "Tap to continue";
    }

    void Update()
    {
        if (!isWaitingForTap) return;

        // Detect tap on mobile or click in Editor
        bool tapped = Input.touchCount > 0
            ? Input.GetTouch(0).phase == TouchPhase.Began
            : Input.GetMouseButtonDown(0);

        if (tapped)
        {
            currentLineIndex++;
            ShowCurrentLine();
        }
    }

    void FinishSequence()
    {
        isWaitingForTap = false;
        dialoguePanel.SetActive(false);

        // Resume game — GameManager.StartLevel handles this via the callback
        onSequenceComplete?.Invoke();
    }

    // Called by skip button if you add one
    public void OnSkipPressed()
    {
        FinishSequence();
    }
}