using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    // Start is called before the first frame update


    public Transform camTransform;    //transform震动对象
    public float shakeTime = 0.7f;    //震动时间
    public bool shakeEffect = false;    //震动标志
    public float shakePower = 1.0f;     //震动强度

    private float timeTool;     //工具计时
    private Vector3 originPos;      //初始位置
    void Start()
    {
        if (camTransform == null)
            camTransform = transform;
        originPos = camTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeEffect)
        {
            if(timeTool > 0)
            {
                camTransform.localPosition = originPos + Random.insideUnitSphere * shakePower;
                timeTool -= Time.deltaTime;
            }
            else
            {
                shakeEffect = false;
                timeTool = shakeTime;
                camTransform.localPosition = originPos;
            }
        }
        
    }
    //提供开始的接口：time表示shake时间,默认是0.7
    public void BeginShake(float time = 0.7f)
    {
        originPos = camTransform.localPosition;
        shakeTime = time;
        timeTool = time;
        shakeEffect = true;
    }
    //设置震动力度
    public void SetShakePower(float power)
    {
        shakePower = power;
    }
    //设置震动的时间
    public void SetShakeTime(float time)
    {
        shakeTime = time; 
    }
}
