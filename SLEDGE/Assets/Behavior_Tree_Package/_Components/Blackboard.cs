using NaughtyAttributes;
using UnityEngine;


[System.Serializable]
public class Blackboard
{
    /* Shared Variables */

    [Header("Shared Variables")]
    [HorizontalLine]

    public EnemyStates currentState;
    public string objectAName;
    public string objectBName;

    /* Flyer Enemy Variables */

    [Header("Flyer Variables")]
    [HorizontalLine]

    public bool dashAvailable;

    /* Shooter Enemy Variables */

    [Header("Shooter Variables")]
    [HorizontalLine]

    public float detectionRadius = 20;


    /* Enum Grave */

    public enum EnemyStates
    {
        IDLE, HOSTILE, STUNNED
    }

    public enum ObjectOptions
    {
        objectA, objectB
    }

    /* Helper Methods */

    public void findObjectReferences()
    {
        objectA = GameObject.Find(objectAName);
        objectB = GameObject.Find(objectBName);
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
    public GameObject getCurrentRunner()
    {
        return currentRunner;
    }
    public void setCurrentRunner(GameObject newRunner)
    {
        currentRunner = newRunner;
    }

    /* Hidden Data */
    private GameObject objectA;
    private GameObject objectB;
    private GameObject currentRunner;
}
