using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chickenAttack : MonoBehaviour
{
    public float lowerLine;
    public float thrust;

    //public GameObject target1;
    //public GameObject target2;
    //public GameObject target3;

    private Rigidbody2D rb2;
    private SpriteRenderer sr;
    private Transform trans;

    // Start is called before the first frame update
    void Start()
    {
        rb2 = this.GetComponent<Rigidbody2D>();
        sr = this.GetComponent<SpriteRenderer>();
        trans = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(trans.position.y < lowerLine)
        {
            this.GetComponent<Rigidbody2D>().AddForce(Vector3.up * thrust);
        }

        //以后再实现，不重要
        //transform.position = Vector2.MoveTowards(transform.position, target1.transform.position, speed * Time.deltaTime);

        if(rb2.velocity.x > 0)
        {
            sr.flipX = true;
        }
        else if (rb2.velocity.x < 0)
        {
            sr.flipX = false;
        }

        if(trans.position.x < -60f || trans.position.x < -60f || trans.position.y < 33f)
        {
            trans.position = new Vector3(0f, 40f, 0f);
        }

    }
}
