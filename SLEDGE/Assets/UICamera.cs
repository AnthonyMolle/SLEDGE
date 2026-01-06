using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    public PlayerController player;
    private Vector3 offset;
    public Vector3 maxPositionLimits;
    [Range(0f, 1f)]
    public float LerpSpeed = 0.1f;
    public float maxSpeed;
    public float maxFallingSpeed;
    public float maxStrafeSpeed;
    // maxSpeed, airMaxSpeed, airMaxHammerSpeed, maxFallingSpeed

    void LateUpdate()
    {
        float forwardVelocity = Vector3.Dot(player.rb.velocity, player.transform.forward);
        float strafeVelocity = Vector3.Dot(player.rb.velocity, player.transform.right);
        offset.z = Remap(forwardVelocity, -maxSpeed, maxSpeed, -maxPositionLimits.z, maxPositionLimits.z);
        offset.y = Remap(player.rb.velocity.y, -maxFallingSpeed, maxFallingSpeed, -maxPositionLimits.y, maxPositionLimits.y);
        offset.x = Remap(strafeVelocity, -maxStrafeSpeed, maxStrafeSpeed, -maxPositionLimits.x, maxPositionLimits.x);
        transform.localPosition = Vector3.Lerp(transform.localPosition, offset, LerpSpeed);
    }

    float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }

}
