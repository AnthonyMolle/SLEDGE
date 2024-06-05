using UnityEngine;

[System.Serializable]
public class Blackboard
{
    public enum EnemyStates
    {
        IDLE, HOSTILE
    }

    public EnemyStates currentState;
    public float attackRange;
    public float alertRange;
    public string objectAName;

    private GameObject currentRunner;
    private GameObject objectA;

    public GameObject getCurrentRunner()
    {
        return currentRunner;
    }
    public void setCurrentRunner(GameObject newRunner)
    {
        currentRunner = newRunner;
    }

    public GameObject getObjectA()
    {
        return objectA;
    }

    public void findObjectA()
    {
        objectA = GameObject.Find(objectAName);
        if (objectA == null)
        {
            Debug.LogWarning("Behavior Tree Blackboard requires " + objectAName + " ot be in scene. Not in scene.");
        }
    }
}
