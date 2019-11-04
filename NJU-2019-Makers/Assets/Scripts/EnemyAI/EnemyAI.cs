using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
	public Enemy enemy;
	public Move move;
	public GoAround goAround;
	public Rigidbody2D rigidbody2;
	public abstract void AfterDie();
	public abstract void SetMove(Move m);

	private bool Alive;

	public void StartMove()
	{
		//开始攻击特效 TODO
		EffectManager.Instance.PlayEffect(EffectManager.EffectType.PlayerTanOut, transform.position, Quaternion.identity);
		move.enabled = true;
		goAround.enabled = false;
		SetMove(move);
	}

    // Start is called before the first frame update
    void Start()
    {
		Alive = true;

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
		if (goAround.enabled && (enemy.Active || (transform.position - PlayerManager.Instance.transform.position).magnitude < goAround.Distance) ) { StartMove(); }
    }
}
