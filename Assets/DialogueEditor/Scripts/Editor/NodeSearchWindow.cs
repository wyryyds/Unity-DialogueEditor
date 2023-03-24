using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using HAITool.DialogueEditor.RunTime;

namespace HAITool.DialogueEditor.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {

        private DialogueGraphView _graphView;
        private EditorWindow  _editorWindow;

        private Texture2D _indentationIcon;

        /// <summary>
        /// 初始化节点查找窗口
        /// </summary>
        /// <param name="dialogueGraphView"></param>
        /// <param name="editorWindow"></param>
        public void Init(DialogueGraphView dialogueGraphView,EditorWindow editorWindow)
        {
            _graphView = dialogueGraphView;
            _editorWindow = editorWindow;

            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0,0));
            _indentationIcon.Apply();
        }
        /// <summary>
        /// 创建层级
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"),0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue"),1),
            new SearchTreeEntry(new GUIContent("Dialogue Node",_indentationIcon))
            {
                userData=new DialogueNode(),level = 2
            }
        };
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(
                _editorWindow.rootVisualElement.parent, context.screenMousePosition-_editorWindow.position.position);
            var localMousePosition = _graphView.contentContainer.WorldToLocal(worldMousePosition);
            switch (SearchTreeEntry.userData)
            {
                case DialogueNode:
                    _graphView.CreateNode("Dialogue Node",localMousePosition);
                    return true;
                default: return false;
            }

        }
    }
}
