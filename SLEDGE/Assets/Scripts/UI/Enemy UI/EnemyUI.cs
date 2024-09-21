using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    public GameObject healthbar;
    public SpriteRenderer hbSprite;
    private float healthBarWidth = 0;
    public GameObject hbPivot;
    public GameObject healthbarFlash;
    public SpriteRenderer hbfSprite;
    private float healthFlashWidth = 0;
    public GameObject hbfPivot;
    public GameObject healthbarBack;
    [Range(0f, 1f)]
    public float health = 1f;
    public float currenthealth;
    public float maxhealth = 1f;
    [Range(0f, 1f)]
    public float damage = 0f;
    public Camera camera;
    public float rotationSpeed = 1.0f;
    [Range(0f, 10f)]
    public float scalar;
    [Range(0f, 0.2f)]
    public float klamb;
    public bool lerping;
    public Coroutine flashlerp;
    // Start is called before the first frame update
    void Start()
    {
        // damage = 0f;
        healthbarFlash.SetActive(false);
        healthBarWidth = hbSprite.size.x * healthbar.transform.localScale.x;
        healthFlashWidth = hbfSprite.size.x * healthbarFlash.transform.localScale.x;
        updateHealth(currenthealth);
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

    public void updateHealth(float newHealth)
    {
        float hurt = currenthealth - newHealth;
        currenthealth = newHealth;
        health = getPercentage(currenthealth, maxhealth);
        Debug.Log($"health: max: {maxhealth}, current: {currenthealth}, ratio: {health}");
        if (hurt > 0)
        {
            damage += getPercentage(hurt, maxhealth);
            healthbarFlash.SetActive(true);
            if (flashlerp != null) {
                StopCoroutine(flashlerp);
            }
            flashlerp = StartCoroutine(FlashLerp(0.35f));
        }
        // set hbfPivot to hbPivot + bounding box x width
        hbfPivot.transform.localPosition = new Vector3(hbPivot.transform.localPosition.x - healthBarWidth * health,
        hbPivot.transform.localPosition.y, hbPivot.transform.localPosition.z);
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
        var pct = at/max;
        return pct;
    }

    private IEnumerator FlashLerp(float time)
    {
        yield return new WaitForSeconds(time);
        while (damage > 0)
        {
            damage -= 0.01f;
            setBarScale("flash");
            yield return new WaitForSeconds(0.01f);
        }
        damage = 0;
        healthbarFlash.SetActive(false);
        lerping = false;
        flashlerp = null;
    }

    public void SetHealth(float maxHealth, float currentHealth)
    {
        maxhealth = maxHealth;
        currenthealth = currentHealth;
        health = getPercentage(currenthealth, maxhealth);
    }
}
