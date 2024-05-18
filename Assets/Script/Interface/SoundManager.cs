using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Script.Interface
{
  public class SoundManager : MonoBehaviour
  {
    [Header("Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    
    [Header("AudioSource")]
    [SerializeField] private AudioSource bgmSource; // AudioSource para la música de fondo
    [SerializeField] private AudioSource sfxSource; // AudioSource para los efectos de sonido
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceMenu;
    
    [Header("Musica asignables")]
    [SerializeField] private AudioClip menuMUSIC; // Música del menú
    [SerializeField] private AudioClip gameMUSIC; // Música del juego
    
    [Header("Efectos de sonido")]
    [SerializeField] private AudioClip clickSOUND; // sonido de click jijija
    [SerializeField] private AudioClip voiceSOUND; // proximamente supongo
    
    public static SoundManager Instance { get; private set; }
    public AudioSource GetAudioSource() { return audioSource; }
    
    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
      }
      
      audioSource = GetComponent<AudioSource>();
      if (audioSource == null)
      {
        audioSource = gameObject.AddComponent<AudioSource>();
      }
      audioSource.clip = clickSOUND;
      SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Start()
    {
      // Valores iniciales de los sliders
      bgmSlider.value = PlayerPrefs.GetFloat("BGM", 0.8f);
      sfxSlider.value = PlayerPrefs.GetFloat("SFX", 0.8f);
      
      AudioListener.volume = bgmSlider.value;
      AudioListener.volume = sfxSlider.value;
    }

    public void ChangeBGMVolume(float value)
    {
      // Cambia el volumen de la música de fondo
      bgmSource.volume = value;
      PlayerPrefs.SetFloat("BGM", value);
      PlayerPrefs.Save();
    }

    public void ChangeSFXVolume(float value)
    {
      // Volumen de los efectos de sonido
      sfxSource.volume = value;
      PlayerPrefs.SetFloat("SFX", value);
      PlayerPrefs.Save();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      // Cambia la música de fondo dependiendo de la escena
      if (scene.name == "Menu")
      {
        audioSourceMenu.clip = menuMUSIC;
        audioSourceMenu.Play();
      }
      else if (scene.name == "Game")
      {
        bgmSource.clip = gameMUSIC;
      }
      // próximamente supongo
      // else if(scene.name == "Credits")
      // {
      //   bgmSource.clip = creditMUSIC;
      // }
    }

    public void PlayClickSound()
    {
      audioSource.PlayOneShot(clickSOUND);
    }
  }
}