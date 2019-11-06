using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
	[HideInInspector]
	public Enemy enemy;
	[HideInInspector]
	public Move move;
	[HideInInspector]
	public GoAround goAround;
	[HideInInspector]
	public Rigidbody2D rigidbody2;
	public abstract void AfterDie();
	public abstract void BeActive();
	public abstract void EndOfRound();
	public abstract void Init();

	private bool Active;
	private bool Alive;
	private bool RoundEnd;

	public float ActiveDistance;
	public Collider2D ActiveCollider;

	public void StartMove()
	{
		Active = true;
		BeActive();
	}

    // Start is called before the first frame update
    void Start()
    {
		Init();
		Alive = true;
		RoundEnd = false;
		Active = false;

		enemy = GetComponent<Enemy>();
		enemy.enabled = true;

		move = GetComponent<Move>();
		move.enabled = false;

		goAround = GetComponent<GoAround>();
		goAround.enabled = true;

		rigidbody2 = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
	void Update()
	{
		if (enemy.health <= 0 && Alive) { AfterDie(); Alive = false; }
		if (!Active &&
				(((transform.position - PlayerManager.Instance.transform.position).magnitude < ActiveDistance)
				|| (ActiveCollider && ActiveCollider.IsTouching(PlayerManager.Instance.EdgeCollider))))
			{ StartMove(); }
		if (goAround.RoundTimes == 1 && !RoundEnd) { RoundEnd = true; EndOfRound(); }
	}
}
