using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach to a Unity GameObject to run a chosen behavior tree on the gameObject!

public class BehaviorTreeRunner : MonoBehaviour
{
    public BehaviorTree tree;

    // Start is called before the first frame update
    void Start()
    {
        tree = tree.Clone();
        tree.Bind();
    }

    // Update is called once per frame
    void Update()
    {
        tree.Update();
    }
}
