using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HAITool.DialogueEditor
{
    public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer _containerCache;
        private List<Edge> Edges =>_targetGraphView.edges.ToList();
        private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();
        public static GraphSaveUtility Instance(DialogueGraphView targteGraphView)
        {
            return new GraphSaveUtility
            {
                _targetGraphView = targteGraphView
            };
        }

        public void SaveGraph(string fileName)
        {
            if (!Edges.Any()) return;

            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            for(int i=0;i<connectedPorts.Length;i++)
            {
                var outputNode = connectedPorts[i].output.node as DialogueNode;
                var inputNode = connectedPorts[i].input.node as DialogueNode;

                dialogueContainer.nodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    PortName = connectedPorts[i].output.portName,
                    TargetNodeGuid=inputNode.GUID
                });
            }

            foreach(var dialogueNode in Nodes.Where(node=>!node.EntryPoint))
            {
                dialogueContainer.dialogueNodeDatas.Add(new DialogueNodeData
                {
                    Guid = dialogueNode.GUID,
                    DialogueText=dialogueNode.DialogueText,
                    Position=dialogueNode.GetPosition().position
                });
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets","Resources");

            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();

        }

        public void LoadGraph(string fileName)
        {
            _containerCache = Resources.Load<DialogueContainer>(fileName);
            if(_containerCache==null)
            {
                EditorUtility.DisplayDialog("File Not Found", "target file does not exists!", "OK");
                return;
            }
            ClearGraph();
            CreatNodes();
            ConnectNodes();
        }

        private void ClearGraph()
        {
            Nodes.Find(x => x.EntryPoint).GUID = _containerCache.nodeLinks[0].BaseNodeGuid;
            foreach(var node in Nodes)
            {
                if (node.EntryPoint) continue;

                //https://stackoverflow.com/questions/23090459/ienumerable-where-and-tolist-what-do-they-really-do
                Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
                _targetGraphView.RemoveElement(node);
            }
        }
        private void CreatNodes()
        {
            foreach(var nodeData in _containerCache.dialogueNodeDatas)
            {
                var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText,Vector2.zero);
                tempNode.GUID = nodeData.Guid;
                _targetGraphView.AddElement(tempNode);

                var nodePorts = _containerCache.nodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
                nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
            }
        }
        private void ConnectNodes()
        {
            for(int i=0;i<Nodes.Count;i++)
            {
                var connections = _containerCache.nodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID).ToList();
                for(int j=0;j<connections.Count;j++)
                {
                    var targetGuid = connections[j].TargetNodeGuid;
                    var targetNode = Nodes.First(x=>x.GUID==targetGuid);
                    LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    DialogueGraphView _targetGraphView1 = _targetGraphView;
                    Rect rect = new Rect(_containerCache.dialogueNodeDatas.First(x => x.Guid == targetGuid).Position,
                                _targetGraphView1.DefaultNodeSize);

                    targetNode.SetPosition(rect);
                }
            }
        }

        private void LinkNodes(Port output,Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);

            _targetGraphView.Add(tempEdge);
        }
    }
}
