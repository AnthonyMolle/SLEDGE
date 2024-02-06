
// Simple script to bob a transform up and down!
using System.Collections;
using UnityEngine;

public class Y_Bobbing : MonoBehaviour
{

    public float duration_of_bob;
    public float y_offset_from_origin;

    public AnimationCurve animCurve;

    void Start()
    {
        float y_origin = transform.position.y;

        StartCoroutine(LerpValue(y_origin, y_origin + y_offset_from_origin));
    }

    IEnumerator LerpValue(float start, float end)
    {
        float timeElapsed = 0;
        float y;

        while(timeElapsed < duration_of_bob)
        {
            float t = timeElapsed / duration_of_bob;

            t = animCurve.Evaluate(t);

            y = Mathf.Lerp(start, end, t);
            Vector3 cur_pos = transform.position;
            transform.position = new Vector3(cur_pos.x, y, cur_pos.z);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        y = end;
        StartCoroutine(LerpValue(end, start));
    }
}
