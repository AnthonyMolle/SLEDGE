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
    [Tooltip("Mass scale for all rigidbodies in the ragdoll")]
    public float massScale = 1f;
    private float lastMassScale = 1f;
    [Tooltip("Drag and angular drag for the rigidbodies in the ragdoll")]
    public float drag = 0.5f;
    public float angularDrag = 0.05f;
    private float lastDrag = 0.5f;
    private float lastAngularDrag = 0.05f;
    [Tooltip("The projection distance -- aka how far each joint can stretch before snapping back")]
    public float projectionDistance = 0.1f;
    private float lastProjectionDistance = 0.1f;

    [Tooltip("The spring damper for the joints (higher values = stiffer joints)")]
    public float springDamper = 0f;
    public float springForce = 0f;
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
    [Tooltip("Automatically disconnect joint before applying force. Good for high forces; doesn't apply force to other body parts so might feel less natural")]
    public bool disconnectAutomatically = false;
    
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
        // break force
        SetBreakForce(breakForce);
        // projection distance
        foreach (Rigidbody rb in bodyparts)
        {
            rb.angularDrag = angularDrag;
            rb.drag = drag;
            CharacterJoint cj = rb.GetComponent<CharacterJoint>();
            if (cj != null)
            {
                cj.projectionDistance = projectionDistance;
                // change spring damper for character joint
                cj.swingLimitSpring = new SoftJointLimitSpring { spring = springForce, damper = springDamper };
                cj.twistLimitSpring = new SoftJointLimitSpring { spring = springForce, damper = springDamper };
            }
        }
        // mass scale
        SetMassScale(massScale);

        // spring damper

        // bodypart break force
        if (bodypartToHit != null)
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

        CharacterJoint cj = bodypartToHit.GetComponent<CharacterJoint>();
        if (disconnectAutomatically && cj != null)
        {
            Destroy(cj);
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
            bodypartToHit.AddTorque(appliedForce, ForceMode.Impulse);
        }
    }

    public void SetBreakForce(float force)
    {
        foreach (Rigidbody rb in bodyparts)
        {
            CharacterJoint cj = rb.gameObject.GetComponent<CharacterJoint>();
            if (cj != null)
            {
                cj.breakForce = force;
                cj.breakTorque = force;
            }
        }
        if (bodypartBreakForce != 0f)
        {
            force = bodypartBreakForce;
            CharacterJoint cj = bodypartToHit.gameObject.GetComponent<CharacterJoint>();
            if (cj != null)
            {
                cj.breakForce = force;
                cj.breakTorque = force;
            }
        }
    }
    
    public void SetMassScale(float scale)
    {
        foreach (Rigidbody rb in bodyparts)
        {
            rb.mass = scale;
            CharacterJoint cj = rb.GetComponent<CharacterJoint>();
            if (cj != null)
            {
                cj.massScale = scale;
                cj.connectedMassScale = scale;
            }
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
