using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;

public class MeshChildrenAlpha : MonoBehaviour
{
    public float alpha;
    public List<MeshRenderer> meshies = new List<MeshRenderer>();
    public List<float> basealphas = new List<float>();

    void Start()
    {
        for (int i = 0; i < meshies.Count; i++)
        {
            basealphas.Add(meshies[i].material.color.a);
        }
    }

    public void SetAlpha(float a)
    {
        alpha = a;
        for (int i = 0; i < meshies.Count; i++)
        {
            var color = meshies[i].material.color;
            color.a = math.remap(0, 1, 0, basealphas[i], alpha);
            meshies[i].material.color = color;
        }
    }

}
