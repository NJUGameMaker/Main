using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWall : MonoBehaviour
{
	public GameObject LevelEnemy;

	private void FixedUpdate()
	{
		if (LevelEnemy.transform.childCount == 0)
		{
			Destroy(gameObject);
		}
	}
}
