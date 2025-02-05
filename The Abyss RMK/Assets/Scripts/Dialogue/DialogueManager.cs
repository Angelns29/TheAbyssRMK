using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
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

    //Multilanguage
    public LocalizeStringEvent eventString;
    public string textEspanol;
    public string textEnglish;
    public string textCatalan;

    public LocalizeStringEvent eventStringNPC;
    public string NPCEspanol;
    public string NPCEnglish;
    public string NPCCatalan;
    
    public LocalizeStringEvent eventStringCharacter;
    public string CharacterEspanol;
    public string CharacterEnglish;
    public string CharacterCatalan;

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
        //SelectLanguageText(currentLine);
        characterIcon.sprite = currentLine.Character.Icon;
        npcIcon.sprite = currentLine.NPC.Icon;
        //characterName.text = currentLine.Character.Name;
        //npcName.text = currentLine.NPC.Name;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
        
    }
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        npcName.text = "";
        characterName.text = "";
        textEspanol = dialogueLine.Line.textEspanol;
        textCatalan = dialogueLine.Line.textCatalan;
        textEnglish = dialogueLine.Line.textIngles;
        eventString.StringReference.RefreshString();


        NPCEspanol = dialogueLine.NPC.Name.textEspanol;
        NPCEnglish = dialogueLine.NPC.Name.textIngles;
        NPCCatalan = dialogueLine.NPC.Name.textCatalan;
        eventStringNPC.StringReference.RefreshString();


        CharacterEspanol = dialogueLine.Character.Name.textEspanol;
        CharacterEnglish = dialogueLine.Character.Name.textIngles;
        CharacterCatalan = dialogueLine.Character.Name.textCatalan;
        eventStringCharacter.StringReference.RefreshString();
        /*foreach (char letter in dialogueLine.Line)
        {
            dialogueArea.text += letter;
        }*/
        yield return null;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        UICanvas.instance.HideMenuDialogue();
    }

}
