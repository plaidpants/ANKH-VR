using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.TTS;
using Meta.WitAi.TTS.Data;
using System.Linq;

public class VolumeControl : MonoBehaviour
{
    public TTSSpeaker speaker;
    public AudioSource music;
    public AudioSource soundEffects;
    public float musicVolumeWhenSpeaking;
    public float musicVolumeWhenNotSpeaking;
    public float soundEffectsVolumeWhenSpeaking;
    public float soundEffectsVolumeWhenNotSpeaking;

    // Update is called once per frame
    void Update()
    {
        if (speaker.IsSpeaking)
        {
            music.volume = musicVolumeWhenSpeaking;
            soundEffects.volume = soundEffectsVolumeWhenSpeaking;
        }
        else
        {
            music.volume = musicVolumeWhenNotSpeaking;
            soundEffects.volume = soundEffectsVolumeWhenNotSpeaking;
        }
    }
}
