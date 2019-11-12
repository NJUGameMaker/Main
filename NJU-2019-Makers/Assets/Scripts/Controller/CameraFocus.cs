using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
	public float OffsetX;
	public float OffsetY;
	public float Speed = 0.2f;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "PlayerHeart")
		{
			EffectManager.Instance.SetCameraContinueFocus(() => { return PlayerManager.Instance.transform.position + new Vector3(OffsetX, OffsetY, 0); }, true, Speed);
		}
	}

}
