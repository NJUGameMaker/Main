using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoChiAI : EnemyAI
{
	public GameObject Triangle;
	public Transform[] pos;
	public List<GameObject> Enemys = new List<GameObject>();
	public Transform Tri_Parent = null;

	public override void AfterDie()
	{
		foreach (var item in Enemys)
		{
			var ai = item.GetComponent<EnemyAI>();
			item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

			ai.StartCoroutine(Statics.WorkAfterSeconds(() => {
				ai.StartCoroutine(ai.StartActive());
				//ai.move.enabled = true;
				//ai.goAround.enabled = false;
				//ai.enemy.Active = true;
				ai.move.AddForceSpeed((item.transform.position - PlayerManager.Instance.transform.position).normalized * 30,0,0.9f);
			}, 0.5f));
		}
		if (Tri_Parent) Destroy(transform.parent.gameObject, 2);
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
		Triangle.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
		Triangle.SetActive(true);
		foreach (var item in pos)
		{
			if (!Tri_Parent) Enemys.Add(Instantiate(Triangle, item.position, item.rotation,transform.parent));
			else Enemys.Add(Instantiate(Triangle, item.position, item.rotation, Tri_Parent));
		}
		Triangle.SetActive(false);
	}
}
