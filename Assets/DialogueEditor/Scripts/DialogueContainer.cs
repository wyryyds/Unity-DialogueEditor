using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HAITool.DialogueEditor
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> nodeLinks = new ();
        public List<DialogueNodeData> dialogueNodeDatas = new();
    }
}
