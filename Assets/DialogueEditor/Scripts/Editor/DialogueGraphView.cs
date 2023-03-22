using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor;
using HAITool.DialogueEditor.RunTime;

namespace HAITool.DialogueEditor.Editor
{
    public class DialogueGraphView : GraphView
    {

        public readonly Vector2 DefaultNodeSize = new(150, 200);

        private static readonly string InputPortNameStr = "input";
        private static readonly string OutputPortNameStr = "Choice";
        private static readonly string DeleteButtonStr = "X";

        private NodeSearchWindow _searchWindow;

        public DialogueGraphView(EditorWindow editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueEditor"));
            //添加视图的鼠标缩放
            SetupZoom(ContentZoomer.DefaultMinScale * 2, ContentZoomer.DefaultMaxScale * 2);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode());
            AddSearchWindow(editorWindow);
        }
        /// <summary>
        /// 供外调用的对话节点生成方法
        /// </summary>
        /// <param name="nodeName"></param>
        public void CreateNode(string nodeName,Vector2 position)
        {
            AddElement(CreateDialogueNode(nodeName,position));
        }
        /// <summary>
        /// 重载端口兼容，制定节点与节点之间的连接规则
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        /// <summary>
        /// 生成单个节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public DialogueNode CreateDialogueNode(string nodeName,Vector2 position)
        {
            var newNode = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };
            var inputPort = GeneratePort(newNode, Direction.Input, Port.Capacity.Multi);
            inputPort.name = InputPortNameStr;
            newNode.inputContainer.Add(inputPort);

            newNode.styleSheets.Add(Resources.Load<StyleSheet>("DialogueNode"));

            var button = new Button(() => { AddChoicePort(newNode); });

            newNode.titleContainer.Add(button);
            button.text = "New Choice";

            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                newNode.DialogueText = evt.newValue;
                newNode.title = evt.newValue;
            });

            textField.SetValueWithoutNotify(newNode.title);
            newNode.mainContainer.Add(textField);

            newNode.RefreshExpandedState();
            newNode.RefreshPorts();
            newNode.SetPosition(new Rect(position, DefaultNodeSize));

            return newNode;
        }
        /// <summary>
        /// 添加输出端口
        /// </summary>
        /// <param name="node"></param>
        public void AddChoicePort(DialogueNode node, string overrideName = "")
        {
            var generatedPort = GeneratePort(node, Direction.Output);

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);

            var outputPortCount = node.outputContainer.Query("connector").ToList().Count;

            var choiceName = string.IsNullOrEmpty(overrideName) ? $"{OutputPortNameStr}{outputPortCount + 1}" : overrideName;

            var textFiled = new TextField { name = string.Empty, value = choiceName };
            textFiled.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);

            generatedPort.contentContainer.Add(new Label(""));
            generatedPort.contentContainer.Add(textFiled);

            var deleteButton = new Button(() => RemovePort(node, generatedPort))
            {
                text = DeleteButtonStr
            };
            generatedPort.contentContainer.Add(deleteButton);

            generatedPort.name = choiceName;

            node.outputContainer.Add(generatedPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }
        /// <summary>
        /// 生成端口
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="portDirection">端口方向</param>
        /// <param name="capacity">输入容量</param>
        /// <returns></returns>
        private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
        // 生成根节点
        private DialogueNode GenerateEntryPointNode()
        {
            var node = new DialogueNode
            {
                title = "Entry",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRY",
                EntryPoint = true
            };

            var generatePort = GeneratePort(node, Direction.Output);
            generatePort.portName = "Output";
            node.outputContainer.Add(generatePort);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            //添加端口后需要刷新视觉效果
            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }
        //移除端口
        private void RemovePort(DialogueNode node, Port generatedPort)
        {
            var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
            if (!targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(generatedPort);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }
        //添加搜索窗口
        private void AddSearchWindow(EditorWindow editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Init(this,editorWindow);
            nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);

        }

    }
}
