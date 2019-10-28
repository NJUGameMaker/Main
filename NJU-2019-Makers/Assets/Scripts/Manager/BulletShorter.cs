using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShorter : MonoBehaviour
{
	enum ShotType
	{
		Static,
		Follow,
		AIFollow,
	}

	public GameObject Bullet;
	public GameObject Target;
	public float Interval;
	public int StartRotate;
	public int RotateStep;
	public float RotateInterval;
	public float Velocity;
	public float Acceleration;
	public EffectManager.EffectType OutType;

	public Move mv;

    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine(Shot()); 
    }

	IEnumerator ShotRound()
	{
		for (int r = 0; r < 360; r+=RotateStep)
		{
			GameObject bullet = Instantiate(Bullet, transform.position, Quaternion.Euler(0, 0, r + StartRotate));

			while (GameManager.Instance.pause) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(RotateInterval);
		}
	}

	IEnumerator Shot()
	{
		yield return new WaitForEndOfFrame();
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
		Shot();
	}
}
