using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton instance of the ScoreManager class
    public AudioSource hitSFX; // Sound effect for a successful hit
    public AudioSource missSFX; // Sound effect for a miss
    public TMPro.TextMeshPro currentComboText; // TextMeshPro component for displaying the current combo
    public TMPro.TextMeshPro scoreText; // TextMeshPro component for displaying the score
    static int comboScore; // Current combo score
    static int Score; // Total score

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        Instance = this;
        comboScore = 0;
        Score = 0;
    }

    /**
     * Hit function to be called when a note is hit
     */
    public static void Hit()
    {
        Animator comboAnimation = Instance.currentComboText.GetComponent<Animator>();
        comboScore += 1;
        Score += 100 * (comboScore + 1);
        Instance.hitSFX.Play();

        // Play Combo animation once
        if (comboAnimation != null)
        {
            comboAnimation.Play("Combo_hit",  -1, 0f); // Play the combo animation from the beginning
        }
    }

    /**
     * Miss function to be called when a note is missed
     */
    public static void Miss()
    {
        comboScore = 0;
        Instance.missSFX.Play();
    }

    /**
     * Update is called once per frame
     */
    private void Update()
    {
        currentComboText.text = comboScore.ToString(); // Update the combo text display
        scoreText.text = Score.ToString(); // Update the score text display
    }
}
