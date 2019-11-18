using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自动移动脚本
public class Move : MonoBehaviour
{

	//移动类型 
	public enum MoveType
	{
		Stop,
		Line,
		Cruve,
		Round,
		AIFollow,
		InOrder
		//TODO ...BJY
	};
	//移动的类型
	public MoveType moveType = MoveType.Stop;
	//移动的速度
	public float speed = 1;
	//初始速度
	public float StartSpeed = 1;
	//移动加速度
	public float acc = 0;
	//方向
	public Vector2 direction = new Vector2(1, 0);
	//中心和半径
	public Vector2 heart = new Vector2(2, 0);
	public float radius = 3f;
	public float roundTime = 3;//一圈的时间
	public float currentTime = 0;
	//开始点
	public Vector2 startPoint;
	public Vector2 endPoint;
	//跟踪的主角
	public GameObject follow;
	//旋转偏移角
	public float rotate;

	//附加的速度，用于被撞击后后退
	private Vector3 addSpeed = new Vector3(0, 0, 0);
	public float declineSpeed = 0.7f;//衰减常量

	//间隔更新
	private bool canMove;


	public void AddForceSpeed(Vector3 vect,float stoptime = 0,float decline = 0.7f)
	{
		declineSpeed = decline;
		var tmp = StartSpeed;
		speed = 0;
		if (stoptime != 0) StartCoroutine(Statics.WorkAfterSeconds(() => { speed = tmp; }, stoptime)); else speed = StartSpeed;
		addSpeed = vect;
	}

	//控制的物体的刚体(test)
	public Rigidbody2D rb2;

	//设置移动的类型
	public void SetMoveType(MoveType mt, Vector2 position, Rigidbody2D rb = null)
	{
		moveType = mt;
		heart = position;
		rb2 = rb;
	}
	//设置为静止的状态，参数为静止点
	public Move SetStopType(Vector2 position, Rigidbody2D rb = null)
	{
		moveType = MoveType.Stop;
		heart = position;
		rb2 = rb;
		return this;
	}

	//设置为巡逻状态，具体参数参考GoAround
	public Move SetInOrder()
	{
		moveType = MoveType.InOrder;
		return this;
	}

