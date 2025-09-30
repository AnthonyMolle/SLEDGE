using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimScript : MonoBehaviour
{
    // public UnityEvent KillSwitch;
    
    [Header("Main Components")]
    [Tooltip("The animator component that controls character animations")]
    public Animator anim;
    
    [Tooltip("List of all rigidbody components that make up the character's body parts for ragdoll physics")]
    public List<Rigidbody> bodyparts = new List<Rigidbody>();
    
    [Tooltip("GameObjects to deactivate when certain conditions are met")]
    public List<GameObject> deactivate = new List<GameObject>();
    
    [Header("Ragdoll Controls")]
    [Tooltip("Toggle this to activate/deactivate ragdoll mode. Turn on for ragdoll, off to revive")]
    public bool killswitch;
    
    [Tooltip("Force required to break character joints. Higher values = stronger joints. May need to retype to set if it isn't setting")]
    public float breakForce = 1000f;
    private float lastBreakForce = 1000f;
    private Transform spawnTransform;
    
    // Store original transforms for reset
    private List<TransformData> originalTransforms = new List<TransformData>();
    
    [System.Serializable]
    private class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        
        public TransformData(Transform transform)
        {
            position = transform.localPosition;
            rotation = transform.localRotation;
        }
    }

    [Header("Hit Force System")]
    [Tooltip("The specific body part rigidbody to apply hit force to")]
    public Rigidbody bodypartToHit;
    
    [Tooltip("Break force for the specific body part being hit (0 = use general break force)")]
    public float bodypartBreakForce;
    private float lastbodypartBreakForce;
    
    [Tooltip("Amount of force to apply when hitting the body part")]
    public float hitForce = 10f;
    private float previousHitForce = 10f;
    public float LastHitForce = 0f;
    
    [Tooltip("Direction vector for applying hit force (will be normalized)")]
    public Vector3 hitDirection;
    private Vector3 previousHitDirection;
    public Vector3 LastHitDirection = Vector3.zero;
    
    [Tooltip("Amount of randomness in hit direction (0 = no randomness, 1 = fully random)")]
    [Range(0f, 1f)]
    public float randomHitDirection = 0f;
    
    [Tooltip("Amount of randomness in hit force (0 = no randomness, 1 = fully random)")]
    [Range(0f, 1f)]
    public float randomHitForce = 0f;
    
    [Tooltip("Check this to apply the hit force to the selected body part")]
    public bool applyHitForce = false;

    private void OnValidate()
    {
        spawnTransform = transform;
        if (killswitch)
        {
            Kill();
        }
        if (!killswitch)
        {
            Revive();
        }
        if (lastBreakForce != breakForce)
        {
            lastBreakForce = breakForce;
            SetBreakForce(breakForce);
        }
        if (lastbodypartBreakForce != bodypartBreakForce && bodypartToHit != null)
        {
            lastbodypartBreakForce = bodypartBreakForce;
            float force = bodypartBreakForce;
            if (bodypartBreakForce == 0f) { force = breakForce; }
            bodypartToHit.gameObject.GetComponent<CharacterJoint>().breakForce = force;
            bodypartToHit.gameObject.GetComponent<CharacterJoint>().breakTorque = force;
        }
        if (previousHitForce != hitForce)
        {
            previousHitForce = hitForce;
        }
        if (previousHitDirection != hitDirection)
        {
            hitDirection = hitDirection.normalized;
            previousHitDirection = hitDirection;
        }
        if (randomHitDirection < 0f) randomHitDirection = 0f;
        if (randomHitDirection > 1f) randomHitDirection = 1f;
        if (randomHitForce < 0f) randomHitForce = 0f;
        if (randomHitForce > 1f) randomHitForce = 1f;
    }

    void Start()
    {
        LastHitForce = 0f;
        LastHitDirection = Vector3.zero;
        // Store original transforms for reset
        StoreOriginalTransforms();
    }
    
    private void StoreOriginalTransforms()
    {
        originalTransforms.Clear();
        foreach (Rigidbody rb in bodyparts)
        {
            originalTransforms.Add(new TransformData(rb.transform));
        }
    }

    public void Kill()
    {
        Debug.Log("killed");
        anim.enabled = false;
        foreach (Rigidbody rb in bodyparts)
        {
            rb.isKinematic = false;
            // rb.AddExplosionForce(10f, transform.position, 5f);   // not a bad idea
        }

        foreach (GameObject go in deactivate)
        {
            go.SetActive(false);
        }

        if (applyHitForce && bodypartToHit != null)
        {
            if (randomHitDirection > 0f)
            {
                Vector3 newVec = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;
                hitDirection = Vector3.Lerp(hitDirection, newVec, randomHitDirection);
            }
            if (randomHitForce > 0f)
            {
                float randomFactor = Random.Range(0.5f, 2f);
                float randomForce = hitForce * randomFactor;
                hitForce = Mathf.Lerp(hitForce, randomForce, randomHitForce);
            }
            Vector3 appliedForce = hitDirection * hitForce;
            LastHitForce = hitForce;
            LastHitDirection = hitDirection;
            hitForce = previousHitForce;
            hitDirection = previousHitDirection;
            bodypartToHit.AddForce(appliedForce, ForceMode.Impulse);
        }
    }

    public void SetBreakForce(float force)
    {
        foreach (Rigidbody rb in bodyparts)
        {
            rb.gameObject.GetComponent<CharacterJoint>().breakForce = force;
            rb.gameObject.GetComponent<CharacterJoint>().breakTorque = force;
        }
        if (bodypartBreakForce != 0f)
        {
            force = bodypartBreakForce;
            bodypartToHit.gameObject.GetComponent<CharacterJoint>().breakForce = force;
            bodypartToHit.gameObject.GetComponent<CharacterJoint>().breakTorque = force;
        }
    }

    public void Revive()
    {
        Debug.Log("revived");
        
        // Reset physics first
        for (int i = 0; i < bodyparts.Count; i++)
        {
            Rigidbody rb = bodyparts[i];
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            
            // Restore original position and rotation
            if (i < originalTransforms.Count)
            {
                rb.transform.localPosition = originalTransforms[i].position;
                rb.transform.localRotation = originalTransforms[i].rotation;
            }
        }
        foreach (GameObject go in deactivate)
        {
            go.SetActive(true);
        }
        
        anim.enabled = true;
    }
}
