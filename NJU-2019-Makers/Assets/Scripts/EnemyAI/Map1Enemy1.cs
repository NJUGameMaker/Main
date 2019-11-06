using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1Enemy1 : EnemyAI
{

	public bool follow;

	public override void AfterDie()
	{
		//rigidbody2.velocity = Vector2.zero;
		move.speed = move.acc = 0;
		//Debug.Log("123123");
	}

	public override void BeActive()
	{
		//开始攻击特效 TODO
		EffectManager.Instance.PlayEffect(EffectManager.EffectType.PlayerTanOut, transform.position, Quaternion.identity);
		move.enabled = true;
		goAround.enabled = false;
		enemy.Active = true;

		move.moveType = Move.MoveType.Line;
		if (follow) move.direction = PlayerManager.Instance.transform.position - transform.position;
		transform.rotation = Statics.FaceTo(move.direction, -90);
	}

	public override void EndOfRound()
	{
		return;
	}

	public override void Init()
	{
		return;
	}
}
