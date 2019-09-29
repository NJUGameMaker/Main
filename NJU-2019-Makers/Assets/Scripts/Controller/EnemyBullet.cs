using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//不具有移动功能 移动功能另加Move组件
public class EnemyBullet : MonoBehaviour
{
	//伤害值
	private float damage;
	//是否始终跟随
	private bool isStatic;
	//移动类型
	public Move move { get; private set; }



	//初始化 应该写完了 TODO
	public void Init(float d, bool s,Move m) { damage = d; isStatic = s; move = m; }

	//攻击到玩家 应该写完了 还有特效 音效 TODO
	public void Attack()
	{
		PlayerManager.Instance.BeingAttack(gameObject);
		Destroy(gameObject);
	}

	//攻击到玩家核心 应该写完了 还有特效 音效 TODO
	public void AttackHeart()
	{
		Debug.Log("attack");

		PlayerManager.Instance.AttackHeart(gameObject);
		Destroy(gameObject);
	}

	//碰到墙 应该写完了 还有特效 音效 TODO
	public void Wall()
	{
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log(collision.gameObject.tag);
		if (collision.gameObject.tag == "PlayerHeart")
		{
			AttackHeart();
		}
		if (collision.gameObject.tag == "PlayerEdge")
		{
			Attack();
		}
		else if (collision.gameObject.tag == "Wall")
		{
			Wall();
		}
	}


	private void Start()
	{
	}

	void Update()
	{
		//暂停
		if (GameManager.Instance.pause)
		{
			return;
		}
		//播放剧情
		if (GameManager.Instance.playVideo)
		{
			return;
		}
	}
}
