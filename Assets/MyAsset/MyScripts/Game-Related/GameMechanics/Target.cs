using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public GameObject destMarker;
    [HideInInspector]
    public Vector3 destPos;
    [HideInInspector]
    public float speed = 10f;

    public AudioSource m_AudioS;
    public AudioClip m_DeathClip;

    private Player.TurretController player;
    public GameObject FX;
    public ParticleSystem m_hitFX;
    public bool DebugRoute;
    public bool doesDamage;
    public bool canFracture;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<Player.TurretController>();

        m_AudioS.clip = m_DeathClip;
    }

    private void Update()
    {
        if(destPos != Vector3.zero)
        {
            
            Debug.DrawLine(transform.position, destPos);
        }

        transform.rotation = Quaternion.FromToRotation(Vector3.forward, destPos);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }

        m_hitFX.Play();
    }
    void Die()
    {
        
        GameObject FracturePS = Instantiate(FX, transform.position, transform.rotation);
        FracturePS.transform.localScale = transform.localScale;
        

        Fracture fractureScript = GetComponent<Fracture>();

        if(fractureScript != null && canFracture)
        {
            fractureScript.FractureObject();
        }

        
        if(m_AudioS.isActiveAndEnabled) m_AudioS.Play();

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        DestructionManager.instance.RemoveFromTargetList(this.gameObject);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

    }

    public void SetDestination(Vector3 _destPos)
    {
        destPos = _destPos;

        if (destMarker && !DebugRoute)
        {
            GameObject marker = Instantiate(destMarker, destPos, transform.rotation);
        }
    }

    public void MoveToDestination() 
    {
        float step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, destPos, step);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Health"))
        {
            if (doesDamage)
            {
                player.TakeDamage(0.1f);
            }

            OptimizedDie();
        }
        
    }

    void OptimizedDie()
    {
        if (canFracture)
        {
            GameObject FracturePS = Instantiate(FX, transform.position, transform.rotation);
            FracturePS.transform.localScale = transform.localScale;
        }

        if (m_AudioS.isActiveAndEnabled)  m_AudioS.Play();

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        DestructionManager.instance.RemoveFromTargetList(this.gameObject);


        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

    }

    void OnDestroy()
    {
        DestructionManager.instance.RemoveFromTargetList(this.gameObject);
    }


}