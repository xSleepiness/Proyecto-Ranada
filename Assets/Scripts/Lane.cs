using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    // Floating Text Prefabs
    public GameObject PerfectPrefab;
    public GameObject GreatPrefab;
    public GameObject GoodPrefab;
    public GameObject MissPrefab;

    Vector3 floatingTextSpawnPos = new Vector3(-6.35f, 1.46f, 0f);

    // Sprite Options
    public Sprite[] spriteOptions;

    int spawnIndex = 0;
    int inputIndex = 0;

    // Coldown for the input
    bool isCooldownActive = false;
    float cooldownDuration = 0.5f;
    float cooldownTimer = 0f;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        
    }

    /**
     * Set the timestamps for the notes based on the given array of notes
     * @param array: The array of notes
     */
    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<Note>());
                note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];

                // Set the sprite of the note
                if (spriteOptions.Length > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, spriteOptions.Length);
                    note.GetComponent<SpriteRenderer>().sprite = spriteOptions[randomIndex];
                    note.GetComponent<SpriteRenderer>().sortingOrder = 14;

                }
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (Input.GetKeyDown(input))
            {
                if (Math.Abs(audioTime - timeStamp) < marginOfError)
                {
                    Hit(notes[inputIndex].gameObject.transform.position.x + 4);
                    print($"Hit on {inputIndex} note");
                    Destroy(notes[inputIndex].gameObject);
                    //StartCoroutine(DestroyAfterDelay(notes[inputIndex].gameObject, 1f));
                    inputIndex++;
                }
                else if (!isCooldownActive)
                {
                    Miss(true);
                    print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                    isCooldownActive = true;
                    cooldownTimer = cooldownDuration;
                }
            }

            if (timeStamp + marginOfError <= audioTime)
            {
                Miss(false);
                print($"Missed {inputIndex} note");
                inputIndex++;
            }

            if (isCooldownActive)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    isCooldownActive = false;
                }
            }
        }
    
    }

    /**
     * Handle the hit event when the note is hit
     * @param position: The position of the note
     */
    private void Hit(float position)
    {
        ScoreManager.Hit();
        GameObject floatingText;
        
        if (position <= 0.5 && position >= -0.5)
        {
            floatingText = Instantiate(PerfectPrefab, floatingTextSpawnPos, Quaternion.identity);
        }
        else if (position <= 1 && position >= -1)
        {
            floatingText = Instantiate(GreatPrefab, floatingTextSpawnPos, Quaternion.identity);
        }
        else
        {
            floatingText = Instantiate(GoodPrefab, floatingTextSpawnPos, Quaternion.identity);
        }

        StartCoroutine(DestroyAfterDelay(floatingText, 3f));
    }

    /**
     * Handle the miss event when the note is missed
     */
    private void Miss( bool isInputMiss )
    {
        ScoreManager.Miss(isInputMiss);
        // Floating Text for Miss
        GameObject floatingText = Instantiate(MissPrefab, floatingTextSpawnPos, Quaternion.identity);
        StartCoroutine(DestroyAfterDelay(floatingText, 3f));
    }

    /**
     * Destroy the GameObject after a delay
     * @param gameObjectToDestroy: The GameObject to destroy
     * @param delay: The delay in seconds
     */
    private IEnumerator DestroyAfterDelay(GameObject gameObjectToDestroy, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Check if the GameObject still exists before attempting to destroy it
        if (gameObjectToDestroy != null)
        {
            Destroy(gameObjectToDestroy);
        }
    }
}
