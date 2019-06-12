using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingEffect : MonoBehaviour
{
    private Vector3 lowerPosition;  //最低的位置
    private Vector3 upperPosition;  //最高的位置
    public float speed = 4f;      //速度设定
    public float diff_Y = 0.75f;       //当前位置和最低/最高位置的距离
    private Vector3 currentPos;     //当前位置
    private Vector3 targetPos;      //目标位置，会是最低和最高位置其中之一，会随位置变化而变化；


    // Start is called before the first frame update
    void Start()
    {
        currentPos = this.transform.position;
        lowerPosition = new Vector3(currentPos.x, currentPos.y - diff_Y, currentPos.z);
        upperPosition = new Vector3(currentPos.x, currentPos.y + diff_Y, currentPos.z);
        targetPos = lowerPosition;  //最初将位置设为最低位置，因为最初要向下移动比较intuitive
    }

    // Update is called once per frame
    void Update()
    {
        //向目标位置移动
        this.transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        //更改目标位置
        if(this.transform.position == lowerPosition)
        {
            targetPos = upperPosition;
        }

        if (this.transform.position == upperPosition)
        {
            targetPos = lowerPosition;
        }
    }
}
