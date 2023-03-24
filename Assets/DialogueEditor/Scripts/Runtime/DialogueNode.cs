using UnityEditor.Experimental.GraphView;

namespace HAITool.DialogueEditor.RunTime
{
    public class DialogueNode : Node
    {
        public string GUID;
        public string DialogueText;
        public bool EntryPoint = false;
    }
}
