using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pEffectController : MonoBehaviour
{
    public GameObject pe1;
    public GameObject pe2;
    public GameObject pe3;
    public GameObject pe4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void playCompletion(float tx, float ty)
    {
        pe1.gameObject.transform.position = new Vector3(tx, ty, 0f);
        pe2.gameObject.transform.position = new Vector3(tx, ty, 0f);

        pe1.gameObject.GetComponent<ParticleSystem>().Play();
        pe2.gameObject.GetComponent<ParticleSystem>().Play();
    }

    public void doorDamanged(float tx, float ty)
    {
        pe3.gameObject.transform.position = new Vector3(tx, ty, 0f);
        pe4.gameObject.transform.position = new Vector3(tx, ty, 0f);

        pe3.gameObject.GetComponent<ParticleSystem>().Play();
        pe4.gameObject.GetComponent<ParticleSystem>().Play();
    }
}
