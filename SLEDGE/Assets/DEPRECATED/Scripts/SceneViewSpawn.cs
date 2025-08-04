using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneViewSpawn : MonoBehaviour
{
    public enum SpawnMode { Default, SceneCamera}

    public SpawnMode spawnMode = SpawnMode.Default;

    [SerializeField]
    Transform player;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log(SceneView.lastActiveSceneView.camera.transform.position);
        if (spawnMode == SpawnMode.SceneCamera)
        {
            player.position = SceneView.lastActiveSceneView.camera.transform.position;
        }
    }
}
