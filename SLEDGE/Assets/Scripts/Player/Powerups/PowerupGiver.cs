using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerupGiver : MonoBehaviour
{
    [Tooltip("Write in the EXACT name of the component you want to add to the player!")]
    public string powerup;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            System.Type scriptType = System.Type.GetType(powerup + ",Assembly-CSharp");

            //Prevents the player from having two of a power-up at once
            if (player.GetComponent(scriptType))
            {
                Destroy(player.GetComponent(scriptType));
            }
            player.AddComponent(scriptType);
            
            //Tells the power-up spawner to disable this for a bit
            transform.parent.GetComponent<PowerupBase>().CollectPowerup();
        }
    }
}
