using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1Enemy1 : EnemyAI
{
	public override void AfterDie()
	{
		Debug.Log("123123");
	}

	public override void SetMove(Move m)
	{
		Debug.Log("123123");
	}
}
