using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CheckState_Decorator : DecoratorNode
{
    public string _string;
    public int _i;
    public float _f;
    public GameObject _object;
    public Blackboard.EnemyStates _enum;

    public string BlackboardVariableName;
    public FieldInfo BlackboardInfo;

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {


        if(BlackboardInfo == null && BlackboardVariableName != null)
        {
            BlackboardInfo = blackboard.GetType().GetField(BlackboardVariableName);
        }

        var obj = BlackboardInfo.GetValue(blackboard);

        if (BlackboardInfo?.FieldType == typeof(string))
        {
            if((string) obj != _string)
            {
                return State.Failure;
            }
        }
        else if (BlackboardInfo?.FieldType == typeof(int))
        {
            if ((int)obj != _i)
            {
                return State.Failure;
            }
        }
        else if (BlackboardInfo?.FieldType == typeof(float))
        {
            if ((float)obj != _f)
            {
                return State.Failure;
            }
        }
        else if (BlackboardInfo?.FieldType == typeof(Blackboard.EnemyStates))
        {
            if ((Blackboard.EnemyStates)obj != _enum)
            {
                return State.Failure;
            }
        }
        else if (BlackboardInfo is { } && BlackboardInfo.FieldType.IsClass)
        {
            if ((GameObject)obj != _object)
            {
                return State.Failure;
            }
        }

        // Otherwise pass through the node execution

        return child.Update();
    }
}

[CustomEditor(typeof(CheckState_Decorator))]
public class CheckStateEditor: Editor
{

    // Create Inspector UI 
    public override void OnInspectorGUI()
    {
        var script = target as CheckState_Decorator;

        script.BlackboardVariableName = EditorGUILayout.TextField("Blackboard Variable Name: ", script.BlackboardVariableName);

        if (script.BlackboardVariableName != null)
        {
            script.BlackboardInfo = script.blackboard.GetType().GetField(script.BlackboardVariableName); // Get Type to show in inspector

            DisplayGuiLayoutForType(script.BlackboardInfo);
        }
    }

    private void DisplayGuiLayoutForType(FieldInfo field)
    {
        var script = target as CheckState_Decorator;

        if (field?.FieldType == typeof(string))
        { 
            script._string = EditorGUILayout.TextArea(script._string);
        }
        else if (field?.FieldType == typeof(int))
        {
            script._i = EditorGUILayout.IntField(script._i);
        }
        else if (field?.FieldType == typeof(float))
        {
            script._f = EditorGUILayout.FloatField(script._f);
        }else if (field?.FieldType == typeof(Blackboard.EnemyStates))
        {
            script._enum = (Blackboard.EnemyStates)EditorGUILayout.EnumPopup("Test:", script._enum);
        }
        else if (field is { } && field.FieldType.IsClass)
        {
            var obj = field.FieldType;
            script._object = (GameObject) EditorGUILayout.ObjectField(script._object, obj, true);
        }
    }
}
