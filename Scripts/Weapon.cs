using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    public Gun[] loadout;
    public Transform weaponParent;
    public GameObject bulletHolePrefab;
    public LayerMask canBeShot;
    public bool isMine;
    public int scoreValue = 1;

    private float currentCooldown;
    private int currentIndex;
    private GameObject currentWeapon;

    private bool isReloading;


    



    [PunRPC]

    private void Start()
    {
        
        photonView.RPC("Equip", RpcTarget.All, 0);
        foreach (Gun a in loadout) a.Initialize();
      
    }

    void Update()
    {
        

        //if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.All, 0);  }
        //if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha2)) { photonView.RPC("Equip", RpcTarget.All, 1);  }

        if (currentWeapon != null)
        { 
        
            if(photonView.IsMine) { 
            
                    

                    if (Input.GetMouseButtonDown(0) && currentCooldown <= 0 && !isReloading)
                    {
                        if (loadout[currentIndex].FireBullet()) photonView.RPC("Shoot", RpcTarget.All);

                 


                    //mAudioSrc.Play();
                }

               
                    if (isReloading && Input.GetMouseButtonDown(0))
                {
                    SoundManager.sndMan.PlayEmptySound();
                }




          

                //cooldown
                if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
            }

        

            // if(Input.GetKeyDown(KeyCode.B)) mAudioSrc.Play();


            //weapon position elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            
        }
    }


    public void RefreshAmmo(Text p_text)
    {
        int t_clip = loadout[currentIndex].GetClip();
        int t_stash = loadout[currentIndex].GetStash();

        p_text.text = t_clip.ToString() + " / " + t_stash.ToString();

        if (Input.GetMouseButtonDown(0) && t_clip == 0)
        {
            SoundManager.sndMan.PlayEmptySound();
        }



        if (Input.GetKeyDown(KeyCode.R) && t_clip != 7 && !isReloading)
        {

            StartCoroutine(Reload(loadout[currentIndex].reload));
        }
    }


    IEnumerator Reload(float p_wait)
    {
        
        
        SoundManager.sndMan.PlayMagazineSound();
        isReloading = true;
        currentWeapon.SetActive(false);

        yield return new WaitForSeconds(p_wait);
        
        loadout[currentIndex].Reload();
        currentWeapon.SetActive(true);

        isReloading = false;
    }

   

    [PunRPC]
    void Equip(int p_ind)
    {
        if (currentWeapon != null)
        {
            if (isReloading) StopCoroutine("Reload");
            Destroy(currentWeapon);
        }


        currentIndex = p_ind;

        GameObject t_newWeapon = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        t_newWeapon.transform.localPosition = Vector3.zero;
        t_newWeapon.transform.localEulerAngles = Vector3.zero;
        t_newWeapon.GetComponent<Sway>().isMine = photonView.IsMine;

        currentWeapon = t_newWeapon;

    }

    





    void Aim (bool p_isAiming)
    {
        Transform t_anchor = currentWeapon.transform.Find("Anchor");
        Transform t_state_hip = currentWeapon.transform.Find("States/Hip");
        Transform t_state_ads = currentWeapon.transform.Find("States/ADS");

        if (p_isAiming)
        {
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime);
        }
 
    }

    
    [PunRPC]
    void Shoot ()
    {
       
            Transform t_spawn = transform.Find("Cameras/Normal Camera");

            //bloom
             Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
             t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
             t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
             t_bloom -= t_spawn.position;
             t_bloom.Normalize();
            

            //cooldown
            currentCooldown = loadout[currentIndex].firerate;
        
        //raycast
        RaycastHit t_hit = new RaycastHit();
        if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
        {
            GameObject t_newHole = Instantiate(bulletHolePrefab, t_hit.point + t_hit.normal * 0.01f, Quaternion.identity) as GameObject;
            t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
            Destroy(t_newHole, 0.25f);

            if (photonView.IsMine)
            {
                //shooting other player on network
                if(t_hit.collider.gameObject.layer == 11)
                {
                    ScoreManager.score += scoreValue;



                    
                  
                     
                    
                        SoundManager.sndMan.PlayReloadSound();
                        SoundManager.sndMan.PlayHitSound();

                    

                    
                    



                    t_hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage);
                    
                }
                if (t_hit.collider.gameObject.layer != 11)
                {
                    


                  
                    
                   
                    
                        SoundManager.sndMan.PlayMissSound();
                        SoundManager.sndMan.PlayReloadSound();
                    


                }


            }
      

         
            //gun fx

            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;

           
        }

      
    }






    [PunRPC]
 private void TakeDamage(int p_damage)
 {
      GetComponent<Player>().TakeDamage(p_damage);
 }






    
  

}