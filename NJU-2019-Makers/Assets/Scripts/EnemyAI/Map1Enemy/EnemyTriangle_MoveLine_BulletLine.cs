using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriangle_MoveLine_BulletLine : EnemyAI
{
	public override void AfterDie()
	{
		foreach (var item in ObjectToActive)
		{
			item.SetActive(false);
		}
		return;
	}

	public override void BeActive()
	{
		goAround.enabled = false;
		move.enabled = true;
		enemy.Active = true;
		move.direction = Statics.FaceVec(transform.rotation,90);
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
