using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ExplosionVFX : MonoBehaviour
{
    VisualEffect explosionEffect;
    float deathTimer;

    // Start is called before the first frame update
    void Start()
    {
        explosionEffect = GetComponent<VisualEffect>();

        explosionEffect.Play();
    }

    // Update is called once per frame
    void Update()
    {
        deathTimer += Time.deltaTime;

        if (deathTimer > 3f)
        {
            Destroy(gameObject);
        }
    }
}
