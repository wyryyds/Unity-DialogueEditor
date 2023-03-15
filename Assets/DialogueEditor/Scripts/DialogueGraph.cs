using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace HAITool.DialogueEditor
{
    public class DialogueGraph : EditorWindow
    {
        private DialogueGraphView m_graphView;
        private string m_fileName= "New Narrative";
        /// <summary>
        /// 打开面板的静态方法
        /// </summary>
        [MenuItem("HAITool/Dialogue Editor")]
        public static void OpenDialogueGraphWindow()
        {
            var window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("Dialogue Editor");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateGraphToolbar();
        }
        /// <summary>
        /// 初始化面板
        /// </summary>
        private void ConstructGraphView()
        {
            m_graphView = new DialogueGraphView { name = "Dialogue Graph" };
            m_graphView.StretchToParentSize();
            rootVisualElement.Add(m_graphView);
        }
        /// <summary>
        /// 创建视图工具栏
        /// </summary>
        private void GenerateGraphToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(m_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => m_fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });


            var nodeCreateButton = new Button(() => { m_graphView.CreateNode("New Node"); });
            nodeCreateButton.text = "Creat Node";
            toolbar.Add(nodeCreateButton);
            rootVisualElement.Add(toolbar);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(m_graphView);
        }

        private void RequestDataOperation(bool save)
        {
            if(string.IsNullOrEmpty(m_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name", "OK");
                return;
            }
            if(save)
            {
                GraphSaveUtility.Instance(m_graphView).SaveGraph(m_fileName);
                return;
            }
            GraphSaveUtility.Instance(m_graphView).LoadGraph(m_fileName);
        }

    }
}
