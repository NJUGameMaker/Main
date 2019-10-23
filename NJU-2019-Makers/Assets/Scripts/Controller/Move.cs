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
		AIFollow
		//TODO ...BJY
	};
    //移动的类型
    public MoveType moveType = MoveType.Stop;
    //移动的速度
    public float speed = 1; 
    //中心和半径
    private Vector2 heart = new Vector2(2, 0);
    private float radius = 3f;
    private float roundTime = 3;//一圈的时间
    private float currentTime = 0;
    //方向
    private Vector2 direction = new Vector2(1, 0);
    //开始点
    private Vector2 startPoint;
    private Vector2 endPoint;
    //跟踪的主角
    GameObject follow;

    //附加的速度，用于被撞击后后退
    private Vector3 addSpeed = new Vector3(0, 0, 0);
    private const float declineSpeed = 0.9f;//衰减常量
	
    public void AddForceSpeed(Vector3 vect)
    {
        addSpeed = vect;
    }

    //控制的物体的刚体(test)
	private Rigidbody2D rb2;

    //设置移动的类型
    public void SetMoveType(MoveType mt, Vector2 position,Rigidbody2D rb = null)
    {
        moveType = mt;
        heart = position;
		rb2 = rb;
    }
    //设置为静止的状态，参数为静止点
    public void SetStopType(Vector2 position, Rigidbody2D rb = null)
    {
        moveType = MoveType.Stop;
        heart = position;
		rb2 = rb;
	}
	//直线移动，设定开始点和移动方向
	public void SetLineType(Vector2 position, Vector2 dt, float sp, Rigidbody2D rb = null)
    {
        moveType = MoveType.Line;
        heart = position;
        direction = dt;
        speed = sp;
		rb2 = rb;
	}
	//曲线移动，传入曲线开始和结束的点
	public void SetCurveType(Vector2 start_point, Vector2 end_point, Rigidbody2D rb = null)
    {
        moveType = MoveType.Cruve;
        startPoint = start_point;
        endPoint = end_point;
		rb2 = rb;
    }
    //圆形移动
    public void SetRoundType(Vector2 position, float r, float roundt, Rigidbody2D rb = null)
    {
        moveType = MoveType.Round;
        heart = position;
        radius = r;
        roundTime = roundt;
        currentTime = 0;
        transform.position = new Vector3(heart.x + r, heart.y, transform.position.z);
		rb2 = rb;
	}
	//跟踪主角视角,参数为主角
	public void SetAIFollowType(GameObject player,float sp = 1 ,Rigidbody2D rb = null)
    {
        moveType = MoveType.AIFollow;
        follow = player;
		rb2 = rb;
		speed = sp;
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
		Vector3 v = new Vector3(0,0,0);
        //分开判断处理
        switch (moveType)
        {
            case MoveType.Stop: {
                    transform.position = new Vector3(heart.x, heart.y, transform.position.z);
                };break;
            case MoveType.Line: {
                v = new Vector3(direction.x, direction.y, 0); //新建移动向量
                v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
                v = v * speed;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
                //transform.Translate(v);                       //移动
                };break;
            case MoveType.Round: {
                    float oldTime = currentTime;
                    currentTime += 2 * Mathf.PI / roundTime;//更新角度
                    float nextX = radius * Mathf.Cos(currentTime);
                    float nextY = radius * Mathf.Sin(currentTime);
                    if (currentTime >= 2 * Mathf.PI)
                        currentTime = 0;
                    //Vector3 v = new Vector3(nextX - radius * Mathf.Cos(currentTime), heart.y + nextY - transform.position.y, 0);
                    v = new Vector3(nextX - radius * Mathf.Cos(oldTime), nextY - radius * Mathf.Sin(oldTime), 0);
                    //v = v.normalized ;
                    //v = v * Time.deltaTime;
                    //transform.Translate(v);
                    
                };break;
            case MoveType.Cruve: { };break;
            case MoveType.AIFollow: {
                    Vector3 target = new Vector3(0, 0, 0);
                    //Vector3 target = follow.transform.position;
                    //Vector2 mousePosition = Input.mousePosition; //获取屏幕坐标
                    //Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition); //屏幕坐标转世界坐标
                    //target.x = mouseWorldPos.x;
                    //target.y = mouseWorldPos.y;
                    target = follow.transform.position;
                    v = new Vector3(target.x - transform.position.x, target.y - transform.position.y, 0); //新建移动向量
                    v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
                    v = v * speed;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
												  //transform.Translate(v);                       //移动
					transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(v.y, v.x)*180f/Mathf.PI);
            };break;
            default: Debug.LogAssertion("Wrong move type");break;
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
        addSpeed = addSpeed * declineSpeed;
        work();
	}
}
