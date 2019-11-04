using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoAround : MonoBehaviour
{

	public enum Type
	{
		Random,
		InOrder,
	}

	public Type type;
	public Transform[] KeyPoints;
	public float RunTime;
	public float StopTime;
	public float StartTime;
	public int RoundTimes { get; private set; }
	private Rigidbody2D rigidbody2;
	private int now;
	private int maxSize;


	IEnumerator Go()
	{
		yield return new WaitForSeconds(StartTime);
		while (true)
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
				}
			}
			rigidbody2.velocity = (KeyPoints[next].position - KeyPoints[now].position) / RunTime;
			yield return new WaitForSeconds(RunTime);
			rigidbody2.velocity = Vector2.zero;
			yield return new WaitForSeconds(StopTime);
			now = next;
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
		StartCoroutine(Go());
    }

    // Update is called once per frame
    void Update()
    {
    }
}
