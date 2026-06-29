using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDialogueData", menuName = "Game/Level Dialogue Data")]
public class LevelDialogueData : ScriptableObject
{
    [Header("Opening Dialogue — plays before level starts")]
    public DialogueSequence openingSequence;

    [Header("Mid-Level Notifications — appear automatically during play")]
    public List<LevelNotification> notifications;
}

// A sequence is a series of dialogue lines the player taps through
[System.Serializable]
public class DialogueSequence
{
    public bool enabled = true;
    [TextArea(2, 5)]
    public List<string> lines; // designer types each line directly in the Inspector
}

// A notification is a single timed message that appears mid-level
[System.Serializable]
public class LevelNotification
{
    [Tooltip("Notification appears when the timer reaches this many seconds remaining. " +
             "e.g. set to 30 to show when countdown hits 30")]
    public float triggerAtTimeRemaining = 30f;

    [TextArea(2, 4)]
    public string message;

    [Tooltip("How many seconds the notification stays visible")]
    public float displayDuration = 3f;

    [HideInInspector] public bool hasTriggered = false;
}