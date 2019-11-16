using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//作为敌人本体出现
//不具有移动功能 移动功能另加Move组件
public class Enemy : MonoBehaviour
{

	public enum Type
	{
		Damage,
		Cut
	}

	//动画组件
	private Animator animator;

	//自身撞击伤害
	public float damage;
	//撞击玩家后自身是否死亡
	public bool AttackDie; 
	//撞墙后是否死亡
	public bool HitWallDie;
	//被子弹击中后 击退参数
	public float HitForce = 5;
	public float ForceDecline = 0.8f;

	//被攻击闪烁时间
	const float atktime = 0.5f;
	//死亡动画时间
	const float dietime = 2f;

	//自身刚体
	private Rigidbody2D m_rb;
	//自身碰撞器
	private Collider2D m_collider;
	//是否激活（被攻击过）
	public bool Active;
	public bool AttackToActive;

	public Type type;
	//血量
	public float maxHealth;
	public float health { get; private set; }
	//移动类型
	public Move move { get; private set; }


	//初始化 应该写完了 TODO
	public void Init(float h, Move m) { maxHealth = health = h; move = m; }

	//敌人本体撞到玩家 需要考虑 玩家是否无敌 （玩家在无敌状态需要被弹开 或者 对玩家造成伤害自己死亡） TODO
	public void AttackPlayer()
	{
		if (PlayerManager.Instance.protect)
		{
			move.AddForceSpeed((transform.position - PlayerManager.Instance.transform.position).normalized * HitForce * 5,0,ForceDecline);
			return;
		}

		if (type == Type.Cut)
		{
			Vector2 v = m_rb.velocity;
			var cast = Physics2D.Raycast(transform.position, v, 100, 1 << 8);
			if (cast)
			{
				PlayerManager.Instance.BeingCut(gameObject, cast.point, v);
			}
		}
		else
		{
			PlayerManager.Instance.BeingAttack(damage);
		}

		if (AttackDie)
		{
			Statics.AnimatorPlay(this, animator, Statics.AnimatorType.Die);
			health = -1;
			m_collider.enabled = false;
			Destroy(gameObject, dietime);
		}
	}

	//攻击到玩家核心 判断游戏结束或者读取存档等 TODO
	public void AttackHeart()
	{
		if (PlayerManager.Instance.protect)
		{
			move.AddForceSpeed((transform.position - PlayerManager.Instance.transform.position).normalized * HitForce * 5, 0, ForceDecline);
			return;
		}
		PlayerManager.Instance.AttackHeart(gameObject);
	}

	//被玩家子弹攻击 受到伤害血量计算 特效 音效等 TODO
	public void BeingAttack(GameObject bullet)
	{
		if (!Active)
		{
			Active = AttackToActive;
			return;
		}
		PlayerManager.Instance.ComboAdd();
		EffectManager.Instance.CameraShake(0.2f, 0.3f);
		AudioManager.Instance.PlaySound("HitEnemy");
		//if (health < 0) return;
        health -= bullet.GetComponent<PlayerBullet>().damage;
		Vector3 shootin = bullet.GetComponent<Rigidbody2D>().velocity;
		shootin.z = 0;
		GetComponent<Move>().AddForceSpeed(shootin.normalized * HitForce, 0, ForceDecline);
		if (health <= 0)
        {
			AudioManager.Instance.PlaySound("EnemyDie");
			m_collider.enabled = false;
			Statics.AnimatorPlay(this,animator, Statics.AnimatorType.Die);
			Destroy(gameObject, dietime);
		}
		else
        {
			Statics.AnimatorPlay(this,animator, Statics.AnimatorType.Attack);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log(collision.collider.gameObject.tag);
		if (collision.collider.gameObject.tag == "PlayerHeart" && health > 0)
		{
			AttackHeart();
		}
		if (collision.collider.gameObject.tag == "PlayerEdge" && health > 0)
		{
			AttackPlayer();
		}
		if (collision.collider.gameObject.tag == "Wall" && health > 0 && HitWallDie)
		{
			Statics.AnimatorPlay(this, animator, Statics.AnimatorType.Die);
			health = -1;
			m_collider.enabled = false;
			Destroy(gameObject, dietime);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Debug.Log(collision.tag);

		if (collision.gameObject.tag == "PlayerBullet" && health > 0)
		{
			BeingAttack(collision.gameObject);
		}

	}

	private void Start()
	{
		animator = GetComponentInChildren<Animator>();
		animator.SetInteger("State", 0);
		m_rb = GetComponent<Rigidbody2D>();
		m_collider = GetComponent<Collider2D>();
		move = GetComponent<Move>();
		health = maxHealth;
		//Active = false;
	}

	private void Update()
	{
	}

}
