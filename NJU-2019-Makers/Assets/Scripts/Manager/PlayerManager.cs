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
	};

	//外层碰撞器(改了类型)
	public PolygonCollider2D EdgeCollider;
	//内层碰撞器
	public CircleCollider2D HeartCollider;
	//外层贴图
	public SpriteRenderer EdgeSprite;
	//内层贴图
	public SpriteRenderer HeartSprite;
	//切削遮罩
	public GameObject CutMask;
	//切削产生的角
	public HashSet<GameObject> Angles = new HashSet<GameObject>();
	//边界点 (改成了动态数组和类型)
	public Vector2[] Points = null;
	//切削产生的点的编号(改成了记录点的vector)
	public HashSet<Vector2> KeyPoints = new HashSet<Vector2>();

	//血量
	private float maxHealth = 100;
	private float health;

	//能量（缩放），0->初始不放缩情况
	private float maxEnergy;
	public float energy;

	//子弹条
	private float maxBullet = 100;
	private float bullet;


	//弹开之后的保护状态
	private bool protect;
	public const float protect_time = 0.1f;

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
	//自身刚体
	private Rigidbody2D m_rb;
	//动画
	private Animator EdgeAnimator;
	private Animator HeartAnimator;
	//子弹Prefeb
	public GameObject BulletNormal;
	public GameObject BulletStrong;
	public GameObject BulletTan;


	//子弹发射位置
	public Transform FirePos;

	//图形界面加预设物体 TODO
	//public GameObject BulletPrefab;

	//子弹类型伤害 和 速度
	public const float NoneDamage = 10;
	public const float StrongDamage = 10;
	public const float BounceDamage = 5;
	public const float NoneSpeed = 10;
	public const float StrongSpeed = 10;
	public const float BounceSpeed = 15;


	//子弹误差默认值（角度值）：
	public const float deviation = 25;
	//发射帧间隔(s)：
	public const float shoot_interval = 0.2f;
	//能够发射子弹
	private bool canFire;

	// 放缩所需参数：
	public const float small_interval = 0.01f;
	public const float bounce_thresold = 0.4f;
	private float step_percent;
	public const float step_interval = 0.002f;
	public const float bounce_cd = 3;
	public const float invincible_time = 0.5f;
	private bool canBomb;
	private bool canSmall;

	// 缓慢自愈单次血量：
	public const float reBlood = 0.03f;

	//增加切削遮罩
	private void addCutMask(Vector2 vec)
	{
		var tmp = Instantiate(CutMask,GOEdge.transform);
		tmp.SetActive(true);
		tmp.transform.localPosition = vec;
		tmp.transform.localRotation = Quaternion.Euler(0, 0, -90+(Mathf.Atan2(vec.y, vec.x) / Mathf.PI * 180));// (0, 0,Mathf.Atan2(vec.y,vec.x), 1);
	}

	private void FaceToMouse()
	{
		var tmp = -(Vector2)transform.position + MOUSE;
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(tmp.y, tmp.x) / Mathf.PI * 180f);
	}

	//当攻击键按下 TODO 不对啊还有子弹条没考虑进去-->有道理哦
	//那还要留一个UI的接口
	public void Fire()
	{
		if (canFire)
		{
			float damage = 0;
			float speed = 0;
			EffectManager.EffectType effectType = EffectManager.EffectType.End;
			GameObject BulletPrefab = BulletNormal;
			switch (bulletType)
			{
				case BulletType.None:
					damage = NoneDamage;
					speed = NoneSpeed;
					effectType = EffectManager.EffectType.EnemyNormalOut;
					BulletPrefab = BulletNormal;
					break;
				case BulletType.Strong:
					damage = StrongDamage;
					speed = StrongSpeed;
					effectType = EffectManager.EffectType.PlayerStrongOut;
					BulletPrefab = BulletStrong;
					break;
				case BulletType.Bounce:
					damage = BounceDamage;
					speed = BounceSpeed;
					effectType = EffectManager.EffectType.PlayerTanOut;
					BulletPrefab = BulletTan;
					break;
				default:
					break;
			}
			canFire = false;
			StartCoroutine(Statics.WorkAfterSeconds(() => canFire = true, shoot_interval));
			var pos = Statics.V3toV2(transform.position);
			GameObject bullet = GameObject.Instantiate(BulletPrefab, Statics.V2toV3(pos), Quaternion.identity) as GameObject;
			bullet.SetActive(true);
			//bullet.GetComponent<SpriteRenderer>().sprite = sprite;
			PlayerBullet playerBullet = bullet.AddComponent<PlayerBullet>();
			playerBullet.Init(bulletType, damage, false, bullet.AddComponent<Move>());
			float angle = Mathf.Atan2((MOUSE - pos).y, (MOUSE - pos).x) * Mathf.Rad2Deg + Random.Range(-deviation, deviation) * (1 - energy / maxEnergy);
			bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
			Vector2 direct = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
			playerBullet.move.SetLineType(pos, direct, speed,bullet.GetComponent<Rigidbody2D>());
			EffectManager.Instance.PlayEffect(effectType, FirePos.position, transform.rotation, 1f);
		}
	}

	//当缩小键按下 改变外壳大小 改变内核大小 改变enegy数值 音效 特效 TODO
	public void BeingSmall()
	{
		if(canSmall && step_percent <= 1){
			canSmall = false;
			StartCoroutine(Statics.WorkAfterSeconds(() => {canSmall = true;energy = Statics.FixFun(Statics.FunType.SqrtX,0,maxEnergy,step_percent);},small_interval));
			step_percent += step_interval;
			float scale = energy / maxEnergy;
			if(scale >= bounce_thresold){
				canBomb = true;
			}
			//当形状足够小的时候把外壳设置为false 能够穿进敌人
			if (energy > 80)
			{
				GOEdge.SetActive(false);
			}

		}
	}

	//当放开缩小键 恢复外壳大小 恢复内核大小 设置短时间无敌 改变enegy数值（可能要设置缩放CD） 音效 特效
	//考虑技能释放 考虑角的攻击 TODO
	public void Bomb()
	{
		GOEdge.SetActive(true);
		if (canBomb){
			energy = 0;
			step_percent = 0;
			canBomb = false;
			StartCoroutine(Statics.WorkAfterSeconds(() => canSmall = false,small_interval));	//以免被BeingSmall的协程函数更改canSmall的值
			StartCoroutine(Statics.WorkAfterSeconds(() => canSmall = true,bounce_cd));
			protect = true;
			StartCoroutine(Statics.WorkAfterSeconds(() => protect = false,protect_time));
		}else{
			energy = 0;
			step_percent = 0;
		}
	}

    //表示移动的速度
    const float speed = 10;
    //移动 TODO
    public void Move()
	{
        
        float inputX = Input.GetAxis("Horizontal");//横向上的移动
        float inputY = Input.GetAxis("Vertical");//竖直线上的移动
        Vector3 v = new Vector3(inputX, inputY, 0); //新建移动向量
		v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
		v = v * speed;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
		//transform.Translate(v);                       //移动
		m_rb.velocity = v;
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

	//内核被攻击 死亡 或者 读取存档点等 TODO 先只考虑死亡
	public void AttackHeart(GameObject other)
	{
		health = 0;
		Statics.AnimatorPlay(this, EdgeAnimator, Statics.AnimatorType.Die);
		Statics.AnimatorPlay(this, HeartAnimator, Statics.AnimatorType.Die);
		Destroy(gameObject,5f);
	}

	//受到攻击 减少生命 减少外壳大小 音效 特效 TODO
	public void BeingAttack(float damage)
	{
        health -= damage;
		maxEnergy = health;
		Statics.AnimatorPlay(this, EdgeAnimator, Statics.AnimatorType.Attack);
		Statics.AnimatorPlay(this, HeartAnimator, Statics.AnimatorType.Attack);
		EffectManager.Instance.CameraShake(0.5f, 0.5f);
		UIManager.Instance.BloodFlash();
	}

	//受到切削 改变外壳形状 新增角的攻击点 维护Mask （需要判断是否切到核心） 音效 特效 TODO
	public void BeingCut(GameObject other)
	{

	}

	public void BeingCut(GameObject other, Vector2 point, Vector2 dir)
	{
		Debug.Log("cut");
		
		//Debug.DrawLine(Vector3.zero, new Vector3(1, 1),Color.red,100);

        //求交点
        Vector2 center = Statics.V3toV2(EdgeCollider.transform.position);
        Vector2 point1;
        point1.x = (2*Mathf.Pow(dir.x,2)*center.x + (Mathf.Pow(dir.y,2)-Mathf.Pow(dir.x,2))*point.x + 2*dir.x*dir.y*(center.y-point.y)) / (Mathf.Pow(dir.x,2) + Mathf.Pow(dir.y,2));
        point1.y = 2 * (dir.y / dir.x) * ((Mathf.Pow(dir.x,2)*(center.x-point.x) + dir.x*dir.y*(center.y-point.y)) / (Mathf.Pow(dir.x,2) + Mathf.Pow(dir.y,2))) + point.y;
////<<<<<<< HEAD
//		//添加遮罩
//		Vector2 vec = new Vector2(dir.y,dir.x);
//		float y = (dir.y / dir.x) * (center.x - point.x) + point.y;
//		if(y > center.y){
//			if(vec.y > 0){
//				vec.x = -vec.x;
//			}else{
//				vec.y = -vec.y;
//			}
//		}else{
//			if(vec.y > 0){
//				vec.y = -vec.y;
//			}else{
//				vec.x = -vec.x;
//			}
//		}
//		Debug.Log(vec);
//		Debug.DrawLine((Vector2)transform.position+vec, new Vector2(0f,0f),Color.blue,100);
//		addCutMask(vec);
//		//判断是否切到核心
//		float distance = Mathf.Abs((dir.y*center.x - dir.x*center.y + dir.x*point.y-dir.y*point.x) / (Mathf.Pow(dir.y*dir.y+dir.x*dir.x,0.5f)));
//=======
        //Debug.DrawLine(point, point1, Color.red, 100);
        //判断是否切到核心
        float distance = Mathf.Abs((dir.y*center.x - dir.x*center.y + dir.x*point.y-dir.y*point.x) / (Mathf.Pow(dir.y*dir.y+dir.x*dir.x,0.5f)));
//>>>>>>> 6909aaace8ae77b650e63399e77f21b682b71fa7
		if(distance <= HeartCollider.radius)
		{
			AttackHeart(other);
		}
        //添加遮罩
        Vector2 vec = new Vector2(dir.y, dir.x);
        vec.Normalize();
        vec = vec * distance;
        float y = (dir.y / dir.x) * (center.x - point.x) + point.y;
        if (y > center.y)
        {
            if (vec.y > 0)
            {
                vec.x = -vec.x;
            }
            else
            {
                vec.y = -vec.y;
            }
        }
        else
        {
            if (vec.y > 0)
            {
                vec.y = -vec.y;
            }
            else
            {
                vec.x = -vec.x;
            }
        }
        //Debug.DrawLine(transform.position, (Vector2)transform.position + vec, Color.blue, 100);
        addCutMask(EdgeCollider.transform.InverseTransformVector(vec));
        Debug.Log(vec.magnitude);
        Debug.Log(EdgeCollider.transform.InverseTransformVector(vec).magnitude);
        //找插入位置
        int x1 = -1,x2 = -1;
        point = EdgeCollider.transform.InverseTransformPoint(point);// 相对距离
        point1 = EdgeCollider.transform.InverseTransformPoint(point1);
		for(int i = 0; i < Points.Length; i++){
			if(Statics.IsPointCut(point,Points[i],Points[(i+1)%Points.Length])){
				x1 = i;
			}
			if(Statics.IsPointCut(point1,Points[i],Points[(i+1)%Points.Length])){
				x2 = i;
			}
		}
        //插入与删除，维护边界点集
		if(x1 > x2){
			int temp = x1;
			x1 = x2;
			x2 = temp;
			Vector2 temp_v = point;
			point = point1;
			point1 = temp_v;
		}
		if(x2 - x1 < Points.Length/2){
			Vector2[] temp_array = new Vector2[Points.Length-(x2-x1)+2];
			for(int i = 0; i <= x1; i++){
				temp_array[i] = Points[i];
			}
			temp_array[x1+1] = point;
			temp_array[x1+2] = point1;
			for(int i = 0; i < Points.Length-x2-1; i++){
				temp_array[x1+3+i] = Points[x2+1+i];
			}
			Points = temp_array;
		}else{
			Vector2[] temp_array = new Vector2[x2-x1+2];
			temp_array[0] = point;
			for(int i = 0; i < x2-x1; i++){
				temp_array[1+i] = Points[x1+1+i];
			}
			temp_array[x2-x1+1] = point1;
			Points = temp_array;
		}
        EdgeCollider.points = Points;
        //Vector2[] test = new Vector2[2] { point,point1};
        //EdgeCollider.points = test;
		//添加切削点，维护切削点集
		KeyPoints.Add(point);
		KeyPoints.Add(point1);
	}

	//回血 TODO: done.
	public void ReHealth(float x)
	{
		if((health + x) >= maxHealth){
			health = maxHealth;
			maxEnergy = health;
		}else{
			health += x;
			maxEnergy = health;
		}
	}

    //随帧检测参数设置圆形大小
    public void ReShape()
    {
        if(energy == 0)
        {
            float scale = health / maxHealth;
            GOEdge.transform.localScale = new Vector3(scale, scale, 1);
            GOHeart.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            float scale = energy / maxEnergy;
            GOEdge.transform.localScale = new Vector3(1 - scale, 1 - scale, 1);
            if(canBomb)
            {
                float heartScale = (scale - bounce_thresold) / (1 - bounce_thresold);
                GOHeart.transform.localScale = new Vector3(1 + heartScale, 1 + heartScale, 1);
            }
        }
		//Debug.Log(GOEdge.transform.localScale.ToString() +" " +energy + " "+health);
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
		health = maxHealth;
		energy = 0;
		bullet = maxBullet;
		protect = false;
		bulletType = BulletType.Strong;
		skillType = SkillType.None;
        GOEdge.transform.localScale = new Vector3(1, 1, 1);
        GOHeart.transform.localScale = new Vector3(1, 1, 1);
		canFire = true;
		step_percent = 0;
		canBomb = false;
		canSmall = true;
		maxEnergy = health;
		m_rb = GetComponent<Rigidbody2D>();
		EdgeAnimator = GOEdge.GetComponent<Animator>();
		HeartAnimator = GOHeart.GetComponent<Animator>();
		Points = EdgeCollider.points;
		//BulletNormal = Resources.Load("Sprites/normal.png") as Sprite;
		//BulletStrong = Resources.Load("Sprites/strong.png") as Sprite;
		//BulletTan = Resources.Load("Sprites/tantan.png") as Sprite;
		//Debug.Log(BulletNormal);

    }

    // Update is called once per frame
	// 添加了随每帧缓慢恢复血量、改变大小
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
		ReHealth(reBlood);
        ReShape();
		FaceToMouse();
		//Debug.Log(EdgeCollider.points[0]);
	}
}
