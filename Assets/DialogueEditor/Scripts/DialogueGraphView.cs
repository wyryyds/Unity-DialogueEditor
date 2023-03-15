using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace HAITool.DialogueEditor
{
    public class DialogueGraphView : GraphView
    {

        public readonly Vector2 DefaultNodeSize = new(150,200);

        public DialogueGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueEditor"));
            //�����ͼ���������
            SetupZoom(ContentZoomer.DefaultMinScale*2, ContentZoomer.DefaultMaxScale*2);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            AddElement(GenerateEntryPointNode());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
        }
        /// <summary>
        /// ���ɶ˿�
        /// </summary>
        /// <param name="node">��ǰ�ڵ�</param>
        /// <param name="portDirection">�˿ڷ���</param>
        /// <param name="capacity">��������</param>
        /// <returns></returns>
        private Port GeneratePort(DialogueNode node,Direction portDirection,Port.Capacity capacity=Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
        /// <summary>
        /// ���ɸ��ڵ�
        /// </summary>
        /// <returns></returns>
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
            generatePort.portName = "Next";
            node.outputContainer.Add(generatePort);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            //��Ӷ˿ں���Ҫˢ���Ӿ�Ч��
            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }
        /// <summary>
        /// ���ض˿ڼ��ݣ��ƶ��ڵ���ڵ�֮������ӹ���
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
        /// ���ɵ����Ի��ڵ�
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public DialogueNode CreateDialogueNode(string nodeName)
        {
            var newNode = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };
            var inputPort = GeneratePort(newNode, Direction.Input, Port.Capacity.Multi);
            inputPort.name = "input";
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
            newNode.SetPosition(new Rect(Vector2.zero, DefaultNodeSize));

            return newNode;
        }
        /// <summary>
        /// �������˿�
        /// </summary>
        /// <param name="node"></param>
        public void AddChoicePort(DialogueNode node,string overrideName="")
        {
            var generatedPort = GeneratePort(node, Direction.Output);

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);

            var outputPortCount = node.outputContainer.Query("connector").ToList().Count;

            var choiceName = string.IsNullOrEmpty(overrideName) ? $"Choice{outputPortCount + 1}" : overrideName;
            
            var textFiled = new TextField { name = string.Empty, value = choiceName };
            textFiled.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);

            generatedPort.contentContainer.Add(new Label(""));
            generatedPort.contentContainer.Add(textFiled);

            var deleteButton = new Button(() => RemovePort(node, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);

            generatedPort.name = choiceName;

            node.outputContainer.Add(generatedPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        private void RemovePort(DialogueNode node, Port generatedPort)
        {
            Debug.Log(edges.ToList().Count);
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

        /// <summary>
        /// ������õĶԻ��ڵ����ɷ���
        /// </summary>
        /// <param name="nodeName"></param>
        public void CreateNode(string nodeName)
        {
            AddElement(CreateDialogueNode(nodeName));
        }
    }
}
