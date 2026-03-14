using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    /// <summary>
    /// /////////////////TIMEEEEE
    /// 3 TIMES 60 seconds
    /// </summary>
    private float  timeDuration = 3f *60f;
    /// <summary>
    /// //////
    /// </summary>
    private float timer;
    [SerializeField] private GameManager _gameManager;
    private bool timeIsOver = false;

[Header("Timer")]
    [SerializeField] private TextMeshProUGUI firstMinute;
    [SerializeField] private TextMeshProUGUI secondMinute;
    [SerializeField] private TextMeshProUGUI separator;
    [SerializeField] private TextMeshProUGUI firstSecond;
    [SerializeField] private TextMeshProUGUI secondSecond;
[Header("Audio Timer")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip warningSound;
    // checks if the audio has being played
    private bool hasPlayedWarning = false;
    private float flashTimer;
    //how fast the timer is going
    private float flashDuration = 1f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //starts with the full time
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
      if (!_gameManager.IsGameEnded)
        {
            
            //counts down if there is time
            if(timer > 0)
            {
                //substracts time
                timer -= Time.deltaTime;
                //send that time update timer
                UpdateTimerDisplay(timer);
                        //20 second 
                // If time drops to 20 or below AND we haven't played the sound yet
                if (timer <= 20f && !hasPlayedWarning)
                {
                    hasPlayedWarning = true; // Flip the flag so this block never runs again
                    audioSource.clip = warningSound;
                    audioSource.Play(); 
                }
            }
        else
            {
                if (!timeIsOver)
                {
                    timeIsOver = true;
                    _gameManager.TimeUp();
                }

                Flash();
            }
        } 
        
    }
    private void ResetTimer()
    {
        timer = timeDuration;
        // Resetin case the game restarts
        hasPlayedWarning = false; 
        
        // stop the sound if the timer is reset early
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); 
        }
    }
    private void UpdateTimerDisplay(float time)
    {//Calculate minutes : 125 seconds /60
        float minutes = Mathf.FloorToInt(time/60);
        float seconds = Mathf.FloorToInt(time % 60);
//4 digit string format 
        string currentTime = string.Format("{0:00}{1:00}", minutes,seconds);
        firstMinute.text = currentTime[0].ToString();       // changing to string using index and changing the value of the TMP
        secondMinute.text = currentTime[1].ToString();
        firstSecond.text = currentTime[2].ToString();
        secondSecond.text = currentTime[3].ToString();

    }
    private void Flash()
    {
        //exactly 0
        if(timer != 0)
        {
            timer = 0;
            UpdateTimerDisplay(timer);
        }
        // from 1 to 0 seconds of duration (flashduration is 1)
        if(flashTimer  <= 0)
        {
           flashTimer = flashDuration; 
        }
        //first alf second hide
        else if (flashTimer >= flashDuration / 2)
        {
            flashTimer -= Time.deltaTime;
            SetTextDisplay(false);
        }
        else
        {
            //second half second shows 
            flashTimer -= Time.deltaTime;
            SetTextDisplay(true);
        }
    }

private void SetTextDisplay(bool enabled)
    {
        firstMinute.enabled = enabled;
        secondMinute.enabled = enabled;

        separator.enabled = enabled;

        firstSecond.enabled = enabled;

        secondSecond.enabled = enabled;

    }
}
