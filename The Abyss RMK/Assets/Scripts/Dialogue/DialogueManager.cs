using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    //Componentes del Dialogue Menu
    public Image characterIcon;
    public Image npcIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI npcName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;
    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null) { Debug.LogWarning("ES NULO"); }
        isDialogueActive = true;

        UICanvas.instance.StartMenuDialogue();

        lines.Clear();

        foreach (DialogueLine line in dialogue.DialogueLines)
        {
            lines.Enqueue(line);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }
        DialogueLine currentLine = lines.Dequeue();
        characterIcon.sprite = currentLine.Character.Icon;
        npcIcon.sprite = currentLine.NPC.Icon;
        characterName.text = currentLine.Character.Name;
        npcName.text = currentLine.NPC.Name;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
    }
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.Line.ToCharArray())
        {
            dialogueArea.text += letter;
        }
        yield return null;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        UICanvas.instance.HideMenuDialogue();
    }

}
