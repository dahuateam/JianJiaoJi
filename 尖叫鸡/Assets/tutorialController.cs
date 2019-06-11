using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialController : MonoBehaviour
{
    public GameObject tt_one;   //tutorial one;
    public GameObject tt_two;   //tutorial two;
    public GameObject tt_three;
    public GameObject tt_four;
    public GameObject tt_five;
    public GameObject tt_six;
    public GameObject tt_seven;
    static public int stepLeft = 0;
    static public int stepRight = 0;
    public int leftTest;
    public int rightTest;


    // Start is called before the first frame update
    void Start()
    {
        StepOne();
        StepTwo();
    }

    // Update is called once per frame
    void Update()
    {
        leftTest = stepLeft;
        rightTest = stepRight;
    }

    private void StepOne()
    {
        if (stepLeft == 0)
        {
            stepLeft = 1;
            tt_one.gameObject.SetActive(true);
        }
    }

    private void StepTwo()
    {
        if (stepRight == 0)
        {
            stepRight = 1;
            tt_two.SetActive(true);
        }
    }

    public void StepThree()
    {
        if (stepLeft == 1)
        {
            stepLeft = 2;
            tt_one.SetActive(false);
            tt_three.SetActive(true);
        }
    }

    public void StepFour()
    {
        if (stepRight == 1)
        {
            stepRight = 2;
            tt_two.SetActive(false);
            tt_four.SetActive(true);
        }
    }

    public void StepFive()
    {
        if (stepLeft == 2)
        {
            stepLeft = 3;
            tt_three.SetActive(false);
            tt_five.SetActive(true);
        }
        if(stepRight == 2)
        {
            tt_four.SetActive(false);
            tt_five.SetActive(true);
        }
    }

    public void StepSix()
    {
        if (stepRight == 2)
        {
            stepLeft = 4;
            stepRight = 4;
            tt_five.SetActive(false);
            tt_six.SetActive(true);
        }
    }

    public void StepSeven()
    {
        if(stepRight == 4 && stepLeft == 4)
        {
            tt_six.SetActive(false);
            tt_seven.SetActive(true);
            stepRight = 5;
            stepLeft = 5;
        }
    }

}
