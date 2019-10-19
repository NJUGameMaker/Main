using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//作为敌人本体出现
//不具有移动功能 移动功能另加Move组件
public class Enemy : MonoBehaviour
{
	//动画组件
	public Animator animator;

	//被攻击闪烁时间
	const float atktime = 0.5f;
	//死亡动画时间
	const float dietime = 2f;

	//血量
	private float maxHealth;
	public float health { get; private set; }
	//移动类型
	public Move move { get; private set; }


	//初始化 应该写完了 TODO
	public void Init(float h, Move m) { maxHealth = health = h; move = m; }

	//敌人本体撞到玩家 需要考虑 玩家是否无敌 （玩家在无敌状态需要被弹开 或者 对玩家造成伤害自己死亡） TODO
	public void Attack()
	{
	}

	//攻击到玩家核心 判断游戏结束或者读取存档等 TODO
	public void AttackHeart()
	{
	}

	//被玩家子弹攻击 受到伤害血量计算 特效 音效等 TODO
	public void BeingAttack(GameObject bullet)
	{
		EffectManager.Instance.CameraShake(0.1f, 0.1f);
		//if (health < 0) return;
        health -= bullet.GetComponent<PlayerBullet>().damage;
        if(health < 0)
        {
			animator.SetInteger("State", 2);
			Destroy(gameObject, dietime);
		}
		else
        {
            Vector3 shootin = bullet.GetComponent<Rigidbody2D>().velocity;
            shootin.z = 0;
            GetComponent<Move>().AddForceSpeed(shootin * 0.5f);
			if (animator.GetInteger("State") == 0)
				StartCoroutine(Statics.WorkAfterSeconds(() => { if (animator.GetInteger("State") == 1) animator.SetInteger("State", 0); }, atktime));
			animator.SetInteger("State", 1);
        }
	}

	private void OnTriggerEnter2D(Collider2D collision)
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

	private void Start()
	{
		animator = GetComponent<Animator>();
		animator.SetInteger("State", 0);
	}

	private void Update()
	{

	}

}
