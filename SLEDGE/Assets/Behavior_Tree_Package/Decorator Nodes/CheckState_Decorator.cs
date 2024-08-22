using UnityEngine;
using UnityEditor;
using System.Reflection;
using static Blackboard;

// Check a blackboard variable == valueToCheckFor
public class CheckState_Decorator : DecoratorNode
{
    public string blackboard_var_name;
    public FieldInfo blackboard_var;
    public string valueToCheckFor;

    protected override void OnStart()
    {
        blackboard_var = GetBlackboardVarByName(blackboard_var_name);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (blackboard_var == null)
        {
            return State.Failure;
        }

        object x = blackboard_var.GetValue(blackboard);

        if (x.ToString() != this.valueToCheckFor)
        {
            return State.Failure;
        }

        child.state = State.Running;
        return child.Update();
    }

    public FieldInfo GetBlackboardVarByName(string name)
    {
        blackboard_var_name = name;

        FieldInfo[] fields = blackboard.GetType().GetFields();

        foreach (var field in fields)
        {
            if (field.Name == name)
            {

                return field;
            }
        }

        return null;
    }

    public override Node Clone()
    {
        CheckState_Decorator node = Instantiate(this);
        node.child = child.Clone();
        node.state = State.Running;
        node.started = false;
        return node;
    }
}

[CustomEditor(typeof(CheckState_Decorator))]
public class CheckStateEditor : Editor
{
    string variableName = "";

    CheckState_Decorator last_opened_checkStateClass;

    // Create Inspector UI 
    public override void OnInspectorGUI()
    {
        var checkStateClass = target as CheckState_Decorator;

        if (last_opened_checkStateClass != checkStateClass)
        {
            variableName = checkStateClass.blackboard_var_name;
            last_opened_checkStateClass = checkStateClass;
        }

        variableName = EditorGUILayout.TextField("Enter Variable:", variableName);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("valueToCheckFor"));

        checkStateClass.description = "Check if " + variableName + " = " + checkStateClass.valueToCheckFor;

        if (variableName != checkStateClass.blackboard_var_name)
        {
            checkStateClass.blackboard_var = checkStateClass.GetBlackboardVarByName(variableName);
        }
        
        if (serializedObject != null)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
