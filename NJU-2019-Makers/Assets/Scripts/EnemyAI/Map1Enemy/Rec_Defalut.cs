using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rec_Defalut : EnemyAI
{
	public bool DelGoAround = true;
	public GameObject[] DieActive;

	public override void AfterDie()
	{
		ActiveList(false);
		EffectManager.Instance.PlayEffect(EffectManager.EffectType.ActiveBig, transform.position, Quaternion.identity);
		foreach (var item in DieActive)
		{
			Debug.Log(item);
			var tmp = Instantiate(item, transform.position, transform.rotation);
			tmp.SetActive(true);
			Destroy(tmp, 3);
		}
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
