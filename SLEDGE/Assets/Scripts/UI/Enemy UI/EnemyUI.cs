using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    public GameObject healthbar;
    public SpriteRenderer hbSprite;
    public GameObject hbPivot;
    public GameObject healthbarFlash;
    public SpriteRenderer hbfSprite;
    public GameObject hbfPivot;
    public GameObject healthbarBack;
    [Range(0f, 1f)]
    public float health;
    public float currenthealth;
    public float maxhealth;
    [Range(0f, 1f)]
    public float damage;
    public Camera camera;
    public float rotationSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraDirection = camera.transform.position - transform.position;
        float singleStep = rotationSpeed * Time.deltaTime;
        Vector3 newRotation = Vector3.RotateTowards(transform.forward, cameraDirection, 5, 0);
        transform.rotation = Quaternion.LookRotation(newRotation);
        // lerp update damage scale constantly; have a boolean check to see if still lerping
//
//
    }

    void updateHealth(float newHealth)
    {
        float hurt = currenthealth - newHealth;
        currenthealth = newHealth;
        health = getPercentage(currenthealth, maxhealth);
        damage += getPercentage(hurt, maxhealth);
        // set hbfPivot to hbPivot + bounding box x width
//
//
        setBarScale("health");
        setBarScale("flash");
    }

    void setBarScale(string bar)
    {
        if (bar == "health") {
            var sc = hbPivot.transform.localScale;
            hbPivot.transform.localScale = new Vector3(health, sc.y, sc.z);
        }
        else if (bar == "flash") {
            var sf = hbfPivot.transform.localScale;
            hbfPivot.transform.localScale = new Vector3(damage, sf.y, sf.z);
        }
    }

    static float getPercentage(float at, float max)
    {
        var pct = max/at;
        return pct;
    }
}
