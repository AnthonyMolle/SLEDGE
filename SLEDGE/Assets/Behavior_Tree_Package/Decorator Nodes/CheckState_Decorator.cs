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
    public string variableName = "";
    public FieldInfo currentField_blackboard;
    public FieldInfo currentField_local;


    protected override void OnStart()
    {
        updateField();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {

        Debug.Log("Blackboard field: " + currentField_blackboard + " Blackboard Value: " + currentField_blackboard.GetValue(blackboard));
        Debug.Log("Local field: " + currentField_local + " Local Value: " + currentField_local.GetValue(this));
        if (currentField_blackboard == null || currentField_local == null || currentField_blackboard.GetValue(blackboard) != currentField_local.GetValue(this))
        {
            return State.Failure;
        } 

        return child.Update();
    }

    public void updateField()
    {
        FieldInfo[] fields = blackboard.GetType().GetFields();

        foreach (var field in fields)
        {
            if (field.Name == variableName)
            {
                currentField_blackboard = field;
                break;
            }
        }

        FieldInfo[] theseFields = this.GetType().GetFields();

        foreach (var field in theseFields)
        {
            if (field.Name == variableName)
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
    // Create Inspector UI 
    public override void OnInspectorGUI()
    {
        var script = target as CheckState_Decorator;
        script.variableName = EditorGUILayout.TextField("Enter Variable:", script.variableName);
     
        if (script.variableName != null)
        {
            // Goal is to have generic SerializedProperty that can be modified 

            // This will be the value compared to the matching blackboard name.

            // This finds one of the above containers that match the variables in our blackboard
            SerializedProperty comparison = serializedObject.FindProperty(script.variableName);

            if (comparison != null)
            {
                // This tells the inspector to update the matching container when modified
                EditorGUILayout.PropertyField(comparison);
                script.updateField();
            }
        }
        if (serializedObject != null)
        {
            serializedObject.ApplyModifiedProperties();
        }

        /*        if (GUI.changed)
                {
                    if (script.comparison != null)
                    {
                        object tempValue = script.comparison.boxedValue;

                        if (tempValue != null)
                        {
                            script.lastValue = tempValue;
                        }

                        script.carryOver = 100;
                    }
                    script.updateField();
                }*/
    }
}
