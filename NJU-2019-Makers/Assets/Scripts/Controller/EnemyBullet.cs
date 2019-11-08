using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//不具有移动功能 移动功能另加Move组件
public class EnemyBullet : MonoBehaviour
{
	//伤害值
	public float damage;
	//是否始终跟随
	public bool isStatic = false;
	//移动类型
	public Move move { get; private set; }
	//子弹类型
	public Type type;
	//子弹命中特效
	public EffectManager.EffectType OnEffect;
	//刚体
	private Rigidbody2D rigidbody2;


	public enum Type
	{
		EnemyBullet,
		EnemyCut,
	}

	//初始化 应该写完了 TODO
	public void Init(float d, bool s, Move m, Type t,EffectManager.EffectType effect)
	{
		damage = d;
		isStatic = s;
		move = m;
		type = t;
		OnEffect = effect;
		gameObject.tag = t.ToString();
	}

	//攻击到玩家 应该写完了 还有特效 音效 TODO
	public void Attack()
	{
		switch (type)
		{
			case Type.EnemyBullet:
				{ PlayerManager.Instance.BeingAttack(damage); }
				break;
			case Type.EnemyCut:
				{
					Vector2 v = rigidbody2.velocity;
					var cast = Physics2D.Raycast(transform.position, v, 100, 1 << 8);
					if (cast)
					{
						PlayerManager.Instance.BeingCut(gameObject, cast.point, v);
						Debug.DrawLine(Vector3.zero, cast.point, Color.red, 100);
					}
				}
				break;
			default:
				break;
		}
		EffectManager.Instance.PlayEffect(OnEffect,transform.position,transform.rotation,1f);
		if (!isStatic) Destroy(gameObject);
	}

	//攻击到玩家核心 应该写完了 还有特效 音效 TODO
	public void AttackHeart()
	{
		//Debug.Log("attack");

		PlayerManager.Instance.AttackHeart(gameObject);
		EffectManager.Instance.PlayEffect(OnEffect, transform.position, transform.rotation,1f);
		if (!isStatic) Destroy(gameObject);

	}

	//碰到墙 应该写完了 还有特效 音效 TODO
	public void Wall()
	{
		EffectManager.Instance.PlayEffect(OnEffect, transform.position, transform.rotation,1f);
		if (!isStatic) Destroy(gameObject);

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Debug.Log(collision.gameObject.tag);
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
		rigidbody2 = GetComponent<Rigidbody2D>();
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
