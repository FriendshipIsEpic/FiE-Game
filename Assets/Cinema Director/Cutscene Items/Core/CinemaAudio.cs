using System.Collections.Generic;
// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Audio", "Play Audio", typeof(AudioClip), CutsceneItemGenre.AudioClipItem)]
    public class CinemaAudio : TimelineActionFixed
    {
        private bool wasPlaying = false;

        public void Trigger(){
        }

        public void End()
        {
            Stop();
        }

        public void UpdateTime(float time, float deltaTime)
        {
            AudioSource audio = gameObject.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.mute = false;
                if (Cutscene.State == CinemaDirector.Cutscene.CutsceneState.Scrubbing)
                {
                    audio.time = time + InTime;
                }
                if (!audio.isPlaying)
                {
                    audio.time = time + InTime;
                    audio.Play();
                }
            }
        }

        public void Resume()
        {
            AudioSource audio = gameObject.GetComponent<AudioSource>();
            if (audio != null)
            {
                if (wasPlaying)
                {
                    audio.Play();
                }
            }
        }

        public void Pause()
        {
            AudioSource audio = gameObject.GetComponent<AudioSource>();
            if (audio != null)
            {
                wasPlaying = false;
                if (audio.isPlaying)
                {
                    wasPlaying = true;
                }
                
                audio.Pause();
            }
        }

        public override void Stop()
        {
            AudioSource audio = gameObject.GetComponent<AudioSource>();
            if (audio)
                audio.Stop();
        }

        public void SetTime(float audioTime)
        {
            AudioSource audio = gameObject.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.time = audioTime;
            }
        }

        public override void SetDefaults(Object PairedItem)
        {
            AudioClip clip = PairedItem as AudioClip;
            if (clip != null)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = clip;

                Firetime = 0;
                Duration = clip.length;
                InTime = 0;
                OutTime = clip.length;
                ItemLength = clip.length;
                source.playOnAwake = false;
            }
        }
    }
}