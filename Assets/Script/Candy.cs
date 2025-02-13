using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public Board board;

    public float column;
    public float row;

    public float targetX;
    public float targetY;

    public float speed=3;
    public bool isReachFinish;
    void Start()
    {
        board=FindAnyObjectByType<Board>();
        row = targetY;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;

        bool reachX = false;
        bool reachY = false;

        if(Mathf.Abs(targetX-transform.position.x)>0.1f)
        {
            Vector2 tempPos=new Vector2(targetX,transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, speed * Time.deltaTime);
            if (board.allCandies[(int)column,(int)row]!=this.gameObject)
            {
                board.allCandies[(int)column, (int)row] = this.gameObject;
            }
        }
        else
        {
            Vector2 tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            reachX = true;
           
        }

        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            Vector2 tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, speed * Time.deltaTime);
            if (board.allCandies[(int)column, (int)row] != this.gameObject)
            {
                board.allCandies[(int)column, (int)row] = this.gameObject;
            }
        }
        else
        {
            Vector2 tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            board.allCandies[(int)column, (int)row] = this.gameObject;
            reachY = true;
        }

        if(reachX&&reachY)
        {
            isReachFinish = true;
        }
        else
        {
            isReachFinish= false;
        }
    }
}
