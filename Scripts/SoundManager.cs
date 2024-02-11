using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[PunRPC]
public class SoundManager : MonoBehaviour
{
    [PunRPC]
    public static SoundManager sndMan;

    private AudioSource audioSrc;
    private AudioClip[] missSounds;
    private AudioClip[] hitSounds;
    private AudioClip[] reloadSounds;
    private AudioClip[] magazineSounds;
    private AudioClip[] emptySounds;
    private AudioClip[] deathSounds;

    private int randomHitSound;
    private int randomMissSound;
    private int randomReloadSound;
    private int randomMagazineSound;
    private int randomEmptySound;
    private int randomDeathSound;
    

    [PunRPC]
    // Start is called before the first frame update
    void Start()
    {
        sndMan = this;
        audioSrc = GetComponent<AudioSource>();
        hitSounds = Resources.LoadAll<AudioClip>("HitSounds");
        missSounds = Resources.LoadAll<AudioClip>("MissSounds");
        reloadSounds = Resources.LoadAll<AudioClip>("ReloadSounds");
        magazineSounds = Resources.LoadAll<AudioClip>("MagazineSounds");
        emptySounds = Resources.LoadAll<AudioClip>("EmptySounds");
        deathSounds = Resources.LoadAll<AudioClip>("DeathSounds");
        
    }
    [PunRPC]
    //Update is called once per frame
    public void PlayHitSound()
    {
        randomHitSound = Random.Range(0, hitSounds.Length);
        audioSrc.PlayOneShot(hitSounds[randomHitSound]);
    }
    public void PlayMissSound()
    {
        randomMissSound = Random.Range(0, missSounds.Length);
        audioSrc.PlayOneShot(missSounds[randomMissSound]);
    }

    public void PlayReloadSound()
    {
        randomReloadSound = Random.Range(0, reloadSounds.Length);
        audioSrc.PlayOneShot(reloadSounds[randomReloadSound]);
    }

    public void PlayMagazineSound()
    {
        randomMagazineSound = Random.Range(0, magazineSounds.Length);
        audioSrc.PlayOneShot(magazineSounds[randomMagazineSound]);
    }
    public void PlayEmptySound()
    {
        randomEmptySound = Random.Range(0, emptySounds.Length);
        audioSrc.PlayOneShot(emptySounds[randomEmptySound]);
    }
    public void PlayDeathSound()
    {
        randomDeathSound = Random.Range(0, deathSounds.Length);
        audioSrc.PlayOneShot(deathSounds[randomDeathSound]);
    }

}
