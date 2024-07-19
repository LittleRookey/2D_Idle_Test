using Litkey.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCInteractor : Interactor
{
    [Header("NPC-specific Settings")]
    [SerializeField] private string _npcName;
    [SerializeField] private string[] _dialogueLines;
    public UnityEvent OnDialogueEnd;
    private int _currentDialogueIndex = 0;

    public override void Interact(PlayerController player, UnityAction OnEnd = null)
    {
        if (!CanInteract(player))
        {
            WarningMessageInvoker.Instance.ShowMessage($"Cannot interact with {_npcName} right now.");
            return;
        }
        player.DisableMovement();
        if (_dialogueLines != null)
        {
            StartCoroutine(DialogueCoroutine(player, OnEnd));
        }
    }

    private IEnumerator DialogueCoroutine(PlayerController player, UnityAction OnEnd)
    {
        player.DisableMovement();

        while (_currentDialogueIndex < _dialogueLines.Length)
        {
            ShowNextDialogue();
            yield return new WaitForSeconds(_interactionTime / _dialogueLines.Length);
        }

        _currentDialogueIndex = 0;
        SetToCooldown(_cooldownTime);
        player.EnableMovement();
        OnEnd?.Invoke();
        OnDialogueEnd?.Invoke();
    }

    private void ShowNextDialogue()
    {
        if (_currentDialogueIndex < _dialogueLines.Length)
        {
            // Implement your dialogue system here
            Debug.Log($"{_npcName}: {_dialogueLines[_currentDialogueIndex]}");
            _currentDialogueIndex++;
        }
    }

    // Override if you want different cooldown behavior for NPCs
    public override bool CanInteract(PlayerController player)
    {
        return base.CanInteract(player);
    }
}