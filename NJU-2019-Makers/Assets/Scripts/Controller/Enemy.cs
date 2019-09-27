using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//作为敌人本体出现
//不具有移动功能 移动功能另加Move组件
public class Enemy : MonoBehaviour
{

	//血量
	private float maxHealth;
	private float health;
	//移动类型
	public Move move { get; private set; }


	//初始化 TODO
	public void Init(float h, Move m) { maxHealth = health = h; move = m; }

	//攻击到玩家 需要考虑 玩家是否无敌  TODO
	public void Attack()
	{
	}

	//攻击到玩家核心 TODO
	public void AttackHeart()
	{
	}

	//被玩家子弹攻击 TODO
	public void BeingAttack(GameObject bullet)
	{
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "PlayerHeart")
		{
			AttackHeart();
		}
		if (collision.gameObject.tag == "PlayerEdge")
		{
			Attack();
		}
		if (collision.gameObject.tag == "PlayerBullet")
		{
			BeingAttack(collision.gameObject);
		}

	}

}
