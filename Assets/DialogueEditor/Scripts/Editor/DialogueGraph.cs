using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

namespace HAITool.DialogueEditor.Editor
{
    public class DialogueGraph : EditorWindow
    {
        private DialogueGraphView _graphView;
        private string _fileName= "New Graph";
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
        
        // 初始化面板
        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView(this) { name = "Dialogue Graph" };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }
        // 创建视图工具栏
        private void GenerateGraphToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

            rootVisualElement.Add(toolbar);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void RequestDataOperation(bool save)
        {
            if(string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name", "OK");
                return;
            }
            if(save)
            {
                GraphSaveUtility.Instance(_graphView).SaveGraph(_fileName);
                return;
            }
            GraphSaveUtility.Instance(_graphView).LoadGraph(_fileName);
        }

    }
}
