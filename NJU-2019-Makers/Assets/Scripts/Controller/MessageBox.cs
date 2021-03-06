﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
	public string[] Messages;
	private bool MouseDown;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0)) MouseDown = true;
	}

	IEnumerator StartMessage()
	{
		AudioManager.Instance.PlaySound("Collect");
		UIManager.Instance.ShowDialog();
		GameManager.Instance.GameVideo();
		PlayerManager.Instance.m_rb.velocity = Vector2.zero;
		yield return new WaitForSeconds(0.6f);
		StartCoroutine(Statics.Flash(GetComponentInChildren<SpriteRenderer>(), Color.white, Color.clear, 0.7f));
		foreach (var item in Messages)
		{
			UIManager.Instance.ShowText(item);
			MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		}
		//MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.HideDialogAndText();
		GameManager.Instance.GameRestart();
		GetComponentInChildren<Collider2D>().enabled = false;
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "PlayerHeart")
		{
			StartCoroutine(StartMessage());
		}
	}
}
