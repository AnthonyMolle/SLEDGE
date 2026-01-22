using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class HitIndicatorTexture : MonoBehaviour
{
    public int tex = 0;
    public List<Texture2D> textures = new List<Texture2D>();
    public Renderer rendy;
    public Vector2 scrollSpeed = new Vector2(10f, 0f);

    public void CycleTexture()
    {
        tex++;
        if (tex >= textures.Count)
        {
            tex = 0;
        }
        rendy.material.SetTexture("_BaseMap", textures[tex]);
    }

    void Update()
    {
        Vector2 offset = rendy.material.mainTextureOffset;
        offset += scrollSpeed * Time.deltaTime;

        rendy.material.mainTextureOffset = offset;
    }

}
