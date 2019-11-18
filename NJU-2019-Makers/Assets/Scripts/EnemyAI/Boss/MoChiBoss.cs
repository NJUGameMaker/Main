using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoChiBoss : EnemyAI
{
	public GameObject Triangle;
	public Transform[] pos;
	public List<GameObject> Enemys = new List<GameObject>();
	public Transform Tri_Parent;
	public Revive[] EnemyMaker;

	public override void AfterDie()
	{
		foreach (var item in Enemys)
		{
			var ai = item.GetComponent<EnemyAI>();
			ai.GetComponent<Rigidbody2D>().simulated = true;
			ai.StartCoroutine(Statics.WorkAfterSeconds(() => {
				Vector2 toplayer = (Vector2)(item.transform.position - PlayerManager.Instance.transform.position).normalized + new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
				ai.StartCoroutine(ai.StartActive());
				ai.move.SetLineType(ai.transform.position, toplayer, 0, 0, 0, ai.rigidbody2);
				ai.move.AddForceSpeed(toplayer * 30, 0, 0.9f);
			}, 0.2f));
			ai.StartCoroutine(Statics.WorkAfterSeconds(() =>
			{
				Vector2 toplayer = (Vector2)(item.transform.position - PlayerManager.Instance.transform.position).normalized + new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
				ai.move.SetLineType(ai.transform.position, -toplayer, 3, 5, 2, ai.rigidbody2);
			}, 1f));
		}
		foreach (var item in EnemyMaker)
		{
			if (item) item.newObj();
		}
		Destroy(transform.parent.gameObject, 1.2f);
		return;
	}

	public override void EndOfRound()
	{
		return;
	}

	public override void BeActive()
	{
		enemy.Active = true;
		return;
	}
	public override void Init()
	{
		//Triangle.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
		Triangle.GetComponent<Rigidbody2D>().simulated = false;
		Triangle.SetActive(true);
		foreach (var item in pos)
		{
			Enemys.Add(Instantiate(Triangle, item.position, item.rotation, Tri_Parent));
		}
		Triangle.SetActive(false);
	}
}
