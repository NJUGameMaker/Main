using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixEnemy : MonoBehaviour
{
    public float ActiveTime;
    public float ActiveMaxDistance;
    public float ActiveMinDistance;
    public int EnemyNum;
    //public Collider2D ActiveCollider;
    public GameObject Container;
    public GameObject EnemyPrefab;
    public Transform[] KeyPoints;
    public Collider2D ActiveCollider;
    //没有设置敌人的状态，给到的敌人都是
    public enum MatrixType
    {
        Stop,
        Dot,
        RectAngle,
        Cruve,
        Round,
        AIFollow,
        InOrder
    };
    public MatrixType MType;
    // Start is called before the first frame update
    void Start()
    {
        //ActiveMaxDistance = 3;
        //ActiveMinDistance = 0;
        Container = new GameObject("Container");
        Container.transform.position = transform.position;
        //MType = MatrixType.RectAngle;
        EnemyNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        float dis = (transform.position - PlayerManager.Instance.transform.position).magnitude;
        if(ActiveCollider && ActiveCollider.IsTouching(PlayerManager.Instance.HeartCollider))
        {
            if(Container.transform.childCount > 0)
            {
                for (int i = Container.transform.childCount - 1; i >= 0; i--)
                {
                    GameObject child = Container.transform.GetChild(i).gameObject;
                    child.GetComponent<GoAround>().enabled = false;
                    child.GetComponent<Move>().moveType = Move.MoveType.AIFollow;
                    child.GetComponent<Move>().speed = 4;
                }
            }
        }
        if(Container.transform.childCount < 1)//已经没有敌人
        {
            //Debug.Log("No childer" + dis);
            if(dis < ActiveMaxDistance && dis > ActiveMinDistance)//准备生成
            {
                
                CreateEnemyMatrix();
            }
        }
        
    }
    private void CreateEnemyMatrix()
    {
        CleanAllEnemy();
        switch(MType)
        {
            case MatrixType.Stop: { }break;
            case MatrixType.RectAngle: { RectangleMatrix(); } break;
            case MatrixType.Round: { RoundMatrix(); }break;
            case MatrixType.Dot: { DotMatrix(); }break;
            default: { }break;
        }
    }
    private void CleanAllEnemy()
    {
        for (int i = Container.transform.childCount - 1; i >= 0; i--)
            Destroy(Container.transform.GetChild(i).gameObject);
    }
    //设置matrix的形状
    void SetMatrixType(MatrixType s)
    {
        MType = s;
    }

    private void StopMatrix()
    {

    }
    private void DotMatrix()
    {
        Vector3 toPoint = PlayerManager.Instance.gameObject.transform.position;
        int len = KeyPoints.Length;
        for (int i = 0; i < len; i++)
        {
            GameObject obj = Object.Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
            var ai = obj.GetComponent<EnemyAI>();
            Transform[] trans = new Transform[2];
            obj.transform.position = KeyPoints[i].position;
            trans[0] = KeyPoints[i];
            trans[1] = transform;
            obj.GetComponent<GoAround>().KeyPoints = trans;
            obj.transform.parent = Container.transform;
            ai.StartCoroutine(ai.StartActive());
        }
    }
    private void RectangleMatrix()
    {
        //Debug.Log("Begin create");
        if(KeyPoints.Length != 4)
        {
            //Debug.LogWarning("No match keyPoint in MatrixEnemy.cs");
            return;
        }
        Transform[] kp1 = { KeyPoints[1], KeyPoints[2], KeyPoints[3], KeyPoints[0] };
        Transform[] kp2 = { KeyPoints[2], KeyPoints[3], KeyPoints[0], KeyPoints[1] };
        Transform[] kp3 = { KeyPoints[3], KeyPoints[0], KeyPoints[1], KeyPoints[2] };
        Transform[] kp4 = { KeyPoints[0], KeyPoints[1], KeyPoints[2], KeyPoints[3] };
        for (int i = 0; i < 4; i++)
        {
            GameObject obj = Object.Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
            var ai = obj.GetComponent<EnemyAI>();
            switch(i)
            {
                case 0: { obj.transform.position = KeyPoints[0].position; obj.GetComponent<GoAround>().KeyPoints = kp4; };break;
                case 1: { obj.transform.position = KeyPoints[1].position; obj.GetComponent<GoAround>().KeyPoints = kp1; }; break;
                case 2: { obj.transform.position = KeyPoints[2].position; obj.GetComponent<GoAround>().KeyPoints = kp2; }; break;
                case 3: { obj.transform.position = KeyPoints[3].position; obj.GetComponent<GoAround>().KeyPoints = kp3; }; break;
                default:break;
            }
            obj.transform.parent = Container.transform;
            ai.StartCoroutine(ai.StartActive());
            
        }
    }
    private void RoundMatrix()
    {
        int len = KeyPoints.Length;
        for(int i = 0;i < len; i++)
        {
            GameObject obj = Object.Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
            var ai = obj.GetComponent<EnemyAI>();
            Transform[] trans = new Transform[len];
            obj.transform.position = KeyPoints[i].position;
            for (int j = 0; j < len; j++)
            {
                int j_index = (j + i) % len;
                trans[j] = KeyPoints[j_index];
            }
            obj.GetComponent<GoAround>().KeyPoints = trans;
            obj.transform.parent = Container.transform;
            ai.StartCoroutine(ai.StartActive());
        }
    }
}
