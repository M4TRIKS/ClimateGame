using UnityEngine;
using UnityEngine.UI;

// makes sure the object has an Image component
[RequireComponent(typeof(Image))]
public class UIImageAnimator : MonoBehaviour
{
    [Header("References")]

    //image that will display the animation
    [SerializeField] private Image _image;

    [Header("Animation Frames")]

    // all sprites / inside a array
    [SerializeField] private Sprite[] _frames;

    [Header("Settings")]

    // frame time
    [SerializeField] private float _frameRate = 0.12f;

    [SerializeField] private bool _playOnAwake = true;

    private int _currentFrame;

    //time between frames
    private float _timer;

    // checks if animation is currently playing
    private bool _playing;

    void Awake()
    {
        // start animation automatically if enabled
        if (_playOnAwake)
            Play();
    }

    void Update()
    {
        // stop if animation is not playing
        if (!_playing) return;

        // stop if no frames assigned
        if (_frames == null || _frames.Length == 0) return;

        // add real time (works even if game is paused)
        _timer += Time.unscaledDeltaTime;

        // when enough time passes, change frame
        if (_timer >= _frameRate)
        {
            _timer = 0f;
            _currentFrame++;

            // if reached last frame, go back to first
            if (_currentFrame >= _frames.Length)
                _currentFrame = 0;

            // change image sprite
            _image.sprite = _frames[_currentFrame];
        }
    }

    // starts the animation
    public void Play()
    {
        // if no frames, do nothing
        if (_frames == null || _frames.Length == 0) return;

        _playing = true;

        // start from first frame
        _currentFrame = 0;
        _timer = 0f;

        // show first frame instantly
        _image.sprite = _frames[_currentFrame];
    }

    // stops the animation
    public void Stop()
    {
        _playing = false;
    }
}