using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class UpdateBlackboard_Action : ActionNode
{
    // Copied from Blackboard -- 
    public Blackboard.EnemyStates currentState;
    public float attackRange;
    public float alertRange;
    public string objectAName;
    public string objectBName;

    public GameObject currentRunner;
    public GameObject objectA;
    public GameObject objectB;
    // Copied from Blackboard -- 

    public FieldInfo currentField_local;
    public FieldInfo currentField_blackboard;
    public string local_input;



    protected override void OnStart()
    {
        updateField(local_input);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (currentField_blackboard == null || currentField_local == null)
        {
            return State.Failure;
        }

        object ourValue = currentField_local.GetValue(this);

        currentField_blackboard.SetValue(blackboard, ourValue);

        child.state = State.Running;
        child.Update();
        return State.Success;
    }

    public void updateField(string name)
    {
        local_input = name;

        FieldInfo[] fields = blackboard.GetType().GetFields();

        foreach (var field in fields)
        {
            if (field.Name == name)
            {
                currentField_blackboard = field;
                break;
            }
        }

        FieldInfo[] theseFields = this.GetType().GetFields();

        foreach (var field in theseFields)
        {
            if (field.Name == name)
            {
                currentField_local = field;
                break;
            }
        }
    }
}



[CustomEditor(typeof(UpdateBlackboard_Action))]
public class UpdateBlackboardEditor : Editor
{
    SerializedProperty comparison;
    string input = "";

    UpdateBlackboard_Action last_ref;


    // Create Inspector UI 
    public override void OnInspectorGUI()
    {
        var script = target as UpdateBlackboard_Action;

        if (last_ref != script)
        {
            input = script.local_input;
        }

        last_ref = script;

        input = EditorGUILayout.TextField("Enter Variable:", input);

        if (script.currentField_local != null)
        {
            script.description = "Set " + input + " = " + script.currentField_local.GetValue(script);
        }

        if (input != null)
        {
            // Goal is to have generic SerializedProperty that can be modified 

            // This will be the value compared to the matching blackboard name.

            // This finds one of the above containers that match the variables in our blackboard
            comparison = serializedObject.FindProperty(input);

            if (comparison != null)
            {
                // This tells the inspector to update the matching container when modified
                EditorGUILayout.PropertyField(comparison);
                script.updateField(input);
            }
        }
        if (serializedObject != null)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
