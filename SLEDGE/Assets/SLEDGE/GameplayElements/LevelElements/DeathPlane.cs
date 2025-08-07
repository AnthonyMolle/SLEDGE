using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc;
        pc = other.gameObject.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.Die();
        }
    }
}
