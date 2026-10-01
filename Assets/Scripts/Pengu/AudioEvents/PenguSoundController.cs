using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguSoundController : MonoBehaviour
{
    private PenguNoiseController penguNoise;

    void Start()
    {
        penguNoise = GetComponent<PenguNoiseController>();
    }

    public void ExecuteFootstep()
    {
        EventManager.TriggerEvent<SnowFootstepEvent, Vector3>(transform.position);
        penguNoise?.MakeNoise(penguNoise.footstepNoiseRadius);
    }

    public void PlayJumpSound()
    {
        EventManager.TriggerEvent<JumpEvent, Vector3>(transform.position);
        penguNoise?.MakeNoise(penguNoise.jumpNoiseRadius);
    }

    public void PlaySlideSound()
    {
        EventManager.TriggerEvent<SlideEvent, Vector3>(transform.position);
        penguNoise?.MakeNoise(penguNoise.slideNoiseRadius);
    }

    public void PlayInteractSound()
    {
        EventManager.TriggerEvent<InteractEvent, Vector3>(transform.position);
    }

    public void PlayDeathSound()
    {
        EventManager.TriggerEvent<DeathEvent, Vector3>(transform.position);
    }

    public void PlayRespawnSound()
    {
        EventManager.TriggerEvent<RespawnEvent, Vector3>(transform.position);
    }
}