using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerHitbox : MonoBehaviour
{
    public BoxCollider hitbox;
    public List<Collider> hitObjects; 

    void Start()
    {
        hitbox = GetComponent<BoxCollider>();
    }

    public void InitializeCollider(float sizeX, float sizeY, float sizeZ)
    {
        hitbox.size = new Vector3(sizeX, sizeY, sizeZ);
        hitbox.center = new Vector3(0, 0, sizeZ/2);
    }

    public void ActivateCollider()
    {
        hitbox.enabled = true;
    }

    public void DeactivateCollider()
    {
        hitbox.enabled = false;
    }


    void OnTriggerStay(Collider collider)
    {
        if (!hitObjects.Contains(collider))
        {
            hitObjects.Add(collider);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (hitObjects.Contains(collider))
        {
            hitObjects.Remove(collider);
        }
    }
}
