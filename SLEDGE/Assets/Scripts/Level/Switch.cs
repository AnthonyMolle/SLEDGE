using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool state = false;
    [SerializeField] MeshRenderer mr;
    [SerializeField] Material trueMat;
    [SerializeField] Material falseMat;

    SwitchController sc;

    private void Start()
    {
        sc = GetComponentInParent<SwitchController>();
    }

    public void SwapStateActive() //swaps the state of the switch itself, the objects it controls, and the other switches it shares state with
    {
        sc.Swap(this);
    }

    public void SwapStatePassive() //swaps the state of only the switch itself, used for updating connected switches to the correct state
    {
        if (state)
        {
            state = false;
            mr.material = falseMat;
        }
        else
        {
            state = true;
            mr.material = trueMat;
        }
    }

    public void SetState(bool incomingState)
    {
        if (state == incomingState)
        {
            SwapStatePassive();
            SwapStatePassive();
        }
        else
        {
            SwapStatePassive();
        }
    }

}
