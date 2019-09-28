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
		//TODO ...
	};
    //移动的类型
    MoveType moveType = MoveType.Stop;
    //中心
    Vector2 heart;
    //设置移动的类型
    public void SetMoveType(MoveType mt, Vector2 position)
    {
        moveType = mt;
        heart = position;
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
            case MoveType.Stop: { };break;
            case MoveType.Line: { };break;
            case MoveType.Round: { };break;
            case MoveType.Cruve: { };break;
            case MoveType.AIFollow: { };break;
            default: Debug.LogAssertion("Wrong move type");break;
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
		work();
	}
}
