using System;
using Unity.VisualScripting;
using UnityEngine;
using static Blackboard;

[System.Serializable]
public class Blackboard
{
    public enum EnemyStates
    {
        IDLE, HOSTILE
    }

    public enum ObjectOptions
    {
        objectA, objectB
    }

    public EnemyStates currentState;
    public float attackRange;
    public float alertRange;
    public string objectAName;
    public string objectBName;
    public bool dashAvailable;

    private GameObject currentRunner;
    private GameObject objectA;
    private GameObject objectB;

    public GameObject getCurrentRunner()
    {
        return currentRunner;
    }
    public void setCurrentRunner(GameObject newRunner)
    {
        currentRunner = newRunner;
    }

    public GameObject getObject(ObjectOptions objectToGet)
    {
        switch (objectToGet)
        {
            case ObjectOptions.objectA:
                return objectA;
            case ObjectOptions.objectB:
                return objectB;
            default:
                return objectA;
        }
    }

    public void findObjectReferences()
    {
        objectA = GameObject.Find(objectAName);
        objectB = GameObject.Find(objectBName);
    }
}
