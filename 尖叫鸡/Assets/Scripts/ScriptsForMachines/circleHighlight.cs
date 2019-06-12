using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleHighlight : MonoBehaviour
{
    private Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        trans = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        trans.localScale -= new Vector3(1f, 1f, 0);

        if(trans.localScale.x == 0.0f)
        {
            this.gameObject.SetActive(false);
        }
    }
}
