using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [SerializeField] Switchable[] controlledObjects;
    Switch[] connectedSwitches;

    [SerializeField] bool initialSwitchState = true;

    [SerializeField] bool isTimed = false;
    [SerializeField] float activeTime = 0;
    float timer = 0;

    private void Start()
    {
        connectedSwitches = GetComponentsInChildren<Switch>();

        foreach (Switch connectedSwitch in connectedSwitches)
        {
            connectedSwitch.SetState(initialSwitchState);
        }
    }

    private void Update()
    {
        if (isTimed && timer > 0)
        {
            timer -= Time.deltaTime;
            
            if (timer <= 0)
            {
                foreach (Switchable controllableObject in controlledObjects)
                {
                    controllableObject.SwitchState();
                }

                foreach (Switch connectedSwitch in connectedSwitches)
                {
                    connectedSwitch.SwapStatePassive();
                }
            }
        }
        else
        {
            return;
        }
    }

    public void Swap()
    {
        if (!isTimed)
        {
            foreach (Switchable controllableObject in controlledObjects)
            {
                controllableObject.SwitchState();
            }

            foreach (Switch connectedSwitch in connectedSwitches)
            {
                connectedSwitch.SwapStatePassive();
            }
        }
        else
        {
            if (timer > 0)
            {
                timer = activeTime;
            }
            else
            {
                timer = activeTime;
                foreach (Switchable controllableObject in controlledObjects)
                {
                    controllableObject.SwitchState();
                }

                foreach (Switch connectedSwitch in connectedSwitches)
                {
                    connectedSwitch.SwapStatePassive();
                }
            }
        }
    }
}
