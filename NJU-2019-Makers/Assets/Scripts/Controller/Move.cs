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
    public float speed = 50; 
    //中心和半径
    private Vector2 heart = new Vector2(2, 0);
    private float radius = 3f;
    private float roundTime = 3;//一圈的时间
    private float currentTime = 0;
    //方向
    private Vector2 direction;
    //开始点
    private Vector2 startPoint;
    private Vector2 endPoint;
    //跟踪的主角
    GameObject follow;
    //设置移动的类型
    public void SetMoveType(MoveType mt, Vector2 position)
    {
        moveType = mt;
        heart = position;
    }
    //设置为静止的状态，参数为静止点
    public void SetStopType(Vector2 position)
    {
        heart = position;
    }
    //直线移动，设定开始点和移动方向
    public void SetLineType(Vector2 position, Vector2 dt, float sp)
    {
        heart = position;
        direction = dt;
        speed = sp;
    }
    //曲线移动，传入曲线开始和结束的点
    public void SetCurveType(Vector2 start_point, Vector2 end_point)
    {
        startPoint = start_point;
        endPoint = end_point;
    }
    //圆形移动
    public void SetRoundType(Vector2 position, float r, float roundt)
    {
        heart = position;
        radius = r;
        roundTime = roundt;
        currentTime = 0;
        transform.position = new Vector3(heart.x + r, heart.y, transform.position.z);
        
    }
    //跟踪主角视角,参数为主角
    public void SetAIFollowType(GameObject player)
    {
        follow = player;
    }
    //设置各种类型移动的参数并且设置各种初始化函数 TODO
    public void Init()
	{

	}

	//根据类型和参数进行移动 TODO
	private void work()
	{
        //分开判断处理
        switch (moveType)
        {
            case MoveType.Stop: {
                    transform.position = new Vector3(heart.x, heart.y, transform.position.z);
                };break;
            case MoveType.Line: {
                Vector3 v = new Vector3(direction.x, direction.y, 0); //新建移动向量
                v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
                v = v * speed * Time.deltaTime;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
                transform.Translate(v);                       //移动
                };break;
            case MoveType.Round: {
                    float oldTime = currentTime;
                    currentTime += 2 * Mathf.PI / roundTime * Time.deltaTime;//更新角度
                    float nextX = radius * Mathf.Cos(currentTime);
                    float nextY = radius * Mathf.Sin(currentTime);
                    if (currentTime >= 2 * Mathf.PI)
                        currentTime = 0;
                    //Vector3 v = new Vector3(nextX - radius * Mathf.Cos(currentTime), heart.y + nextY - transform.position.y, 0);
                    Vector3 v = new Vector3(nextX - radius * Mathf.Cos(oldTime), nextY - radius * Mathf.Sin(oldTime), 0);
                    //v = v.normalized ;
                    //v = v * Time.deltaTime;
                    transform.Translate(v);
                    
                };break;
            case MoveType.Cruve: { };break;
            case MoveType.AIFollow: { };break;
            default: Debug.LogAssertion("Wrong move type");break;
        }
	}

    // Update is called once per frame
    void Update()
    {
		//暂停
		/*if (GameManager.Instance.pause)
		{
			return;
		}
		//播放剧情
		if (GameManager.Instance.playVideo)
		{
			return;
		}*/
		work();
	}
}
