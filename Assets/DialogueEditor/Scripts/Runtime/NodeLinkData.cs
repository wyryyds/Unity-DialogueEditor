using System;
using UnityEngine;

namespace HAITool.DialogueEditor.RunTime
{
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string PortName;
        public string TargetNodeGuid;
    }
}