using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioClip roar1;
    public AudioClip roar2;
    public AudioClip attack;
    public AudioClip death;
    public AudioClip hurt;
    public AudioClip spell;

    private AudioSource audioSource;

    void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRoarOne()
    {
        audioSource.PlayOneShot(roar1);
    }

    public void PlayRoarTwo()
    {
        audioSource.PlayOneShot(roar2);
    }

    public void PlayAttack()
    {
        audioSource.PlayOneShot(attack);
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(death);
    }

    public void PlayHurt()
    {
        audioSource.PlayOneShot(hurt);
    }

    public void PlaySpell()
    {
        audioSource.PlayOneShot(spell);
    }
}
