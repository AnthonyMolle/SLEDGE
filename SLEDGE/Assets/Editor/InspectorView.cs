using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    Editor editor;
    Node currentNode;
    public InspectorView()
    {
        currentNode = null;
    }

    internal void UpdateSelection(NodeView nodeView)
    {
        Clear();

        if(currentNode != null)
        {
            currentNode.beingInspected = false;
        }
        UnityEngine.Object.DestroyImmediate(editor);

        currentNode = nodeView.node;
        editor = Editor.CreateEditor(currentNode);
        currentNode.beingInspected = true;

        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        Add(container);
    }
}
