using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI
{

	public override void AfterDie()
	{
		ActiveList(false);
		GameManager.Instance.StartGame("TempEnd");
		return;
	}

	public override void BeActive()
	{
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

	IEnumerator updateHealth()
	{
		while (true)
		{
			UIManager.Instance.SetCurHealth(enemy.health);
			yield return new WaitForFixedUpdate();
		}
	}

	public override void Init()
	{
		UIManager.Instance.UseBossHealth(enemy.maxHealth, true);
		StartCoroutine(updateHealth());
		return;
	}

	private void FixedUpdate()
	{

	}

}
