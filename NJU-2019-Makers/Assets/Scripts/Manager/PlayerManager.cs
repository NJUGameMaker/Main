using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public static PlayerManager Instance = null;

	//初始化单例
	private void Awake()
	{
		//初始化
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	public enum BulletType
	{
		None,
		Strong,
		Bounce
	};

	public enum SkillType
	{
		None,
		Satellite,
		Laser
	}

	//外层碰撞器
	public EdgeCollider2D EdgeCollider;
	//内层碰撞器
	public Collider2D HeartCollider;
	//外层贴图
	public SpriteRenderer EdgeSprite;
	//内层贴图
	public SpriteRenderer HeartSprite;
	//切削产生的角
	public HashSet<GameObject> Angles = new HashSet<GameObject>();
	//边界点
	public List<Vector2> Points = new List<Vector2>();
	//切削产生的点的编号
	public HashSet<int> KeyPoints = new HashSet<int>();

	//血量
	private float maxHealth = 100;
	private float health = 100;

	//能量（缩放），0->初始不放缩情况
	private float maxEnergy = 100;
	private float energy = 0;

	//子弹条
	private float maxBullet = 100;
	private float bullet = 100;


	//弹开之后的保护状态
	private bool protect;

	private BulletType bulletType;
	private SkillType skillType;

	//按键配置
	private bool LEFT => Input.GetKey(KeyCode.A);
	private bool RIGHT => Input.GetKey(KeyCode.D);
	private bool UP => Input.GetKey(KeyCode.W);
	private bool DOWN => Input.GetKey(KeyCode.S);
	private bool FIRE => Input.GetMouseButton(0);
	private bool SMALL => Input.GetMouseButton(1);
	private bool BOMB => Input.GetMouseButtonUp(1);
	//鼠标位置
	private Vector2 MOUSE => Camera.main.ScreenToWorldPoint(Input.mousePosition);
	//获取子物体
	private GameObject GOHeart => HeartCollider.gameObject;
	private GameObject GOEdge => EdgeCollider.gameObject;

	//图形界面加预设物体 TODO
	public GameObject BulletPrefab;

	//子弹类型伤害
	public const float NoneDamage = 10;
	public const float StrongDamage = 10;
	public const float BounceDamage = 5;


	//子弹误差默认值（角度值）：
	public const float deviation = 25;
	//发射帧间隔(s)：
	public const float interval = 0.2f;
	//能够发射子弹
	private bool canFire = true;


	//当攻击键按下 TODO
	public void Fire()
	{
		if (canFire)
		{
			float damage = 0;
			switch (bulletType)
			{
				case BulletType.None:
					damage = NoneDamage;
					break;
				case BulletType.Strong:
					damage = StrongDamage;
					break;
				case BulletType.Bounce:
					damage = BounceDamage;
					break;
			}
			canFire = false;
			StartCoroutine(Statics.WorkAfterSeconds(() => canFire = true, interval));
			var pos = Statics.V3toV2(transform.position);
			GameObject bullet = GameObject.Instantiate(BulletPrefab, Statics.V2toV3(pos), Quaternion.identity) as GameObject;
			PlayerBullet playerBullet = bullet.AddComponent<PlayerBullet>();
			playerBullet.Init(bulletType, damage, false, bullet.AddComponent<Move>());
			float angle = Mathf.Atan2((MOUSE - pos).y, (MOUSE - pos).x) * Mathf.Rad2Deg + Random.Range(-deviation, deviation) * (1 - energy / maxEnergy);
			Vector2 direct = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
			playerBullet.move.SetLineType(pos, direct, 10);
		}
	}

	//当缩小键按下 改变外壳大小 改变内核大小 改变enegy数值 音效 特效 TODO
	public void BeingSmall()
	{

	}

	//当放开缩小键 恢复外壳大小 恢复内核大小 设置短时间无敌 改变enegy数值（可能要设置缩放CD） 音效 特效
	//考虑技能释放 考虑角的攻击 TODO
	public void Bomb()
	{

	}

    //表示移动的速度
    public float speed = 50;
    //移动 TODO
    public void Move()
	{
        
        float inputX = Input.GetAxis("Horizontal");//横向上的移动
        float inputY = Input.GetAxis("Vertical");//竖直线上的移动
        Vector3 v = new Vector3(inputX, inputY, 0); //新建移动向量
        v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
        v = v * speed * Time.deltaTime;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
        transform.Translate(v);                       //移动
        if (LEFT)
		{

		}

		if (RIGHT)
		{

		}

		else if (UP)
		{

		}

		else if (DOWN)
		{

		}
	}

	//内核被攻击 死亡 或者 读取存档点等 TODO
	public void AttackHeart(GameObject other)
	{

	}

	//受到攻击 减少生命 减少外壳大小 音效 特效 TODO
	public void BeingAttack(GameObject other)
	{

	}

	//受到切削 改变外壳形状 新增角的攻击点 维护Mask （需要判断是否切到核心） 音效 特效 TODO
	public void BeingCut(GameObject other)
	{

	}

	//回血 TODO
	public void ReHealth(float x)
	{

	}

	public void SetBullet(BulletType bullet)
	{
		bulletType = bullet;
	}

	public void SetSkill(SkillType skill)
	{
		skillType = skill;
	}

	// 初始化 TODO
	void Start()
    {
        
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

		if (FIRE)
		{
			Fire();
		}

		if (SMALL)
		{
			BeingSmall();
		}

		if (BOMB)
		{
			Bomb();
		}
        
        Move();
	}
}
