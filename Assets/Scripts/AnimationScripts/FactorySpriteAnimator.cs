using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FactorySpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    private Coroutine _animationCoroutine;

    void Awake()
    {
        if (_renderer == null)
            _renderer = GetComponent<SpriteRenderer>();
    }

    public void ApplyLevelVisuals(FactoryLevelData levelData)
    {
        StopAnimation();

        if (_renderer == null || levelData == null)
            return;

        // If the level has animation frames, play them
        if (levelData.animationFrames != null && levelData.animationFrames.Length > 0)
        {
            _animationCoroutine = StartCoroutine(PlayAnimation(levelData.animationFrames, levelData.animationFrameRate));
        }
        // Otherwise use the static sprite
        else if (levelData.sprite != null)
        {
            _renderer.sprite = levelData.sprite;
            _renderer.color = Color.white;
        }
    }

    private IEnumerator PlayAnimation(Sprite[] frames, float frameRate)
    {
        int frameIndex = Random.Range(0, frames.Length);

        while (true)
        {
            _renderer.sprite = frames[frameIndex];
            _renderer.color = Color.white;

            frameIndex = (frameIndex + 1) % frames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }

    private void StopAnimation()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }
}