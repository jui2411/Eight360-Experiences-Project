using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretController : MonoBehaviour
{
    [Header("Weapon System")]
    public float damage = 1f;
    public float range = 100f;
    public float shootDelay = 15f; //Fire Rate
    [Tooltip("-0.1f, 0.1f")]
    public Vector3 shootSpread;
    public ParticleSystem m_muzzleFlash;
    public ParticleSystem m_impact;
    public TrailRenderer m_beam;
    public Transform m_FireTransform;
    public AudioSource m_ShootingAudio;
    public AudioClip m_FireClip;
    public GameObject m_zoomGO;
    public float m_ZoomRotMultiplier = 1f;
    public float sensitivity = 10f;
    public float smoothTime = 5f;

    //Private Vars
    private float lastShootTime = 0f;

    Vector2 playerInput;

    private bool m_zoomed;
    private bool m_standby;


    // Start is called before the first frame update
    void Start()
    {
        ZoomOut();
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            transform.Rotate(Vector3.down, sensitivity * playerInput.x * Time.deltaTime * Mathf.Rad2Deg, Space.World);
        }
        else
        {
            transform.Rotate(Vector3.up, sensitivity * playerInput.x * Time.deltaTime * Mathf.Rad2Deg, Space.World);
        }
        
        transform.Rotate(Vector3.left, sensitivity * playerInput.y * Time.deltaTime * Mathf.Rad2Deg, Space.Self);

        if (playerInput == Vector2.zero)
        {
            m_standby = true;
        } else
        {
            m_standby = false;
        }


    }

    public void BallRotation(InputAction.CallbackContext context)
    {
        float zoomRotMultiplier;
        
        if (!m_zoomed) {
            zoomRotMultiplier = 1;
        } else {

            zoomRotMultiplier = m_ZoomRotMultiplier;
        }

        Vector2 inputVector = context.ReadValue<Vector2>();
        playerInput = new Vector2(inputVector.x * zoomRotMultiplier, inputVector.y * zoomRotMultiplier);
        
    }

    public void ZoomIn()
    {
        m_zoomGO.SetActive(true);
        m_zoomed = true;
    }

    public void ZoomOut()
    {
        m_zoomGO.SetActive(false);
        m_zoomed = false;
    }


    public void Shoot(InputAction.CallbackContext context)
    {

        if (lastShootTime + shootDelay < Time.time)
        {

            if (m_muzzleFlash) m_muzzleFlash.Play();

            RaycastHit hit;
            //Debug.Log("Fire!");
            Vector3 fireDirection = GetFireDirection();

            if (Physics.Raycast(m_FireTransform.position, fireDirection, out hit, range))
            {
                //Debug.Log(hit.transform.name);

                TrailRenderer newTrail = Instantiate(m_beam, m_FireTransform.position, m_FireTransform.rotation) as TrailRenderer;
                //newTrail.Clear();
                //newTrail.AddPosition(m_FireTransform.position);
                //newTrail.AddPosition(hit.point);
                StartCoroutine(SpawnTrail(newTrail, hit));


                Target target = hit.transform.GetComponent<Target>();
                if (target != null)// only find target component and then take damage
                {
                    target.TakeDamage(damage);
                    ScoreManager.instance.AddScore();
                }

                lastShootTime = Time.time;


            }
        }
        
        
    }

    private Vector3 GetFireDirection()
    {
        Vector3 direction = transform.forward;

        if(shootSpread != Vector3.zero)
        {
            direction += new Vector3(
                Random.Range(-shootSpread.x, shootSpread.x),
                Random.Range(-shootSpread.y, shootSpread.y),
                Random.Range(-shootSpread.z, shootSpread.z));
        }

        direction.Normalize();

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer _trail, RaycastHit _hit)
    {
        float time = 0;
        Vector3 startPos = m_FireTransform.transform.position;

        while (time < 1)
        {
            _trail.transform.position = Vector3.Lerp(startPos, _hit.point, time);
            time += Time.deltaTime / _trail.time;

            yield return null;
        }

        _trail.transform.position = _hit.point;
        Instantiate(m_impact, _hit.point, Quaternion.LookRotation(_hit.normal));

        Destroy(_trail.gameObject, _trail.time);
    }

}
