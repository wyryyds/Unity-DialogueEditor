using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HAITool.DialogueEditor
{
    public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;

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

        }
    }
}
