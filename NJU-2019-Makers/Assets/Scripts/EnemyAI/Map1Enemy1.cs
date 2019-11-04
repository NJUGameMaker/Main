using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1Enemy1 : EnemyAI
{
	public override void AfterDie()
	{
		//rigidbody2.velocity = Vector2.zero;
		move.speed = move.acc = 0;
		//Debug.Log("123123");
	}

	public override void SetMove(Move m)
	{
		m.moveType = Move.MoveType.Line;
		m.direction = PlayerManager.Instance.transform.position - transform.position;
		m.speed = 2;
		m.acc = 0.1f;
		transform.rotation = Statics.FaceTo(m.direction,-90);
		//m.SetLineType(transform.position, PlayerManager.Instance.transform.position - transform.position, 3, 0.5f, rigidbody);
		//Debug.Log("233233");
	}
}
