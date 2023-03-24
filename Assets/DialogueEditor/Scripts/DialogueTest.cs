using HAITool.DialogueEditor.RunTime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTest : MonoBehaviour
{
    public TextMeshProUGUI _mainDialogueText;

    public List<Button> ChoicesButton;

    private readonly static string containerPath = "SimpleDialogue";

    private void Start()
    {
        DialogueManager.Instance.LoadDialogue(containerPath, _mainDialogueText, ChoicesButton);
    }

   
}
