using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Blackboard
{
    public enum EnemyStates
    {
        IDLE, HOSTILE
    }

    public EnemyStates currentState;
    public GameObject player;
    public float attackRange;
    public float alertRange;
}
