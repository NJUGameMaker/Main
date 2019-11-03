using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    float currentAmount = 100;
    const float speed = 0.3f;
	// Update is called once per frame

	private void FixedUpdate()
	{
		//currentAmount += speed;
		//if (currentAmount < 0)
		//currentAmount = 100;
		SetCurrentAmount();
		GetComponent<Image>().fillAmount = currentAmount / 100.0f;
		//GetComponent<Image>().color = new Color(1 - (currentAmount / 100.0f), (currentAmount / 100.0f), 0, (255 / 255f));
	}



	public void SetCurrentAmount()
    {
		var val = PlayerManager.Instance.bullet / PlayerManager.Instance.maxBullet * 100;
		currentAmount += (val - currentAmount)*speed;
        
    }
}
