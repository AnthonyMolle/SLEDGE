using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    [Header("Basics")]
    private Rigidbody rb;
    private Material baseMaterial;
    private Renderer renderer;
    public Material hitGlow;
    public float flashTime = 0.1f;
    // Part things:
    [Header("Model")]
    public int health = 10;                 // total health
    public int damaged_health = 5;          // health to swap to damaged part
    public GameObject damagedPart;      // part to swap to when part is damaged
    public List<GameObject> chunks;     // chunks to spawn when part shatters
    public Quaternion ogrotation;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        baseMaterial = renderer.material;
        ogrotation = transform.localRotation;
    }

    public void TakeDamage(int damage, Vector3 force)
    {
        health -= damage;
        if (health <= 0)
        {
            Shatter(force);
        }
        else if (health <= damaged_health)
        {
            SwapParts(force);
        }
        // StartCoroutine(Shake(force));
        StartCoroutine(FlashWhite());
    }

    public void SwapParts(Vector3 force)
    {
        if (damagedPart == null) {
            return;
        }
        else 
        {
            var brokenPiece = Instantiate(damagedPart, transform.position, Quaternion.identity);
            var newPart = brokenPiece.GetComponent<Part>();
            newPart.health = health;
            // newPart.StartCoroutine(Shake(force));
            newPart.StartCoroutine(FlashWhite());
            Destroy(gameObject);
        }
        return;
    }

    public void Shatter(Vector3 force)
    {
        Debug.Log("Shatter");
        foreach (GameObject chunk in chunks)
        {
            var newPosition = new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), 
            transform.position.y + Random.Range(-2.0f, 2.0f), transform.position.z + Random.Range(-2.0f, 2.0f));
            var direction = transform.position - newPosition;
            var distance = Vector3.Distance(transform.position, newPosition);
            var normalizedDirection = direction/distance;
            var c = Instantiate(chunk, newPosition, Quaternion.identity);
            var crb = c.GetComponent<Rigidbody>();
            crb.AddForce(force, ForceMode.Impulse);
            crb.AddForce(normalizedDirection * Random.Range(1f, 10f), ForceMode.Impulse);
        }
        // Destroy(gameObject);
    }

    public IEnumerator Shake(Vector3 force)
    {
        var timeBetweenShake = 0.02f;
        var shakeTimer = 0f;
        var power = 30f;
        var currentPower = 25f;
        var buildTime = flashTime;
        // ogrotation = transform.localRotation;
        while (shakeTimer < buildTime)
        {
            shakeTimer += timeBetweenShake;
            float xRot = Random.Range(-currentPower, currentPower);
            float yRot = Random.Range(-currentPower, currentPower);
            float zRot = Random.Range(-currentPower, currentPower);

            transform.localRotation = Quaternion.Euler(xRot, yRot, zRot);            
            //hammerCameraObject.transform.localPosition = new Vector3(-xPos, -yPos, originalHammerCamPos.z);

            currentPower = power * shakeTimer / buildTime;

            yield return new WaitForSeconds(timeBetweenShake);
            // transform.Rotate(-xRot, -yRot, -zRot, Space.localRotation);
        }
        Debug.Log("yeah resetting og rotation");
        transform.localRotation = ogrotation;
        // transform.Rotate(-xRot, -yRot, -zRot, Space.Self);
    }

    public IEnumerator FlashWhite()
    {
        renderer.material = hitGlow;
        yield return new WaitForSeconds(flashTime);
        renderer.material = baseMaterial;
    }
}
