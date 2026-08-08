using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Audio Manager providing sound effect triggers for coin collection, collisions, victory,
    /// jump sound effects, and explosive vehicle blast sound effects.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Clips")]
        public AudioClip coinPickupClip;
        public AudioClip obstacleHitClip;
        public AudioClip jumpClip;
        public AudioClip victoryClip;
        public AudioClip explosionClip;

        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayCoinSound()
        {
            if (coinPickupClip != null)
            {
                audioSource.PlayOneShot(coinPickupClip, 0.8f);
            }
            else
            {
                PlayTone(880f, 0.15f, 0.5f);
            }
        }

        public void PlayObstacleHitSound()
        {
            if (obstacleHitClip != null)
            {
                audioSource.PlayOneShot(obstacleHitClip, 0.9f);
            }
            else
            {
                PlayTone(150f, 0.2f, 0.7f);
            }
        }

        public void PlayJumpSound()
        {
            if (jumpClip != null)
            {
                audioSource.PlayOneShot(jumpClip, 0.75f);
            }
            else
            {
                // Play synthesized cartoon rising pitch jump sweep
                PlayJumpSweep(300f, 650f, 0.2f, 0.5f);
            }
        }

        public void PlayExplosionSound()
        {
            if (explosionClip != null)
            {
                audioSource.PlayOneShot(explosionClip, 1.0f);
            }
            else
            {
                PlayExplosionNoise(0.6f, 0.9f);
            }
        }

        public void PlayVictorySound()
        {
            if (victoryClip != null)
            {
                audioSource.PlayOneShot(victoryClip, 1.0f);
            }
            else
            {
                PlayTone(523.25f, 0.3f, 0.6f);
            }
        }

        private void PlayTone(float frequency, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = 1.0f - (t / duration);
                samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * envelope * volume;
            }

            AudioClip synthClip = AudioClip.Create("SynthTone", sampleCount, 1, sampleRate, false);
            synthClip.SetData(samples, 0);
            audioSource.PlayOneShot(synthClip);
        }

        private void PlayJumpSweep(float startFreq, float endFreq, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, t / duration);
                float envelope = 1.0f - (t / duration);
                samples[i] = Mathf.Sin(2 * Mathf.PI * currentFreq * t) * envelope * volume;
            }

            AudioClip sweepClip = AudioClip.Create("JumpSweep", sampleCount, 1, sampleRate, false);
            sweepClip.SetData(samples, 0);
            audioSource.PlayOneShot(sweepClip);
        }

        private void PlayExplosionNoise(float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            System.Random rand = new System.Random();
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = Mathf.Pow(1.0f - (t / duration), 2.5f);
                float noise = (float)(rand.NextDouble() * 2.0 - 1.0);
                samples[i] = noise * envelope * volume;
            }

            AudioClip blastClip = AudioClip.Create("ExplosionBlast", sampleCount, 1, sampleRate, false);
            blastClip.SetData(samples, 0);
            audioSource.PlayOneShot(blastClip);
        }
    }
}
