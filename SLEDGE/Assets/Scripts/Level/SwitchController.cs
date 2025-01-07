using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [SerializeField] Switchable[] controlledObjects;
    Switch[] connectedSwitches;

    [SerializeField] bool initialSwitchState = true;

    // variables for handling timer versions of the switches (CANNOT be active while activateOnMultipleSwitches is)
    [SerializeField] bool isTimed = false;
    [SerializeField] float activeTime = 0;

    // variables for handling multiple switches needing activation before activating the object (CANNOT be active while isTimed id)
    [SerializeField] bool activateOnMultipleSwitches = false;
    int numSwitchesToActivate = 0;
    int numSwitchesActive = 0;

    float timer = 0;

    private void Start()
    {
        connectedSwitches = GetComponentsInChildren<Switch>();
        numSwitchesToActivate = connectedSwitches.Length;

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

    public void Swap(Switch switchToActivate)
    {
        if (activateOnMultipleSwitches)
        {
            if (numSwitchesActive < numSwitchesToActivate)
            {
                if (switchToActivate.state == initialSwitchState)
                {
                    switchToActivate.SwapStatePassive();
                    numSwitchesActive += 1;

                    if (numSwitchesActive == numSwitchesToActivate)
                    {
                        foreach (Switchable controllableObject in controlledObjects)
                        {
                            controllableObject.SwitchState();
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
        else if (!isTimed)
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
