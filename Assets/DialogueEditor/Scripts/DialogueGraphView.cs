using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;

namespace HAITool.DialogueEditor
{
    public class DialogueGraphView : GraphView
    {

        private readonly static Vector2 defaultNodeSize = new(150,200);

        public DialogueGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueEditor"));
            //添加视图的鼠标缩放
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
        /// 生成端口
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="portDirection">端口方向</param>
        /// <param name="capacity">输入容量</param>
        /// <returns></returns>
        private Port GeneratePort(DialogueNode node,Direction portDirection,Port.Capacity capacity=Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
        /// <summary>
        /// 生成根节点
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
            //添加端口后需要刷新视觉效果
            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
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
        /// 生成单个对话节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private DialogueNode CreateDialogueNode(string nodeName)
        {
            var newNode = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };
            var inputPort = GeneratePort(newNode, Direction.Input, Port.Capacity.Multi);
            inputPort.name = "Input";
            newNode.inputContainer.Add(inputPort);

            var button = new Button(() => { AddChoicePort(newNode); });

            newNode.titleContainer.Add(button);
            button.text = "New Choice";

            newNode.RefreshExpandedState();
            newNode.RefreshPorts();
            newNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

            return newNode;
        }
        /// <summary>
        /// 添加多个输出端口
        /// </summary>
        /// <param name="node"></param>
        private void AddChoicePort(DialogueNode node)
        {
            var generatedPort = GeneratePort(node, Direction.Output);

            var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
            var outputPortName = $"Choice {outputPortCount}";

            node.outputContainer.Add(generatedPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }
        /// <summary>
        /// 供外调用的对话节点生成方法
        /// </summary>
        /// <param name="nodeName"></param>
        public void CreateNode(string nodeName)
        {
            AddElement(CreateDialogueNode(nodeName));
        }
    }
}
