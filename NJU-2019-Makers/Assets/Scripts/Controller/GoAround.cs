using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoAround : MonoBehaviour
{

	public enum Type
	{
		Random,
		InOrder,
		AllRandom,
	}

	public Type type;
	public Transform[] KeyPoints;
	public float RunTime;
	public float StopTime;
	public float StartTime;
	public float A_dis_min;
	public float A_dis_max;
	public int RoundTimes { get; private set; }
	public CallBack RunAfterStep;
	public CallBack RunAfterRound;
	private Rigidbody2D rigidbody2;
	private int now;
	private int maxSize;
	private Vector2 sp;


	IEnumerator Go()
	{
		yield return new WaitForSeconds(StartTime);
		while (true)
		{
			if (type == Type.AllRandom)
			{
				rigidbody2.velocity = Statics.FaceVec(Quaternion.Euler(0,0,Random.Range(0,360))) * Random.Range(A_dis_min,A_dis_max) / RunTime;
				RoundTimes++;
				yield return new WaitForSeconds(RunTime);
				if (RunAfterStep) RunAfterStep.Fun();
				rigidbody2.velocity = Vector2.zero;
				yield return new WaitForSeconds(StopTime);
			}
			else
			{
				int next;
				if (type == Type.Random)
				{
					next = Random.Range(0, maxSize);
				}
				else
				{
					next = now + 1;
					if (next == maxSize)
					{
						next = 0;
						RoundTimes++;
						if (RunAfterRound) RunAfterRound.Fun();
					}
				}
				rigidbody2.velocity = (KeyPoints[next].position - KeyPoints[now].position) / RunTime;
				yield return new WaitForSeconds(RunTime);
				if (RunAfterStep) RunAfterStep.Fun();
				rigidbody2.velocity = Vector2.zero;
				yield return new WaitForSeconds(StopTime);
				now = next;
			}
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		now = 0;
		RoundTimes = 0;
		//transform.position = KeyPoints[now].position;
		maxSize = KeyPoints.Length;
		rigidbody2 = GetComponent<Rigidbody2D>();
		if (maxSize>1 || type == Type.AllRandom) StartCoroutine(Go());
    }

    // Update is called once per frame
    void Update()
    {
	}
}
