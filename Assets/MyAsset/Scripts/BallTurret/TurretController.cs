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
    public ParticleSystem[] m_muzzleFlash;
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

    public float m_resetSensitivity = 20f;
    public float m_stabiliseSensitivity = 20f;

    private bool m_zoomed;
    private bool m_standby;
    public bool m_controlPaused;
    public bool m_isResetingRot;
    public bool m_isStabilisingRot;

    //UI
    public GameObject StabiliserReady;
    public GameObject StabilisingUI;
    public GameObject ResetRotUI;



    // Start is called before the first frame update
    void Awake()
    {
        Unzoom();
        ResetBars();
        ResetControl();
        HideHUD();
    }

    // Update is called once per frame
    void Update()
    {


        if(m_isResetingRot || m_isStabilisingRot)
        {
            playerInput = Vector2.zero;
        }

        if(m_isStabilisingRot)
        {
            RotateUpRight();
        }

        if(m_isResetingRot)
        {
            ResetRotation();
        }

        Rotate();

        if (isFiring)
        {
            Shoot();

        }


        if(transform.localEulerAngles.z > 10f && !m_isStabilisingRot)
        {
            StabiliserReady.SetActive(true);
        }

        OverheatCheck();
    }

    void HideHUD()
    {
        StabiliserReady.SetActive(false);
        ResetRotUI.SetActive(false);
        StabilisingUI.SetActive(false);
    }

    void Rotate()
    {

        // If facing upwards
        if (Vector3.Dot(transform.forward, Vector3.up) > 0.5f && Vector3.Dot(transform.forward, Vector3.up) <= 1.0f)
        {
            controlScheme = ControlScheme.Advanced;
            float zAng = transform.localEulerAngles.z;

            if (Vector3.Dot(transform.forward, Vector3.up) < 0.65f)
            {

                //float i = Vector3.Dot(transform.forward, Vector3.up);
                //float max = 0.5f;
                //float min = 0.65f;
                //float normalizedFloat = 0f;

                ////Calculate the normalized float;     
                //normalizedFloat = (i - min) / (max - min);
                ////Clamp the "i" float between "min" value and "max" value
                //i = Mathf.Clamp(i, min, max);
                ////Clamp the normalized float between 0 and 1
                //normalizedFloat = Mathf.Clamp(normalizedFloat, 0, 1);


                //float lerpAng = Mathf.Lerp(0, zAng, normalizedFloat);
                //transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, lerpAng);
            }

        }
        else
        {
            // Do not switch to simple mode when upside down
            if (Vector3.Dot(transform.up, Vector3.down) < 0f)
            {
                
                controlScheme = ControlScheme.Simple;
            }
            


        }

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
    }

    void ClampRotation()
    {

        // Clamp rot because it can get stuck
        if (transform.eulerAngles.x > 0 && transform.eulerAngles.x < 90)
        {

            transform.localEulerAngles = new Vector3(-0.1f, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        // Clamp rot because it can get stuck
        if (transform.eulerAngles.x > 320f && transform.eulerAngles.x < 359f && Vector3.Dot(transform.up, Vector3.down) > 0)
        {

            transform.localEulerAngles = new Vector3(319.9f, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }

    void RotateUpRight()
    {
        //transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0), Time.deltaTime * m_stabiliseSensitivity);

        HideHUD();
        StabilisingUI.SetActive(true);

        if (transform.localEulerAngles.z < 0.05f)
        {
            m_isResetingRot = false;
            m_isStabilisingRot = false;
            m_controlPaused = false;
            HideHUD();
        }


    }


    public void RotateInput(InputAction.CallbackContext context)
    {

        if (!m_controlPaused)
        {

            float zoomRotMultiplier;

            if (!m_zoomed)
            {
                zoomRotMultiplier = 1;
            }
            else
            {

                zoomRotMultiplier = m_ZoomRotMultiplier;
            }

            Vector2 inputVector = context.ReadValue<Vector2>();
            playerInput = new Vector2(inputVector.x * zoomRotMultiplier, inputVector.y * zoomRotMultiplier);

        }
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        if (!m_controlPaused)
        {
            if (context.started)
            {
                m_zoomGO.SetActive(true);
                m_zoomed = true;
            }

            if (context.canceled)
            {
                m_zoomGO.SetActive(false);
                m_zoomed = false;
            }
        }
    }

    public void Unzoom()
    {
        m_zoomGO.SetActive(false);
        m_zoomed = false;
    }

    public void ShootInput(InputAction.CallbackContext context)
    {
        if (!m_controlPaused)
        {
            isFiring = context.ReadValue<float>() > 0;
        }

    }

    public void ResetRotation()
    {
        // Determine which direction to rotate towards
        //Vector3 targetDirection = target.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = m_resetSensitivity * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Quaternion newDirection = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, singleStep);

        // Draw a ray pointing at our target in
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = newDirection;

        //UI
        HideHUD();
        ResetRotUI.SetActive(true);

        if(transform.rotation == Quaternion.identity)
        {
            m_isResetingRot = false;
            m_isStabilisingRot = false;
            m_controlPaused = false;
            HideHUD();
        }
    }

    void ResetControl()
    {
        m_isResetingRot = false;
        m_isStabilisingRot = false;
        m_controlPaused = false;
    }

    public void ResetRotationInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            m_controlPaused = true;
            m_isResetingRot = true;
            m_isStabilisingRot = false;
        }

    }

    public void StabiliseRotationInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_controlPaused = true;
            m_isResetingRot = false;
            m_isStabilisingRot = true;
        }
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
                foreach(ParticleSystem muzzleFlash in m_muzzleFlash)
                {
                    muzzleFlash.Play();
                }

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

