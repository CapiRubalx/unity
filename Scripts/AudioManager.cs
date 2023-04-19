using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Make it a singleton class that can be accessible everywhere
   
    [SerializeField]
    Sound[] m_sounds;

    void Start() {
        for(int i = 0; i < m_sounds.Length; i++) {
            GameObject go = new GameObject("Sound_" + i + "_" + m_sounds[i].m_name);
            go.transform.SetParent(transform);
            m_sounds[i].SetSource(go.AddComponent<AudioSource>());
        }
    }

    public void ReproducirSonido( string audio ) {
        for( int i = 0; i < m_sounds.Length; i++ ) {
            if( m_sounds[i].m_name == audio ) {
                m_sounds[i].Play();
                return;
            }
        }

        Debug.LogWarning("AudioManager: Sound name not found in list: " + name);
    }

    private void Awake()
    {
        if( Instance == null ) Instance = this;
        else Debug.Log( "Cuidado! Más de un AudioManager en escena." );
    }
}


[System.Serializable]
public class Sound
{
    public string m_name;
    public AudioClip[] m_clips;

    [Range(0f, 1f)]
    public float volume = 1.0f;
    [Range(0f, 1.5f)]
    public float pitch = 1.0f;

    public Vector2 m_randomVolumeRange = new Vector2(1.0f, 1.0f);
    public Vector2 m_randomPitchRange = new Vector2(1.0f, 1.0f);

    private AudioSource m_source;

    public void SetSource(AudioSource source)
    {
        m_source = source;
        int randomClip = Random.Range(0, m_clips.Length - 1);
        m_source.clip = m_clips[randomClip];
    }

    public void Play()
    {
        if(m_clips.Length > 1) {
            int randomClip = Random.Range(0, m_clips.Length - 1);
            m_source.clip = m_clips[randomClip];
        }

        m_source.volume = volume * Random.Range( m_randomVolumeRange.x, m_randomVolumeRange.y );
        m_source.pitch  =  pitch * Random.Range( m_randomPitchRange.x, m_randomPitchRange.y );
        m_source.Play();
    }
}