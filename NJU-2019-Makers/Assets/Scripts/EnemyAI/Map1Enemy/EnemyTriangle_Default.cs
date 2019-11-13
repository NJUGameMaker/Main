using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriangle_Default : EnemyAI
{
    public bool DelGoAround =true;
	public override void AfterDie()
	{
		ActiveList(false);
		return;
	}

	public override void BeActive()
	{
        if (DelGoAround)
        {
            goAround.enabled = false;
            move.enabled = true;
        }
		move.follow = PlayerManager.Instance.gameObject;
		enemy.Active = true;
		return;
	}

	public void ActiveList(bool b)
	{
		foreach (var item in ObjectToActive)
		{
			item.SetActive(b);
		}
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
