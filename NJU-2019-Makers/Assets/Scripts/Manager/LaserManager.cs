using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
	//预制激光特效
	public GameObject Laser;

	//激光检测参数
	private const int wallmask = 0b11 << 10;
	private const int enemymask = 0b1 << 12;
	private const int playermask = 0b11 << 8;
	private const int maxdis = 200;
	private LayerMask mask = new LayerMask();

	//test
	private Vector3 st = new Vector3();
	private Vector3 ed = new Vector3();


	public void MakeLaser(Vector2 pos,Vector2 dir,bool isplayer,ref RaycastHit2D[] res,float time = 0.5f)
	{
		//物理碰撞
		RaycastHit2D end = Physics2D.Raycast(pos, dir, maxdis, wallmask);
		if (isplayer)
		{
			res = Physics2D.RaycastAll(pos, dir,maxdis,enemymask);
		}
		else
		{
			res = Physics2D.RaycastAll(pos, dir, maxdis, playermask);
		}

		//图形渲染
		var laser = Instantiate(Laser);
		var lr = laser.GetComponent<LineRenderer>();
		lr.SetPosition(0, pos);
		if (end)
		{
			lr.SetPosition(1, end.point);
		}
		else
		{
			lr.SetPosition(1, pos + dir.normalized * maxdis);
		}
		foreach (var x in res)
		{
			Debug.Log(x.collider.tag);
		}
		StartCoroutine(Statics.DestroyAfterSeconds(laser, time));
	}

	private void Start() { }

	private void Update()
	{
		//test
		RaycastHit2D[] tmp = { };
		if (Input.GetMouseButtonDown(0))
		{
			st = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            laserBullet.enabled = true;
            FireLaser();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			ed = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			MakeLaser(st, ed - st, false,ref tmp);
            laserBullet.enabled = false;
        }
	}

    public LineRenderer laserBullet;
    public void FireLaser()
    {
        RaycastHit2D hit;
        //hit = Physics2D.Raycast(PlayerManager.Instance.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector2 startpoint;
        startpoint.x = 20;
        startpoint.y = 20;
        Vector2 endpoint;
        endpoint.x = 0;
        endpoint.y = 0;
        hit = Physics2D.Raycast(startpoint, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //激光发射碰撞
        if (hit && laserBullet.enabled == true)
        {
            Debug.Log("collider");
            switch(hit.collider.tag)
            {
                case "Enemy": { };break;
                default:break;
            }
            
            //射线的起始点
            laserBullet.SetPosition(0, startpoint);

            //因为激光只有一个终点，所以障碍物位置为终点
            //laserBullet.SetPosition(1, hit.point);
            laserBullet.SetPosition(1, endpoint);
        }
        else
        {
            Debug.Log(" no collider");
            //射线的起始点
            laserBullet.SetPosition(0, startpoint);

            //因为激光只有一个终点，所以障碍物位置为终点
            laserBullet.SetPosition(1, endpoint);
        }
        Debug.Log(startpoint + (100 * (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)));
    }
}
