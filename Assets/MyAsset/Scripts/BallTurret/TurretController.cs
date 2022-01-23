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

    //Overheating Mechanism
    public float minHeat;
    public float maxHeat;
    public float decHeat;
    public float incHeat;
    [Range(0f,1f)]
    public float heatingVal;
    public float coolingTime = 3f;
    public bool overheated = false;
    public bool heatCheck = false;
    public SingleAxisBar overheatingBar;

    // Health
    public float health = 1f;
    private float currentHealth;
    public SingleAxisBar healthBar;

    public float smoothTime = 5f;

    [Header("Camera Settings")]
    public float sensitivity = 10f;
    public float m_ZoomRotMultiplier = 1f;
    public float pitchMaxLimit = 10f;
    public float pitchMinLimit = -90f;

    public enum ControlScheme {Simple, Advanced };
    public ControlScheme controlScheme;

    public bool isFiring;
    //Private Vars
    private float lastShootTime = 0f;

    Vector2 playerInput;

    private bool m_zoomed;
    private bool m_standby;


    // Start is called before the first frame update
    void Start()
    {
        Unzoom();
        ResetBars();
    }

    // Update is called once per frame
    void Update()
    {

        if (controlScheme == ControlScheme.Simple)
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
            ClampRotation();
        }
        else if (controlScheme == ControlScheme.Advanced)
        {

            //Control 2
            transform.Rotate(Vector3.left, sensitivity * playerInput.y * Time.deltaTime * Mathf.Rad2Deg, Space.Self);
            transform.Rotate(Vector3.up, sensitivity * playerInput.x * Time.deltaTime * Mathf.Rad2Deg, Space.Self);
            ClampRotation();
        }

        if (playerInput == Vector2.zero)
        {
            m_standby = true;
        } else
        {
            m_standby = false;
        }

        if (isFiring)
        {
            Shoot();

        }

        OverheatCheck();
    }

    void ClampRotation()
    {

        // Clamp rot because it can get stuck
        if (transform.eulerAngles.x > 0 && transform.eulerAngles.x < 90)
        {

            transform.localEulerAngles = new Vector3(-1f, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        // Clamp rot because it can get stuck
        if (transform.eulerAngles.x > 320f && transform.eulerAngles.x < 359f && Vector3.Dot(transform.up, Vector3.down) > 0)
        {

            transform.localEulerAngles = new Vector3(319f, transform.localEulerAngles.y, transform.localEulerAngles.z);
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

    public void Zoom(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            m_zoomGO.SetActive(true);
            m_zoomed = true;
        }

        if(context.canceled)
        {
            m_zoomGO.SetActive(false);
            m_zoomed = false;
        }
    }

    public void Unzoom()
    {
        m_zoomGO.SetActive(false);
        m_zoomed = false;
    }

    public void ShootInput(InputAction.CallbackContext context)
    {

        isFiring = context.ReadValue<float>() > 0;

    }

    public void ResetRotation()
    {
        //float resetTime = 0f;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, resetTime);
        //resetTime += Time.deltaTime;
        transform.rotation = Quaternion.identity;
    }



    void Shoot()
    {
        // Holding/Pressing Trigger Conditions
        StopCoroutine("HeatCooldown");

        if (lastShootTime + shootDelay < Time.time)
        {

            

            RaycastHit hit;
            //Debug.Log("Fire!");
            Vector3 fireDirection = GetFireDirection();

            if (Physics.Raycast(m_FireTransform.position, fireDirection, out hit, range) && !overheated)
            {

                if (m_muzzleFlash) m_muzzleFlash.Play();

                TrailRenderer newTrail = Instantiate(m_beam, m_FireTransform.position, m_FireTransform.rotation) as TrailRenderer;

                m_ShootingAudio.clip = m_FireClip;
                m_ShootingAudio.Play();

                StartCoroutine(SpawnTrail(newTrail, hit));


                // Score System
                Target target = hit.transform.GetComponent<Target>();
                if (target != null)// only find target component and then take damage
                {
                    target.TakeDamage(damage);
                    ScoreManager.instance.AddScore();
                }

                // Overheating Mechanism
                heatingVal += incHeat;
                heatCheck = true;

                overheatingBar.ChangePercentage(heatingVal);

                // Ending Shoot Function
                lastShootTime = Time.time;


            }
        }
    }

    void ResetBars()
    {
        heatingVal = 0f;
        overheatingBar.ChangePercentage(heatingVal);

        currentHealth = health;
        healthBar.ChangePercentage(currentHealth);
    }

    void OverheatCheck()
    {
        //heat clamps
        if (heatingVal >= maxHeat)
        {
            heatingVal = maxHeat;
            heatCheck = true;
            overheated = true;

        }
        else if (heatingVal <= minHeat)
        {
            heatingVal = minHeat;
            heatCheck = false;
            if (overheated) overheated = false;
        } else
        {
            heatCheck = true;
        }

        //Cooldown turret
        if (heatCheck && !isFiring)
        {
            StartCoroutine("HeatCooldown");
            
        }
    }

    IEnumerator HeatCooldown()
    {
        
        if((heatingVal - decHeat) <= minHeat)
        {
            heatingVal = 0f;
        } else
        {
            //wait for 1 sec
            yield return new WaitForSeconds(coolingTime);
            heatingVal -= decHeat;// decrease the value from overheat
        }

        overheatingBar.ChangePercentage(heatingVal);

    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        healthBar.ChangePercentage(currentHealth);

        if(currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }

    }

    [ContextMenu("HURT")]
    public void DoDamange()
    {
        TakeDamage(0.1f);
    }

    void Die()
    {
        //Gameover


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
