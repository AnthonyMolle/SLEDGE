using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach to a Unity GameObject to run a chosen behavior tree on the gameObject!

public class BehaviorTreeRunner : MonoBehaviour
{
    BehaviorTree tree;


    // Start is called before the first frame update
    void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviorTree>();

        var logA = ScriptableObject.CreateInstance<DebugLog_Action>();
        logA.message = "A";

        var logB = ScriptableObject.CreateInstance<DebugLog_Action>();
        logB.message = "B";

        var logC = ScriptableObject.CreateInstance<DebugLog_Action>();
        logC.message = "C";

        var wait = ScriptableObject.CreateInstance<Wait_Action>();
        wait.duration = 1;

        var sequencer = ScriptableObject.CreateInstance<Sequencer_Composite>();

        sequencer.children.Add(wait);
        sequencer.children.Add(logA);
        sequencer.children.Add(wait);
        sequencer.children.Add(logB);
        sequencer.children.Add(wait);
        sequencer.children.Add(logC);

        var loop = ScriptableObject.CreateInstance<Repeat_Decorator>();
        loop.child = sequencer;

        loop.repeatNumber = 5;

        tree.rootNode = loop;

    }

    // Update is called once per frame
    void Update()
    {
        tree.Update();
    }
}
