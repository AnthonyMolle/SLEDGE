using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerupGiver : MonoBehaviour
{
    public string powerup;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            System.Type scriptType = System.Type.GetType(powerup + ",Assembly-CSharp");
            other.gameObject.AddComponent(scriptType);
        }
    }
}
