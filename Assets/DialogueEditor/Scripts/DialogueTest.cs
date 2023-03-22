using HAITool.DialogueEditor.RunTime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTest : MonoBehaviour
{
    public TextMeshProUGUI _mainDialogueText;

    public List<Button> ChoicesButton;

    public DialogueContainer _dialogueCounter;

    private void Start()
    {
        var entryNodeData = _dialogueCounter.nodeLinks.First();
        UpdateDialogue(entryNodeData.TargetNodeGuid);
    }

    private void UpdateDialogue(string targetNodeGuid)
    {
        var text = _dialogueCounter.dialogueNodeDatas.Find(x => x.Guid == targetNodeGuid).DialogueText;
        _mainDialogueText.text = text;
        var choices = _dialogueCounter.nodeLinks.Where(x => x.BaseNodeGuid == targetNodeGuid);
        int index = 0;
        if (!choices.Any())
        {
            foreach(var button in ChoicesButton)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                button.onClick.RemoveAllListeners();
            }
        }
        else
        {
            foreach (var choice in choices)
            {
                ChoicesButton[index].GetComponentInChildren<TextMeshProUGUI>().text = choice.PortName;
                ChoicesButton[index].onClick.AddListener(() => UpdateDialogue(choice.TargetNodeGuid));
                index++;
            }
        }
    }
}
