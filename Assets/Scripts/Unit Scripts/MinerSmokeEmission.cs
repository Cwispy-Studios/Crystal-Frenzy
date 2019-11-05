using UnityEngine;

public class MinerSmokeEmission : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem minorSmoke = null, majorSmoke = null, majorSparks = null;

  private Health minerHealth = null;

  [FMODUnity.EventRef]
  public string sparkSound = "";
  private FMOD.Studio.EventInstance sparkInstance;

  private void Awake()
  {
    minorSmoke.Stop();
    majorSmoke.Stop();
    majorSparks.Stop();

    minerHealth = GetComponent<Health>();
    sparkInstance = FMODUnity.RuntimeManager.CreateInstance(sparkSound);
    FMODUnity.RuntimeManager.AttachInstanceToGameObject(sparkInstance, transform, GetComponent<Rigidbody>());
  }

  private void Update()
  {
    float healthPct = minerHealth.CurrentHealth / minerHealth.MaxHealth;

    FMODUnity.RuntimeManager.AttachInstanceToGameObject(sparkInstance, transform, GetComponent<Rigidbody>());

    FMOD.Studio.PLAYBACK_STATE playbackState;

    sparkInstance.getPlaybackState(out playbackState);

    if (healthPct >= 0.6f)
    {
      minorSmoke.Stop();
      majorSmoke.Stop();
      majorSparks.Stop();
    }

    else if (healthPct >= 0.25f && healthPct < 0.6f)
    {
      if (!minorSmoke.isPlaying)
      {
        minorSmoke.Play();
      }
      
      majorSmoke.Stop();
      majorSparks.Stop();
    
      if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
      {
        sparkInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
      }
    }

    else
    {
      minorSmoke.Stop();

      if (!majorSmoke.isPlaying)
      {
        majorSmoke.Play();
      }

      if (!majorSparks.isPlaying)
      {
        majorSparks.Play();
      }

      if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
      {
        sparkInstance.start();
      }
    }
  }

  private void OnDestroy()
  {
    sparkInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
  }
}
