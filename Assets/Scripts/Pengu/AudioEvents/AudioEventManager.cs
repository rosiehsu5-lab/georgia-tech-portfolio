using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioEventManager : MonoBehaviour
{

    public EventSound3D eventSound3DPrefab;
    public EventSound3D currentSlideSound;
    public AudioClip[] snowFootstepAudio;
    public AudioClip jumpAudio;
    public AudioClip slideAudio;
    public AudioClip interactAudio;
    public AudioClip deathAudio;
    public AudioClip respawnAudio;


    private UnityAction<Vector3> snowFootstepEventListener;
    private UnityAction<Vector3> jumpEventListener;
    private UnityAction<Vector3> slideEventListener;

    private UnityAction<Vector3> interactEventListener;
    private UnityAction<Vector3> deathEventListener;
    private UnityAction<Vector3> respawnEventListener;


    void Awake()
    {

        jumpEventListener = new UnityAction<Vector3>(jumpEventHandler);
        snowFootstepEventListener = new UnityAction<Vector3>(snowFootstepEventHandler);
        slideEventListener = new UnityAction<Vector3>(PlaySlide);
        interactEventListener = new UnityAction<Vector3>(PlayInteract);
        deathEventListener = new UnityAction<Vector3>(PlayDeath);
        respawnEventListener = new UnityAction<Vector3>(PlayRespawn);
    }

    void OnEnable()
    {

        EventManager.StartListening<JumpEvent, Vector3>(jumpEventListener);
        EventManager.StartListening<SnowFootstepEvent, Vector3>(snowFootstepEventListener);
        EventManager.StartListening<SlideEvent, Vector3>(slideEventListener);
        EventManager.StartListening<InteractEvent, Vector3>(interactEventListener);
        EventManager.StartListening<DeathEvent, Vector3>(deathEventListener);
        EventManager.StartListening<RespawnEvent, Vector3>(respawnEventListener);
    }

    void OnDisable()
    {
        EventManager.StopListening<JumpEvent, Vector3>(jumpEventListener);
        EventManager.StopListening<SnowFootstepEvent, Vector3>(snowFootstepEventListener);
        EventManager.StopListening<SlideEvent, Vector3>(slideEventListener);
        EventManager.StopListening<InteractEvent, Vector3>(interactEventListener);
        EventManager.StopListening<DeathEvent, Vector3>(deathEventListener);
        EventManager.StopListening<RespawnEvent, Vector3>(respawnEventListener);

    }

    void jumpEventHandler(Vector3 pos)
    {
        //AudioSource.PlayClipAtPoint(this.explosionAudio, worldPos, 1f);

        if (eventSound3DPrefab)
        {

            EventSound3D snd = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);

            snd.audioSrc.clip = this.jumpAudio;

            snd.audioSrc.minDistance = 5f;
            snd.audioSrc.maxDistance = 100f;

            snd.audioSrc.Play();
        }
    }

    void snowFootstepEventHandler(Vector3 pos)
    {

        EventSound3D snd = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);

        snd.audioSrc.clip = this.snowFootstepAudio[Random.Range(0, snowFootstepAudio.Length)];

        snd.audioSrc.minDistance = 5f;
        snd.audioSrc.maxDistance = 100f;

        snd.audioSrc.Play();

    }

    void PlaySlide(Vector3 pos)
    {
        if(eventSound3DPrefab)
        {
            if (currentSlideSound != null) return;
            currentSlideSound = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);
            
            currentSlideSound.audioSrc.clip = this.slideAudio;
            currentSlideSound.audioSrc.loop = true;
            currentSlideSound.audioSrc.minDistance = 5f;
            currentSlideSound.audioSrc.maxDistance = 100f;
            currentSlideSound.audioSrc.Play();
            Destroy(currentSlideSound.gameObject, 1.0f);
        }
    }

    void PlayInteract(Vector3 pos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);
        snd.audioSrc.clip = this.interactAudio;
        snd.audioSrc.minDistance = 5f;
        snd.audioSrc.maxDistance = 100f;
        snd.audioSrc.Play();
    }

    void PlayDeath(Vector3 pos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);
        snd.audioSrc.clip = this.deathAudio;
        snd.audioSrc.minDistance = 5f;
        snd.audioSrc.maxDistance = 100f;
        snd.audioSrc.Play();
    }

    void PlayRespawn(Vector3 pos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);
        snd.audioSrc.clip = this.respawnAudio;
        snd.audioSrc.minDistance = 5f;
        snd.audioSrc.maxDistance = 100f;
        snd.audioSrc.Play();
    }
}
