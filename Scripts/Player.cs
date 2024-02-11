using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;


public class Player : MonoBehaviourPunCallbacks
{
    #region Variables
    public float speed;
    public float sprintModifier;
    public float jumpForce;
    public int max_health;
    public Camera normalCam;
    public GameObject cameraParent;
    public Transform weaponParent;
    public Transform groundDetector;
    public LayerMask ground;
    public int deathValue = 1;


    private Transform ui_healthbar;
    private Text ui_ammo;
    private Text ui_kills;

    private AudioSource mAudioSrc;
    private Rigidbody rig;

    private Vector3 targetWeaponBobPosition;
    private Vector3 weaponParentOrigin;

    private float movementCounter;
    private float idleCounter;

    private float baseFOV;
    private float sprintFOVModifier = 1.07f;

   private int current_health;

    private Manager manager;
    private Weapon weapon;

    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {
        //mAudioSrc = GetComponent<AudioSource>();
        weapon = GetComponent<Weapon>();
        manager = GameObject.Find("Manager").GetComponent<Manager>();

        current_health = max_health;
       
        cameraParent.SetActive(photonView.IsMine);

        if(!photonView.IsMine) gameObject.layer = 11;
        
        

        baseFOV = normalCam.fieldOfView;


       if(Camera.main) Camera.main.enabled = false;

        rig = GetComponent<Rigidbody>();
        weaponParentOrigin = weaponParent.localPosition;

        if (photonView.IsMine)
        { 
        ui_healthbar = GameObject.Find("HUD/Health/Bar").transform;
        ui_ammo = GameObject.Find("HUD/Ammo/Text").GetComponent<Text>();
        ui_kills = GameObject.Find("HUD/Kills/Text").GetComponent<Text>();
        
        RefreshHealthBar();
        }
    }

    private void Update()
    {

        if (!photonView.IsMine) return;

        float t_hmove = Input.GetAxisRaw("Horizontal");
        float t_vmove = Input.GetAxisRaw("Vertical");

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetKeyDown(KeyCode.Space);

        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.4f, ground);
        bool isJumping = jump && isGrounded;
        bool isSprinting = sprint && t_vmove > 0 && !isJumping && isGrounded;

        if (isJumping)

        {
            rig.AddForce(Vector3.up * jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.U)) TakeDamage(25);

        //Head Bob
        if (t_hmove == 0 && t_vmove == 0)
        {
            HeadBob(idleCounter, 0.025f, 0.025f);
            idleCounter += Time.deltaTime;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
        }
        else if(!isSprinting)
        {
            HeadBob(movementCounter, 0.03f, 0.03f);
            movementCounter += Time.deltaTime * 2.5f;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 3f);
        }
        else
        {
            HeadBob(movementCounter, 0.07f, 0.05f);
            movementCounter += Time.deltaTime * 4f;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 5f);
        }

        //UI Refreshes
        RefreshHealthBar();
        weapon.RefreshAmmo(ui_ammo);

    }
    void FixedUpdate()
    {

        if (!photonView.IsMine) return;

        float t_hmove = Input.GetAxisRaw("Horizontal");
        float t_vmove = Input.GetAxisRaw("Vertical");


        bool sprint = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetKeyDown(KeyCode.Space);

        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
        bool isJumping = jump;
        bool isSprinting = sprint && t_vmove > 0 && !isJumping;


    



        Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
        t_direction.Normalize();


        float t_adjustedSpeed = speed;
        if (isSprinting) t_adjustedSpeed *= sprintModifier;


        Vector3 t_targetVelocity = transform.TransformDirection(t_direction) * t_adjustedSpeed * Time.deltaTime;
        t_targetVelocity.y = rig.velocity.y;
        rig.velocity = t_targetVelocity;


        if (isSprinting) { normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 5f); }
        else { normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 5f); }
    }
    #endregion

    #region Private Methods

    void HeadBob (float p_z, float p_x_intensity, float p_y_intensity)
    {
        targetWeaponBobPosition = weaponParentOrigin + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
    }

    void RefreshHealthBar ()
    {
        float t_health_ratio = (float)current_health / (float)max_health;
        ui_healthbar.localScale = Vector3.Lerp(ui_healthbar.localScale, new Vector3(t_health_ratio, 1, 1), Time.deltaTime * 8f);
    }

    #endregion

    #region Public Methods

    
    public void TakeDamage (int p_damage)
    {
       if(photonView.IsMine)
        { 
            current_health -= p_damage;
            
            RefreshHealthBar();
           

           if(current_health <= 0)
          {
                SoundManager.sndMan.PlayDeathSound();
                DeathCounter.death += deathValue;
                manager.Spawn();
                PhotonNetwork.Destroy(gameObject);

                
            }

      
            //mAudioSrc.Play();
        }
        
    }
}


#endregion

