using UnityEngine;
namespace Player
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundScript : MonoBehaviour
    {
        [SerializeField] AudioClip bigGun, heavyDamage;
        [SerializeField] AudioClip[] smallDamageSounds;
        AudioSource audioSource;
        bool playedHeavyDamageSound = false;
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        public void PlayBigGunSound()
        {
            audioSource.PlayOneShot(bigGun);
        }
        public void DamageTaken(float healthPercentage)
        {
            if (!playedHeavyDamageSound && healthPercentage <= 0.5f)
            {
                playedHeavyDamageSound = true;
                audioSource.PlayOneShot(heavyDamage, 1 - healthPercentage);
            }
            else if (smallDamageSounds.Length > 0)
            {
                int index = Random.Range(0, smallDamageSounds.Length);
                audioSource.PlayOneShot(smallDamageSounds[index]);
            }
        }
        public void PlayDamageSound()
        {
            audioSource.PlayOneShot(heavyDamage);
        }
    }
}