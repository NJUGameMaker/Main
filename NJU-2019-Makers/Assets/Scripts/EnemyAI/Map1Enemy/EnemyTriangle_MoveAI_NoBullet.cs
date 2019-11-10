using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriangle_MoveAI_NoBullet : EnemyAI
{
	public override void AfterDie()
	{
		return;
	}

	public override void BeActive()
	{
		goAround.enabled = false;
		move.enabled = true;
		move.follow = PlayerManager.Instance.gameObject;
		enemy.Active = true;
		return;
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
