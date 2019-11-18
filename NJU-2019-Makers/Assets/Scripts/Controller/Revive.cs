using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revive : MonoBehaviour
{
	public GameObject Obj;
	public float ReviveTime;
	public bool auto = true;

	private SpriteRenderer render;
	private EnemyAI enemyAI;
	private GameObject Current;

	public void newObj()
	{
		Current = Instantiate(Obj, transform);
		Current.SetActive(true);
		enemyAI = Current.GetComponentInChildren<EnemyAI>();
		render = Current.GetComponentInChildren<SpriteRenderer>();
		if (render) StartCoroutine(Statics.Flash(render, Color.clear, Color.white, 0.5f));
		if (enemyAI) StartCoroutine(enemyAI.StartActive());
	}

	IEnumerator Loop()
	{
		while (true)
		{
			while (Current) yield return new WaitForFixedUpdate();
			yield return new WaitForSeconds(ReviveTime);
			newObj();
		}
	}

    // Start is called before the first frame update
    void Start()
    {
		if (auto) StartCoroutine(Loop());
    }

}
