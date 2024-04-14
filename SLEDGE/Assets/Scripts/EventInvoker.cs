using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventInvoker : MonoBehaviour
{
    [SerializeField]
    UnityEvent unityEvent;

    public void ActivateEvent()
    {
        unityEvent.Invoke();
    }
}
