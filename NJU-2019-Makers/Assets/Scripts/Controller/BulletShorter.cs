using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShorter : MonoBehaviour
{
	public enum ShotType
	{
		Static,
		Follow,
		AIFollow,
	}
	//发射状态
	public ShotType shotType;
	//子弹prefab
	private GameObject Bullet;
	public GameObject CutBullet;
	public GameObject DamageBullet;
	//跟踪目标
	public GameObject Target;
	//单轮攻击间隔
	public float Interval;
	//射击初始角度
	public int StartRotate;
	//旋转射击角度间隔
	public int RotateStep;
	//旋转射击子弹间隔
	public float RotateInterval;
	//初始速度
	public float Velocity;
	//加速度
	public float Acceleration;
	//伤害
	public float Damage;
	//发射特效
	public EffectManager.EffectType OutType;
	//击中特效
	public EffectManager.EffectType HitType;
	//子弹类型
	public EnemyBullet.Type bulletType;
	//射击点
	public Transform ShotPoint;
	//移动
	private Move move;
	//刚体
	private Rigidbody2D rigidbody2;

    // Start is called before the first frame update
    void Start()
    {
		switch (bulletType)
		{
			case EnemyBullet.Type.EnemyBullet:
				Bullet = DamageBullet;
				break;
			case EnemyBullet.Type.EnemyCut:
				Bullet = CutBullet;
				break;
			default:
				break;
		}
		rigidbody2 = GetComponent<Rigidbody2D>();
		StartCoroutine(Shot());
	}

	IEnumerator Shot()
	{
		while (true)
		{

			switch (shotType)
			{
				case ShotType.Static:
					for (int r = 0; r < 360; r += RotateStep)
					{
						//Debug.Log(Quaternion.Euler(0, 0, r + StartRotate).eulerAngles);
						float rot = r + StartRotate;
						transform.rotation = Quaternion.Euler(0, 0, rot);
						GameObject bullet = Instantiate(Bullet, transform.position, transform.rotation);
						bullet.SetActive(true);
						bullet.AddComponent<EnemyBullet>().Init(
							Damage, false,
							bullet.AddComponent<Move>().SetLineType(
								transform.position,
								new Vector2(Mathf.Cos(rot/180 *Mathf.PI),Mathf.Sin(rot / 180 * Mathf.PI)),
								Velocity, Acceleration, bullet.GetComponent<Rigidbody2D>()
							),
							bulletType, HitType
						);
						EffectManager.Instance.PlayEffect(OutType, ShotPoint.position, transform.rotation);
						while (GameManager.Instance.pause) yield return new WaitForEndOfFrame();
						yield return new WaitForSeconds(RotateInterval);
					}
					break;
				case ShotType.Follow:
					{
						transform.rotation.SetLookRotation(Target.transform.position - ShotPoint.position);
						GameObject bullet = Instantiate(Bullet, transform.position, transform.rotation);
						bullet.SetActive(true);
						bullet.AddComponent<EnemyBullet>().Init(
							Damage, false,
							bullet.AddComponent<Move>().SetLineType(
								transform.position,
								Target.transform.position - ShotPoint.position,
								Velocity, Acceleration, bullet.GetComponent<Rigidbody2D>()
							),
							bulletType, HitType
						);
						EffectManager.Instance.PlayEffect(OutType, ShotPoint.position, transform.rotation);
					}
					break;
				case ShotType.AIFollow:
					{
						transform.rotation.SetLookRotation(Target.transform.position - ShotPoint.position);
						GameObject bullet = Instantiate(Bullet, transform.position, Quaternion.Euler(0, 0, 0));
						bullet.SetActive(true);
						bullet.AddComponent<EnemyBullet>().Init(
							Damage, false,
							bullet.AddComponent<Move>().SetAIFollowType(
								Target, Velocity, bullet.GetComponent<Rigidbody2D>()
							),
							bulletType, HitType
						);
						EffectManager.Instance.PlayEffect(OutType, ShotPoint.position, transform.rotation);
					}
					break;
				default:
					break;
			}
			while (GameManager.Instance.pause) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(Interval);
		}
	}

    // Update is called once per frame
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
