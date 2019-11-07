﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoChiAI : EnemyAI
{
	public Transform BombCenter;
	public GameObject Triangle;
	public Transform[] pos;
	public List<GameObject> Enemys = new List<GameObject>();

	public override void AfterDie()
	{
		foreach (var item in Enemys)
		{
			var ai = item.GetComponent<EnemyAI>();
			item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

			ai.StartCoroutine(Statics.WorkAfterSeconds(() => {
				ai.move.enabled = true;
				ai.goAround.enabled = false;
				ai.enemy.Active = true;
				ai.move.AddForceSpeed((item.transform.position - BombCenter.transform.position).normalized * 30);
			}, 0.5f));
			ai.StartCoroutine(Statics.WorkAfterSeconds(() => {
				ai.BeActive();
			}, 2f));
		}
		return;
	}

	public override void EndOfRound()
	{
		return;
	}

	public override void BeActive()
	{
		return;
	}
	public override void Init()
	{
		Triangle.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
		foreach (var item in pos)
		{
			Enemys.Add(Instantiate(Triangle, item.position, item.rotation));
		}
	}
}