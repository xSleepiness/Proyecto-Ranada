using System.Collections;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Script.Interface;
using UnityEngine;
using UnityEngine.Networking;

namespace Script.Notes
{
  public class SongManager : MonoBehaviour
  {
    public static SongManager Instance;
    public AudioSource audioSource;
    [SerializeField] private Lane[] lanes;
    public float songDelayInSeconds;
    public double marginOfError; // in seconds

    public int inputDelayInMilliseconds;
    
    [SerializeField] private string fileLocation;
    
    public float noteTime;
    public float noteSpawnY;
    public float noteTapY;

    public float noteDespawnY { get { return noteTapY - (noteSpawnY - noteTapY); } }

    public static MidiFile midiFile;

    void Start()
    {
      Instance = this;
      if (Application.dataPath.StartsWith("http://") || Application.dataPath.StartsWith("https://"))
      {
        StartCoroutine(ReadFromWebsite());
      }
      else
      {
        ReadFromFile();
      }
    }

    private IEnumerator ReadFromWebsite()
    {
      using (UnityWebRequest request = UnityWebRequest.Get(Application.dataPath + "/Audio/MIDI_Files" + fileLocation))
      {
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
          Debug.LogError(request.error);
        }
        else
        {
          byte[] results = request.downloadHandler.data;
          using (var stream = new MemoryStream(results))
          {
            midiFile = MidiFile.Read(stream);
            GetDataFromMidi();
          }
        }
      }
    }

    private void ReadFromFile()
    {
      midiFile = MidiFile.Read(Application.dataPath + "/Audio/MIDI_Files/" + fileLocation);
      GetDataFromMidi();
    }

    public void GetDataFromMidi()
    {
      var notes = midiFile.GetNotes();
      var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
      notes.CopyTo(array, 0);

      foreach (var lane in lanes) lane.SetTimeStamps(array);

      Invoke(nameof(StartSong), songDelayInSeconds);
    }

    public void StartSong()
    {
      if (SoundManager.Instance != null)
      {
        AudioSource soundManagerAudioSource = SoundManager.Instance.GetAudioSource();

        soundManagerAudioSource.Play();
      }
      else
      {
        Debug.LogError("SoundManager no se ha inicializado");
      }
    }

    public static double GetAudioSourceTime()
    {
      return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }
  }
}