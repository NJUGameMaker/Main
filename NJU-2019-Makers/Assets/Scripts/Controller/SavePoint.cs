using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
	public Map1Manager Map1;
	public Map1Manager.SavePoint save;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "PlayerHeart")
		{
			if (Map1.save != save) Map1.ReLoad(save);
			Map1.save = save;
		}
	}
}
