using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
	public Vector2 Offset;
	public float Speed;
	private float m_speed;

	IEnumerator AddSpeed()
	{
		//yield return new WaitForSeconds(2f);
		while (m_speed < 0.4f && Offset == PlayerManager.Instance.CameraOffset)
		{
			m_speed += 0.001f;
			EffectManager.Instance.SetCameraContinueFocus(() => { return PlayerManager.Instance.transform.position + (Vector3)Offset; }, true, m_speed);
			yield return new WaitForFixedUpdate();

		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "PlayerHeart" && Offset != PlayerManager.Instance.CameraOffset)
		{
			m_speed = Speed;
			PlayerManager.Instance.CameraOffset = Offset;
			EffectManager.Instance.SetCameraContinueFocus(() => { return PlayerManager.Instance.transform.position + (Vector3)Offset; }, true, m_speed);
			StartCoroutine(AddSpeed());
		}
	}

}
