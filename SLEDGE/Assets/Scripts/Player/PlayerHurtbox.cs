using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    private PlayerController pc;

    private void Start()
    {
        pc = FindObjectOfType<PlayerController>();
    }

    
}
