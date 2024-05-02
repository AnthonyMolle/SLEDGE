using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schema;

public class ChasePlayer : Action {
	class ChasePlayerMemory {
		//Per-node memory goes here
	}
	
	public override NodeStatus Tick(object nodeMemory, SchemaAgent agent) {
		ChasePlayerMemory memory = (ChasePlayerMemory)nodeMemory;

		Debug.Log("Test");

		return NodeStatus.Success;
	}
}
