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

	private SpriteRenderer spriteRenderer;

	public bool isGoast = false;
	public bool GoastAfterDie = false;

	private HashSet<GameObject> BeAttackBullet = new HashSet<GameObject>();

	//初始化 应该写完了 TODO
	public void Init(float h, Move m) { maxHealth = health = h; move = m; }

	//敌人本体撞到玩家 需要考虑 玩家是否无敌 （玩家在无敌状态需要被弹开 或者 对玩家造成伤害自己死亡） TODO
	public void AttackPlayer()
	{

		if (type == Type.Cut)
		{
			Vector2 v = m_rb.velocity;
			var cast = Physics2D.Raycast(transform.position, v, 100, 1 << 8);
			Debug.DrawLine(transform.position, (Vector2)transform.position+v,Color.red,10);

			if (cast)
			{
				PlayerManager.Instance.BeingCut(gameObject, cast.point, v);
			}
			else
			{
				if (PlayerManager.Instance.protect)
				{
					move.AddForceSpeed((transform.position - PlayerManager.Instance.transform.position).normalized * HitForce * 5, 0, ForceDecline);
					return;
				}
				move.AddForceSpeed((transform.position - PlayerManager.Instance.transform.position).normalized * HitForce * 2, 0, ForceDecline);
				PlayerManager.Instance.BeingAttack(damage);
			}
		}
		else
		{
			if (PlayerManager.Instance.protect)
			{
				move.AddForceSpeed((transform.position - PlayerManager.Instance.transform.position).normalized * HitForce * 5, 0, ForceDecline);
				return;
			}
			move.AddForceSpeed((transform.position - PlayerManager.Instance.transform.position).normalized * HitForce * 2, 0, ForceDecline);
			PlayerManager.Instance.BeingAttack(damage);
		}

		if (AttackDie)
		{
			GoDie();
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


	public void GoDie()
	{
		if (GoastAfterDie && !isGoast)
		{
			health = maxHealth;
			BecomeGoast();
		}
		else
		{
			health = -1;
			m_collider.enabled = false;
			AudioManager.Instance.PlaySound("EnemyDie");
			Statics.AnimatorPlay(this, animator, Statics.AnimatorType.Die);
			Destroy(gameObject, dietime);
		}
	}

	//被玩家子弹攻击 受到伤害血量计算 特效 音效等 TODO
	public void BeingAttack(GameObject bullet)
	{
		if (BeAttackBullet.Contains(bullet)) return;
		BeAttackBullet.Add(bullet);
		if (!Active)
		{
			AudioManager.Instance.PlaySound("HitWall");
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
			GoDie();
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

		if ((collision.gameObject.tag == "PlayerBullet" && !isGoast) && health > 0)
		{
			BeingAttack(collision.gameObject);
		}
		if ((collision.gameObject.tag == "PlayerCut") && health > 0)
		{
			BeingAttack(collision.transform.parent.parent.gameObject);
		}
	}

	public IEnumerator GoastFlash()
	{
		Color st = new Color(1, 1, 1, 0.5f);
		Color ed = new Color(1, 1, 1, 0.1f);
		while (true)
		{
			StartCoroutine(Statics.Flash(spriteRenderer, st, ed, 1.5f));
			yield return new WaitForSeconds(1.5f);
			StartCoroutine(Statics.Flash(spriteRenderer, ed, st, 1.5f));
			yield return new WaitForSeconds(1.5f);
		}
	}

	public IEnumerator GoastSound()
	{
		while (true)
		{
			if (Statics.InScreen(transform.position)) AudioManager.Instance.PlaySound("BecomeGoast");
			yield return new WaitForSeconds(Random.Range(6f,9f));
		}
	}

	public void BecomeGoast()
	{
		if (!isGoast)
		{
			if (Statics.InScreen(transform.position)) AudioManager.Instance.PlaySound("BecomeGoast");
			isGoast = true;
			StartCoroutine(GoastFlash());
			//StartCoroutine(GoastSound());
		}
	}

	private void Start()
	{
		animator = GetComponentInChildren<Animator>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animator.SetInteger("State", 0);
		m_rb = GetComponent<Rigidbody2D>();
		m_collider = GetComponent<Collider2D>();
		move = GetComponent<Move>();
		health = maxHealth;
		if (isGoast) { isGoast = false; BecomeGoast(); }
		//Active = false;
	}

	private void Update()
	{
	}

}
