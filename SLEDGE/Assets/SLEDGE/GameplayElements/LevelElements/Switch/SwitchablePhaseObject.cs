using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchablePhaseObject : Switchable
{
    [SerializeField] bool initialState = false;
    bool currentState;
    MeshRenderer mr;
    Collider collider;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();

        mr.enabled = initialState;
        collider.enabled = initialState;
        currentState = initialState;
    }

    public override void SwitchState()
    {
        mr.enabled = !currentState;
        collider.enabled = !currentState;
        currentState = !currentState;
    }
}
