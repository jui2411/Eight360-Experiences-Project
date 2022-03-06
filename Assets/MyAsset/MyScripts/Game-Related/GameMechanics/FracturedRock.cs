using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedRock : MonoBehaviour
{
    private float t = 0;
    private float startScale = 0f;
    private float endScale = 0f;
    public float scalingTime = 3f;
    public GameObject m_SmokeTrailFX;
    private float parentScale;

    private GameObject trailPS;

    // Start is called before the first frame update
    void Start()
    {
        startScale = this.transform.localScale.x;
        endScale = 0f;
        parentScale = this.transform.parent.localScale.x;


        trailPS = Instantiate(m_SmokeTrailFX, transform.position, transform.rotation);
        trailPS.transform.localScale = new Vector3(1, 1, 1);
        trailPS.transform.SetParent(this.transform);
        var PSmain = trailPS.GetComponent<ParticleSystem>().main;
        PSmain.startSize = parentScale;



    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        this.transform.localScale = new Vector3(Mathf.Lerp(startScale, endScale, t / scalingTime), 
            Mathf.Lerp(startScale, endScale, t / scalingTime), 
            Mathf.Lerp(startScale, endScale, t / scalingTime)); 

        if(this.transform.localScale.x < 0.1f)
        {
            Disintegrate();
        }

        trailPS.transform.localScale = transform.localScale;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Health"))
        {
            
            this.gameObject.SetActive(false);
        }
        
    }

    void Disintegrate()
    {
        this.gameObject.SetActive(false);
    }
}
