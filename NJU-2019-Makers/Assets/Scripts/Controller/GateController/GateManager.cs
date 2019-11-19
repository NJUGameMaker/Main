using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour
{
    public enum Status
    {
        Start,
        Line1, Line2,
        Up1, Up2, Up3, Up4
    };
    public enum DoorLabel
    {
        Up,
        Down,
        Left,
        Right,
        GoIn
    };
    public Status CurrentSatus;
    //一般三个，左右下
    public Transform[] Gates;
    public Transform MidTarget;
    public Transform EndTarget;
    public GameObject MessageManager;
    public GameObject[] Messages;
    public Transform[] position;
    // Start is called before the first frame update
    void Start()
    {
        CurrentSatus = Status.Start;
        if(MessageManager && MessageManager.transform.childCount == 0)
        {
            GameObject ms = Object.Instantiate(Messages[0], position[0].position, Quaternion.identity);
            ms.transform.parent = MessageManager.transform;
        }
    }
    public Transform NextPosition(int direction)
    {
        //出去了
        if (CurrentSatus == Status.Line2 || CurrentSatus == Status.Up4)
            return EndTarget;
        if (direction == (int)DoorLabel.GoIn)
        {
            if (MessageManager && MessageManager.transform.childCount == 0 && Random.value > 0.5)
            {
                int index = (int)Random.Range(0, Messages.Length);
                GameObject ms = Object.Instantiate(Messages[index], position[index].position, Quaternion.identity);
                ms.transform.parent = MessageManager.transform;
            }
            return MidTarget;
        }
        int reindex = (int)Random.Range(0, Gates.Length);
        switch(CurrentSatus)
        {
            case Status.Start: {
                    if (direction == (int)DoorLabel.Right)
                    {
                        CurrentSatus = Status.Line1;
                        reindex = 1;
                    }
                    else
                        CurrentSatus = Status.Start;
                };break;
            case Status.Line1: {
                    if (direction == (int)DoorLabel.Down)
                    {
                        CurrentSatus = Status.Line2;
                        reindex = 2;
                    }
                    else if(direction == (int)DoorLabel.Right)
                    {
                        CurrentSatus = Status.Up1;
                        reindex = 1;
                    }
                    else
                        CurrentSatus = Status.Start;
                };break;
            case Status.Up1:{
                    if (direction == (int)DoorLabel.Right)
                    {
                        CurrentSatus = Status.Up2;
                        reindex = 1;
                    }
                    else
                        CurrentSatus = Status.Start;
                };break;
            case Status.Up2:
                {
                    if (direction == (int)DoorLabel.Right)
                    {
                        CurrentSatus = Status.Up3;
                        reindex = 1;
                    }
                    else
                        CurrentSatus = Status.Start;
                }; break;
            case Status.Up3:
                {
                    if (direction == (int)DoorLabel.Right)
                    {
                        CurrentSatus = Status.Up4;
                        reindex = 1;
                    }
                    else
                        CurrentSatus = Status.Start;
                }; break;
            default: { }break;
        }
        if(reindex >= Gates.Length)
        {
            Debug.LogAssertion("No Enough Door for the choose");
            reindex = 0;
        }
        return Gates[reindex];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
