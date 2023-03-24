using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HAITool.DialogueEditor.RunTime
{
    public class DialogueManager
    {
        private static DialogueManager instance;
        private DialogueContainer _dialogueCounter;
        public static DialogueManager Instance
        {
            get 
            {
                instance ??= new DialogueManager();
                return  instance;
            }
        }
        /// <summary>
        /// 加载对话系统
        /// </summary>
        /// <param name="containerName">待加载对话资源名称</param>
        /// <param name="textMeshProUGUI">承载节点内容的TextMeshPro组件</param>
        /// <param name="buttons">承载选项分支的button组件列表</param>
        public void LoadDialogue(string containerName, TextMeshProUGUI textMeshProUGUI, List<Button> buttons)
        {
            Instance._dialogueCounter = Resources.Load<DialogueContainer>(containerName);
            if (Instance._dialogueCounter == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("The name object does not exist!");
#endif          
                return;
            }
            //拿到根节点之后的第一个节点
            var entryNodeTargetGuid = _dialogueCounter.nodeLinks.First().TargetNodeGuid;
            UpdateDialogue(entryNodeTargetGuid, textMeshProUGUI, buttons);
        }
        
        /// <summary>
        /// 加载对话系统
        /// </summary>
        /// <param name="containerName">待加载的对话资源名称</param>
        /// <param name="mainText">承载节点内容的TextMeshPro组件</param>
        /// <param name="buttons">承载选项分支的button组件列表</param>
        public void LoadDialogue(string containerName,Text mainText,List<Button> buttons)
        {
            Instance._dialogueCounter = Resources.Load<DialogueContainer>(containerName);
            if (Instance._dialogueCounter == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("The name object does not exist!");
#endif          
                return;
            }
            //拿到根节点之后的第一个节点
            var entryNodeTargetGuid = _dialogueCounter.nodeLinks.First().TargetNodeGuid;
            UpdateDialogue(entryNodeTargetGuid, mainText, buttons);
        }
        
        private void UpdateDialogue(string targetNodeGuid,TextMeshProUGUI textMeshProUGUI, List<Button> choicesButtons)
        {
            textMeshProUGUI.text = _dialogueCounter.dialogueNodeDatas.Find(x => x.Guid == targetNodeGuid).DialogueText;
            var choices = _dialogueCounter.nodeLinks.Where(x => x.BaseNodeGuid == targetNodeGuid);
            int index = 0;
            if (!choices.Any())
            {
                foreach (var button in choicesButtons)
                {
                    button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    button.onClick.RemoveAllListeners();
                }
            }
            else
            {
                foreach (var choice in choices)
                {
                    choicesButtons[index].GetComponentInChildren<TextMeshProUGUI>().text=choice.PortName;
                    choicesButtons[index].onClick.AddListener(() => UpdateDialogue(choice.TargetNodeGuid,textMeshProUGUI, choicesButtons));
                    index++;
                }
            }
        }
        private void UpdateDialogue(string targetNodeGuid, Text mainText, List<Button> choicesButtons)
        {
            mainText.text = _dialogueCounter.dialogueNodeDatas.Find(x => x.Guid == targetNodeGuid).DialogueText;
            var choices = _dialogueCounter.nodeLinks.Where(x => x.BaseNodeGuid == targetNodeGuid);
            int index = 0;
            if (!choices.Any())
            {
                foreach (var button in choicesButtons)
                {
                    button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    button.onClick.RemoveAllListeners();
                }
            }
            else
            {
                foreach (var choice in choices)
                {
                    choicesButtons[index].GetComponentInChildren<Text>().text = choice.PortName;
                    choicesButtons[index].onClick.AddListener(() => UpdateDialogue(choice.TargetNodeGuid,mainText,choicesButtons));
                    index++;
                }
            }
        }
    }
}
