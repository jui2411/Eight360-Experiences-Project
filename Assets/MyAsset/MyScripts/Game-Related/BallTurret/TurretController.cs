using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Player
{

    public class TurretController : MonoBehaviour
    {

        public static TurretController instance;

        [Header("Functionailty")]
        [Tooltip("Projectile Spawn Point")]
        public Transform m_FireTransform;
        [Tooltip("Render Texture Object for ADS (Aim-Down Sight) Effect")]
        public GameObject m_zoomGO;
        //Private Vars
        private float lastShootTime = 0f;
        private bool isFiring;
        private bool m_zoomed;
        private bool m_standby;

        [Header("Weapon System")]
        [Tooltip("Weapon Damage Per Projectile - Recommended: 1")]
        public float damage = 1f;
        [Tooltip("Max Weapon Range - Recommended: 900K")]
        public float range = 100f;
        [Tooltip("Time between firing in secs - Recommended: 0.25")]
        public float shootDelay = 0.25f; //Fire Rate
        [Tooltip("Projectile Spreadness - Recommended: 0.01f, 0.01f, 0.01f")]
        public Vector3 shootSpread;

        [Header("Visual Effect")]
        public ParticleSystem[] m_muzzleFlash;
        public ParticleSystem m_impact;
        public TrailRenderer m_beam;
        public ParticleSystem lFire;
        public ParticleSystem rFire;

        [Header("SFX")]
        public AudioSource m_ShootingAudio;
        public AudioClip m_FireClip;
        

        [Header("Animations")]
        public Transform m_barrel_LT;
        public Transform m_barrel_LB;
        public Transform m_barrel_RT;
        public Transform m_barrel_RB;
        [Tooltip("Barrel Pullback Distance - Recommended: 0.15f")]
        public float barrelRecoil = 0.15f;
        //Private Variables
        private bool topBarrelFiring;
        private float barrelPos;

        [Header("Overheating Mechanic")]
        [Tooltip("Mininum Heat Threshold - Recommended: 0.01f")]
        public float minHeat;
        [Tooltip("Maximum Heat Threshold - Recommended: 0.99f")]
        public float maxHeat;
        [Tooltip("Heat Cooldown Per Second - Recommended: 0.05f")]
        public float decHeat;
        [Tooltip("Heat Buildup Per Shot - Recommended: 0.01f")]
        public float incHeat;
        [Tooltip("Cooldown Time")]
        public float coolingTime = 3f;
        public SingleAxisBar overheatingBar;
        //Private Vars
        [Range(0f, 1f)]
        private float heatingVal;
        private bool overheated = false;
        private bool heatCheck = false;

        [Header("Health")]
        [Tooltip("Starting Health - Recommended: 1")]
        public float health = 1f;
        public SingleAxisBar healthBar;
        //Private Vars
        private float currentHealth;

        [Header("Rotational Settings")]
        public float pitchLimit = 0f;
        public float yawLimit = 320f;
        public float sensitivity = 10f;
        public float m_ZoomRotMultiplier = 1f;
        public float pitchMaxLimit = 10f;
        public float pitchMinLimit = -90f;

        [Header("Input System")]
        public ControlScheme controlScheme;
        public enum ControlScheme { Simple, Advanced };
        [Tooltip("When true, use old input manager instead of New input manager.")]
        public bool canUseOldInputM = false;
        Vector2 playerInput;
        public Joystick joy;

        [Header("Additional Features")]
        public GameObject StabiliserReady;
        public GameObject StabilisingUI;
        public GameObject ResetRotUI;
        public float m_resetSensitivity = 20f;
        public float m_stabiliseSensitivity = 20f;
        // Private Vars
        private bool stabalizeHist;
        private bool m_controlPaused;
        private bool m_isResetingRot;
        private bool m_isStabilisingRot;

        [Header("UI")]
        [SerializeField] Image reticle;
        

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            Unzoom();
            ResetBars();
            ResetControl();
            HideHUD();

            joy = Joystick.current;
        }


        private void Start()
        {
            //Reset Barrel Animations
            barrelPos = m_barrel_LT.localPosition.x;
        }

        // Update is called once per frame
        void Update()
        {


            if (m_isResetingRot || m_isStabilisingRot)
            {
                playerInput = Vector2.zero;
            }

            if (m_isStabilisingRot)
            {
                RotateUpRight();
            }

            if (m_isResetingRot)
            {
                ResetRotation();
            }

            Rotate();

            if (isFiring)
            {
                Shoot();

            }


            if (transform.localEulerAngles.z > 10f && !m_isStabilisingRot)
            {
                StabiliserReady.SetActive(true);
            }

            OverheatCheck();

        }
        void LateUpdate()
        {

            float horizontalControl;
            float verticalControl;
            bool button0_fire;
            bool button1_aim;
            bool button4_stablise;
            bool button3_reset;

            if (!canUseOldInputM)
            {
                Vector2 joystick_input = joy.stick.ReadValue();
                horizontalControl = joystick_input.x;
                verticalControl = -joystick_input.y;
                button0_fire = joy.allControls[1].IsPressed();
                button1_aim = joy.allControls[2].IsPressed();
                button4_stablise = joy.allControls[5].IsPressed();
                button3_reset = joy.allControls[4].IsPressed();
            }
            else
            {
                horizontalControl = Input.GetAxis("Horizontal");
                verticalControl = -Input.GetAxis("Vertical");
                button0_fire = Input.GetButton("Fire1");
                button1_aim = Input.GetButton("Fire2");
                button4_stablise = Input.GetButton("Fire3");
                button3_reset = Input.GetButton("Jump");
            }

            playerInput.x = horizontalControl;
            playerInput.y = verticalControl;

            isFiring = button0_fire;
            m_zoomGO.SetActive(button1_aim);

            bool stabalizeBool = button4_stablise;

            if (stabalizeBool)
            {
                m_controlPaused = true;
                m_isResetingRot = false;
                m_isStabilisingRot = true;
            }
            stabalizeBool = false;

            bool resetBool = button3_reset;

            if (resetBool)
            {
                m_controlPaused = true;
                m_isResetingRot = true;
                m_isStabilisingRot = false;
            }
            resetBool = false;

        }

        /// <summary>
        /// Hide All HUD Elements
        /// </summary>
        private void HideHUD()
        {
            StabiliserReady.SetActive(false);
            ResetRotUI.SetActive(false);
            StabilisingUI.SetActive(false);
        }

        /// <summary>
        /// Rotates the turret.
        /// </summary>
        private void Rotate()
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
        }

        /// <summary>
        /// Limit the rotation of the ball.
        /// </summary>
        private void ClampRotation()
        {



            // Clamp rot because it can get stuck
            if (transform.eulerAngles.x > pitchLimit && transform.eulerAngles.x < (pitchLimit + 90))
            {

                transform.localEulerAngles = new Vector3((pitchLimit - 0.1f), transform.localEulerAngles.y, transform.localEulerAngles.z);
            }

            // Clamp rot because it can get stuck
            if (transform.eulerAngles.x > yawLimit && transform.eulerAngles.x < 359.9f && Vector3.Dot(transform.up, Vector3.down) > 0)
            {

                transform.localEulerAngles = new Vector3((yawLimit - 0.1f), transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
        }

        /// <summary>
        /// Reset the roll (degree of freedom) of the ball.
        /// </summary>
        private void RotateUpRight()
        {

            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0), Time.deltaTime * m_stabiliseSensitivity);

            float singleStep = m_stabiliseSensitivity * Time.deltaTime;
            Quaternion newDirection = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0), singleStep);
            transform.rotation = newDirection;

            HideHUD();
            StabilisingUI.SetActive(true);

            if (transform.localEulerAngles.z < 0.1f)
            {
                m_isResetingRot = false;
                m_isStabilisingRot = false;
                m_controlPaused = false;
                HideHUD();
            }

        }

        /// <summary>
        /// For new input manager, registering player input to rotate the ball.
        /// </summary>
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


        /// <summary>
        /// For new input manager, registering player input to zoom in.
        /// </summary>
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

        /// <summary>
        /// Unzoom player's aim by disabling render texture.
        /// </summary>
        public void Unzoom()
        {
            m_zoomGO.SetActive(false);
            m_zoomed = false;
        }


        /// <summary>
        /// For new input manager, registering player input to shoot weapon.
        /// </summary>
        public void ShootInput(InputAction.CallbackContext context)
        {
            if (!m_controlPaused)
            {
                isFiring = context.ReadValue<float>() > 0;
                reticle.color = isFiring ? Color.red * 0.5f : Color.gray * 0.5f;

            }

        }

        /// <summary>
        /// Control Feature to reset ball to origin orientation.
        /// </summary>
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

            if (transform.rotation == Quaternion.identity)
            {
                m_isResetingRot = false;
                m_isStabilisingRot = false;
                m_controlPaused = false;
                HideHUD();
            }
        }

        /// <summary>
        /// Control Feature to reset player's input from the additional features (stabilize, reset).
        /// </summary>
        private void ResetControl()
        {
            m_isResetingRot = false;
            m_isStabilisingRot = false;
            m_controlPaused = false;
        }

        /// <summary>
        /// For new input manager, registering player input to reset ball.
        /// </summary>
        public void ResetRotationInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                m_controlPaused = true;
                m_isResetingRot = true;
                m_isStabilisingRot = false;
            }

        }

        /// <summary>
        /// For new input manager, registering player input to reset roll (DoF).
        /// </summary>
        public void StabiliseRotationInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                m_controlPaused = true;
                m_isResetingRot = false;
                m_isStabilisingRot = true;
            }
        }

        /// <summary>
        /// Shoot weapon.
        /// </summary>
        private void Shoot()
        {
            // Holding/Pressing Trigger Conditions
            StopCoroutine("HeatCooldown");

            if (lastShootTime + shootDelay < Time.time)
            {

                RaycastHit hit;
                Vector3 fireDirection = GetFireDirection();

                if (Physics.Raycast(m_FireTransform.position, fireDirection, out hit, range) && !overheated)
                {

                    // Barrel Anim
                    topBarrelFiring = !topBarrelFiring;
                    BarrelAnim(topBarrelFiring);

                    foreach (ParticleSystem muzzleFlash in m_muzzleFlash)
                    {
                        muzzleFlash.Play();
                    }

                    TrailRenderer newTrail = Instantiate(m_beam, m_FireTransform.position, m_FireTransform.rotation) as TrailRenderer;

                    if (m_ShootingAudio.isActiveAndEnabled) { m_ShootingAudio.PlayOneShot(m_FireClip); }

                    StartCoroutine(SpawnTrail(newTrail, hit));

                    // Score System
                    Target target = hit.transform.GetComponent<Target>();
                    if (target != null)// only find target component and then take damage
                    {
                        target.TakeDamage(damage);
                        ScoreManager.instance.AddScore();
                    }

                    //Friendly
                    Ally teammate = hit.transform.GetComponent<Ally>();
                    if (teammate != null)// only find target component and then take damage
                    {
                        teammate.TakeDamage();
                        //ScoreManager.instance.AddScore();
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

        /// <summary>
        /// Function for animating the gun barrels, purely visual.
        /// </summary>
        private void BarrelAnim(bool isTopBarrelFiring)
        {


            if (isTopBarrelFiring)
            {
                m_barrel_LT.localPosition = new Vector3(barrelPos + barrelRecoil, m_barrel_LT.localPosition.y, m_barrel_LT.localPosition.z);
                m_barrel_RT.localPosition = new Vector3(barrelPos + barrelRecoil, m_barrel_RT.localPosition.y, m_barrel_RT.localPosition.z);
                m_barrel_LB.localPosition = new Vector3(barrelPos, m_barrel_LB.localPosition.y, m_barrel_LB.localPosition.z);
                m_barrel_RB.localPosition = new Vector3(barrelPos, m_barrel_LB.localPosition.y, m_barrel_LB.localPosition.z);
            }
            else
            {
                m_barrel_LT.localPosition = new Vector3(barrelPos, m_barrel_LT.localPosition.y, m_barrel_LT.localPosition.z);
                m_barrel_RT.localPosition = new Vector3(barrelPos, m_barrel_RT.localPosition.y, m_barrel_RT.localPosition.z);
                m_barrel_LB.localPosition = new Vector3(barrelPos + barrelRecoil, m_barrel_LB.localPosition.y, m_barrel_LB.localPosition.z);
                m_barrel_RB.localPosition = new Vector3(barrelPos + barrelRecoil, m_barrel_LB.localPosition.y, m_barrel_LB.localPosition.z);
            }
        }

        /// <summary>
        /// Reset HUD Bars to default values.
        /// </summary>
        public void ResetBars()
        {
            heatingVal = 0f;
            overheatingBar.ChangePercentage(heatingVal);

            currentHealth = health;
            healthBar.ChangePercentage(currentHealth);
        }

        /// <summary>
        /// Checking weapon's heating state. 
        /// </summary>
        private void OverheatCheck()
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
            }
            else
            {
                heatCheck = true;
            }

            //Cooldown turret
            if (heatCheck && !isFiring)
            {
                StartCoroutine("HeatCooldown");

            }
        }

        /// <summary>
        /// Weapon Cooldown Function 
        /// </summary>
        IEnumerator HeatCooldown()
        {

            if ((heatingVal - decHeat) <= minHeat)
            {
                heatingVal = 0f;
            }
            else
            {
                //wait for 1 sec
                yield return new WaitForSeconds(coolingTime);
                heatingVal -= decHeat;// decrease the value from overheat
            }

            overheatingBar.ChangePercentage(heatingVal);

        }

        /// <summary>
        /// Player taking damage.
        /// </summary>
        public void TakeDamage(float _damage)
        {
            currentHealth -= _damage;
            healthBar.ChangePercentage(currentHealth);

            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                Die();
            }

        }

        /// <summary>
        /// Player's Game Over Sequence.
        /// </summary>
        private void Die()
        {
            ScoreManager gameManager = ScoreManager.instance;
            gameManager.GameOver();

        }

        /// <summary>
        /// Weapon Spread Function.
        /// </summary>
        private Vector3 GetFireDirection()
        {
            Vector3 direction = transform.forward;

            if (shootSpread != Vector3.zero)
            {
                direction += new Vector3(
                    Random.Range(-shootSpread.x, shootSpread.x),
                    Random.Range(-shootSpread.y, shootSpread.y),
                    Random.Range(-shootSpread.z, shootSpread.z));
            }

            direction.Normalize();

            return direction;
        }

        /// <summary>
        /// Projectile Trail Function.
        /// </summary>
        private IEnumerator SpawnTrail(TrailRenderer _trail, RaycastHit _hit)
        {
            float time = 0;
            Vector3 startPos = m_FireTransform.transform.position;
            lFire.Emit(1);
            rFire.Emit(1);
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
}

