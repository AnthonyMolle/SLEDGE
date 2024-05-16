using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public class BehaviorTreeView : GraphView
{
    public new class UxmlFactory: UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }
    public BehaviorTreeView()
    {

        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviorTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }
}
