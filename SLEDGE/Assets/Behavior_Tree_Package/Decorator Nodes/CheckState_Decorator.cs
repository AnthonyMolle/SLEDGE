using UnityEngine;
using UnityEditor;
using System.Reflection;

public class CheckState_Decorator : DecoratorNode
{
    // Copied from blackboard to store what this checkState is looking for
    public Blackboard.EnemyStates currentState;
    public GameObject player;
    public float attackRange;
    public float alertRange;

    // Unique to state
    public FieldInfo currentField_blackboard;
    public FieldInfo currentField_local;

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
        if(currentField_blackboard == null || currentField_local == null)
        {
            return State.Failure;
        }

        object x = currentField_blackboard.GetValue(blackboard);
        object y = currentField_local.GetValue(this);

        if (x.ToString() != y.ToString())
        {
            return State.Failure;
        }
        
        return child.Update();
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
    public override Node Clone()
    {
        CheckState_Decorator node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}

[CustomEditor(typeof(CheckState_Decorator))]
public class CheckStateEditor : Editor
{
    SerializedProperty comparison; 
    string input = "";

    CheckState_Decorator last_decor;


    // Create Inspector UI 
    public override void OnInspectorGUI()
    {
        var script = target as CheckState_Decorator;

        if(last_decor != script)
        {
            input = script.local_input;
        }

        last_decor = script;

        input = EditorGUILayout.TextField("Enter Variable:", input);

        if (script.currentField_local != null)
        {
            script.description = "Check if " + input + " = " + script.currentField_local.GetValue(script);
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
