using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource effectsSource;
    
    // Funcion para ejecutar audios en el AudioSource de la camara
    public void PlaySound(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }
}