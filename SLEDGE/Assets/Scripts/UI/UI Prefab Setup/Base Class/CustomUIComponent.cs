using NaughtyAttributes;
using UnityEngine;

public abstract class CustomUIComponent : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }

    public abstract void Setup();
    public abstract void Configure();

    [Button("Apply Changes!!")]
    public void InspectorInit()
    {
        Init();
        foreach (CustomUIComponent x in GetComponentsInChildren<CustomUIComponent>())
        {
            x.Init();
        }
    }
    public void Init()
    {
        Setup();
        Configure();
    }
    private void OnValidate()
    {
        Init();
    }
}
