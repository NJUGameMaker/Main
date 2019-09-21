using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{

	private EdgeCollider2D ec2;
    // Start is called before the first frame update
    void Start()
    {
		ec2 = GetComponent<EdgeCollider2D>();
		Debug.Log(ec2.points.Length);
		Vector2[] vector2s = new Vector2[20];
		const int len = 1;
		for (int i = 0; i < vector2s.Length; i++)
		{
			vector2s[i] = new Vector2(len * Mathf.Cos(2 * Mathf.PI / (vector2s.Length-1) * i), len * Mathf.Sin(2 * Mathf.PI / (vector2s.Length-1) * i));
			//Debug.Log(vector2s[i]);
		}
		ec2.points = vector2s;


	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.DrawLine(new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y,0), new Vector3(collision.contacts[0].point.x + collision.relativeVelocity.x, collision.contacts[0].point.y + collision.relativeVelocity.y, 0),Color.blue,10);
	}

	// Update is called once per frame
	void Update()
    {

    }
}
