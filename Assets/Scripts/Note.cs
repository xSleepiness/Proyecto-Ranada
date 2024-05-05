using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime(); // Get the current time of the audio source
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated; // Get the time since the note was instantiated
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(Vector3.right * SongManager.Instance.noteSpawnY, Vector3.right * SongManager.Instance.noteDespawnY, t); 
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
