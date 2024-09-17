using System;
using Unity.VisualScripting;
using UnityEngine;


/*public class EnemyBlackboard : Blackboard, IBlackboard
{
    public enum EnemyStates
    {
        IDLE, HOSTILE
    }

    public EnemyStates currentState;
    public bool dashAvailable;
}
public interface IBlackboard
{

}
*/
[System.Serializable]
public class Blackboard
{
    public enum EnemyStates
    {
        IDLE, HOSTILE
    }

    public EnemyStates currentState;
    public bool dashAvailable;
    private GameObject currentRunner;
    public enum ObjectOptions
    {
        objectA, objectB
    }

    public GameObject getCurrentRunner()
    {
        return currentRunner;
    }
    public void setCurrentRunner(GameObject newRunner)
    {
        currentRunner = newRunner;
    }

    private GameObject objectA;
    private GameObject objectB;

    public string objectAName;
    public string objectBName;

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