	//直线移动，设定开始点和移动方向
	public Move SetLineType(Vector2 position, Vector2 dt, float sp, Rigidbody2D rb = null)
	{
		moveType = MoveType.Line;
		heart = position;
		direction = dt;
		StartSpeed = speed = sp;
		rb2 = rb;
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dt.y, dt.x) * 180f / Mathf.PI + rotate);
		return this;
	}

	//直线移动，设定开始点和移动方向(加速度)
	public Move SetLineType(Vector2 position, Vector2 dt, float sp, float a,float at = 0, Rigidbody2D rb = null)
	{
		SetLineType(position, dt, sp, rb);
		acc = a;
		if (at != 0) StartCoroutine(Statics.WorkAfterSeconds(() => { acc = 0; }, at));
		return this;
	}

	//曲线移动，传入曲线开始和结束的点
	public Move SetCurveType(Vector2 start_point, Vector2 end_point, Rigidbody2D rb = null)
	{
		moveType = MoveType.Cruve;
		startPoint = start_point;
		endPoint = end_point;
		rb2 = rb;
		return this;
	}
	//圆形移动
	public Move SetRoundType(Vector2 position, float r, float roundt, Rigidbody2D rb = null)
	{
		moveType = MoveType.Round;
		heart = position;
		radius = r;
		roundTime = roundt;
		currentTime = 0;
		transform.position = new Vector3(heart.x + r, heart.y, transform.position.z);
		rb2 = rb;
		return this;
	}

	//跟踪主角视角,参数为主角
	public Move SetAIFollowType(GameObject player, float sp = 1, Rigidbody2D rb = null)
	{
		moveType = MoveType.AIFollow;
		follow = player;
		rb2 = rb;
		StartSpeed = speed = sp;
		return this;
	}

	public Move SetAIFollowType(GameObject player, float sp,float a,float at,Rigidbody2D rb = null)
	{
		SetAIFollowType(player, sp, rb);
		acc = a;
		if (at != 0) StartCoroutine(Statics.WorkAfterSeconds(() => { acc = 0; }, at));
		return this;
	}

	//设置各种类型移动的参数并且设置各种初始化函数 TODO
	public void Init()
	{

	}

	//根据类型和参数进行移动 TODO
	//update test 测试刚体运动
	private void work()
	{
		//速度参数
		Vector3 v = new Vector3(0, 0, 0);
		//分开判断处理
		switch (moveType)
		{
			case MoveType.InOrder:
				{ v = GetComponent<GoAround>().CurV; };
				break;
			case MoveType.Stop:
				{
					transform.position = new Vector3(heart.x, heart.y, transform.position.z);
				}; break;
			case MoveType.Line:
				{
					speed += acc * Time.fixedDeltaTime;
					v = new Vector3(direction.x, direction.y, 0); //新建移动向量
					v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
					v = v * speed;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
												  //transform.Translate(v);                       //移动
				}; break;
			case MoveType.Round:
				{
					float oldTime = currentTime;
					currentTime += 2 * Mathf.PI / roundTime;//更新角度
					float nextX = radius * Mathf.Cos(currentTime);
					float nextY = radius * Mathf.Sin(currentTime);
					if (currentTime >= 2 * Mathf.PI)
						currentTime = 0;
					//Vector3 v = new Vector3(nextX - radius * Mathf.Cos(currentTime), heart.y + nextY - transform.position.y, 0);
					v = new Vector3(nextX - radius * Mathf.Cos(oldTime), nextY - radius * Mathf.Sin(oldTime), 0);
					v = v.normalized ;
					v = v * speed;
					//v = v * Time.deltaTime;
					//transform.Translate(v);

				}; break;
			case MoveType.Cruve: {
                    speed += acc * Time.fixedDeltaTime;
                    v = new Vector3(Random.Range(0, 10), Random.Range(0, 10), 0); //新建移动向量
                    v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
                    v = v * speed;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
                                                  //transform.Translate(v);                       //移动
                }; break;//随机游走
			case MoveType.AIFollow:
				{
					speed += acc * Time.fixedDeltaTime;
					Vector3 target = new Vector3(0, 0, 0);
					//Vector3 target = follow.transform.position;
					//Vector2 mousePosition = Input.mousePosition; //获取屏幕坐标
					//Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition); //屏幕坐标转世界坐标
					//target.x = mouseWorldPos.x;
					//target.y = mouseWorldPos.y;
                    //if(follow)
					target = follow.transform.position;
					v = new Vector3(target.x - transform.position.x, target.y - transform.position.y, 0); //新建移动向量
					v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
					v = v * speed;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
												  //transform.Translate(v);                       //移动
					transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(v.y, v.x) * 180f / Mathf.PI + rotate);
				}; break;
			default: Debug.LogAssertion("Wrong move type"); break;
		}
		if (moveType != MoveType.Stop) v += addSpeed;
		//移动
		if (rb2)
		{
			rb2.velocity = v;
		}
		else
		{
			transform.Translate(v);
		}
		addSpeed = addSpeed * declineSpeed;

		//if (!ConstRotation)
		//{
		//	transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(v.y, v.x) * 180f / Mathf.PI);
		//}

	}
	public Vector2 GetSpeedDirection()
	{
		switch (moveType)
		{
			case MoveType.Stop:
				{
					return new Vector2(0, 0);
				}; break;
			case MoveType.Line:
				{
					return direction;
				}; break;
			case MoveType.Round:
				{
					float oldTime = currentTime;
					float newTime = oldTime + 2 * Mathf.PI / roundTime;//更新角度
					float nextX = radius * Mathf.Cos(newTime);
					float nextY = radius * Mathf.Sin(newTime);
					if (currentTime >= 2 * Mathf.PI)
						currentTime = 0;
					//Vector3 v = new Vector3(nextX - radius * Mathf.Cos(currentTime), heart.y + nextY - transform.position.y, 0);
					Vector2 v = new Vector2(nextX - radius * Mathf.Cos(oldTime), nextY - radius * Mathf.Sin(oldTime));
					return v;

				}; break;
			case MoveType.Cruve: { }; break;
			case MoveType.AIFollow:
				{
					Vector3 target = new Vector3(0, 0, 0);
					target = follow.transform.position;
					Vector2 v = new Vector2(target.x - transform.position.x, target.y - transform.position.y);
					return v;

				}; break;
			default: Debug.LogAssertion("Wrong move type"); break;
		}
		return new Vector2(0, 0);
	}

	private void FixedUpdate()
	{
		//暂停
		if (GameManager.Instance.pause)
		{
			return;
		}
		//播放剧情
		if (GameManager.Instance.playVideo)
		{
			work();
			return;
		}
		work();
	}

	private void Start()
	{
		rb2 = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
