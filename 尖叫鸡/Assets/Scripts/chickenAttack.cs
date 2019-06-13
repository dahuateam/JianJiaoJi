using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chickenAttack : MonoBehaviour
{
    public float lowerLine;
    public float thrust;
	public float enterSpeed;//鸡在门被攻破后进入场景的行动速度

    //public GameObject target1;
    //public GameObject target2;
    //public GameObject target3;

    private Rigidbody2D rb2;
    private SpriteRenderer sr;
    private Transform trans;

	private int status = 0;//状态0-破门前,状态1-破门后前往入口中,状态2-进门后向下行走中
	private Vector3 enterPos;//破口位置

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
		if (status == 0)
		{
			if (trans.position.y < lowerLine)
			{
				this.GetComponent<Rigidbody2D>().AddForce(Vector3.up * thrust);
			}

			//以后再实现，不重要
			//transform.position = Vector2.MoveTowards(transform.position, target1.transform.position, speed * Time.deltaTime);

			if (rb2.velocity.x > 0)
			{
				sr.flipX = true;
			}
			else if (rb2.velocity.x < 0)
			{
				sr.flipX = false;
			}

			if (trans.position.x < -60f || trans.position.x < -60f || trans.position.y < 33f)
			{
				trans.position = new Vector3(0f, 40f, 0f);
			}
		}
		else if (status == 1)
		{
			transform.position = Vector3.Lerp(transform.position, enterPos, enterSpeed * Time.deltaTime);
			//rb2.AddForce((enterPos - transform.position) * enterSpeed);
			if (rb2.velocity.x > 0)
			{
				sr.flipX = true;
			}
			else if (rb2.velocity.x < 0)
			{
				sr.flipX = false;
			}
			//transform.Translate((enterPos - transform.position) * enterSpeed);
			if ((enterPos - transform.position).magnitude <= 1.0f)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y, -1.0f);
				rb2.velocity = new Vector2(0.0f, -15.0f);
				status = 2;
			}
		}
		else if (status == 2)
		{
			rb2.AddForce(Vector3.down * enterSpeed);
		}
    }

	public void ChickenEnter(Vector3 pos)
	{
		enterPos = pos;
		status = 1;
		transform.GetComponent<Collider2D>().enabled = false;
		rb2.velocity = new Vector2(0.0f, 0.0f);
		rb2.gravityScale = 0.0f;
		//rb2.velocity = Vector2.zero;
	}

}
