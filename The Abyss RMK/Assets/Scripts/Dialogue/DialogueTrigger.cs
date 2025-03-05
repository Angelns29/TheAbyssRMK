using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public ScriptableDialogues Name;
    public Sprite Icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter Character;
    public DialogueCharacter NPC;
    public ScriptableDialogues Line;
}
[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> DialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject npcDialogueIcon;
    bool startDialogue = false;
    public GameObject imageNPC;

    private void FixedUpdate()
    {
        if (CharacterMovement.instance.interactInput && startDialogue)
        {
            DialogueManager.instance.StartDialogue(dialogue);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            npcDialogueIcon.SetActive(true);
            startDialogue = true;
            imageNPC.SetActive(true);
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            npcDialogueIcon.SetActive(false);
            startDialogue= false;
        }
    }
}
