using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Serialization;

public class Target : InteractionObject
{
    [SerializeField] private AudioClip _clipTargetUp;
    [SerializeField] private AudioClip _clipTargetDown;
    [SerializeField] private float _targetUpDelayTime = 3;

    private AudioSource _audioSource;
    private bool _isPossibleHit = true;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public override void TakeDamage(int damage)
    {
        _currentHP -= damage;

        if (_currentHP <= 0 && _isPossibleHit == true)
        {
            _isPossibleHit = false;

            StartCoroutine(nameof(OnTargetDown));
        }
    }

    private IEnumerator OnTargetDown()
    {
        _audioSource.clip = _clipTargetDown;
        _audioSource.Play();

        yield return StartCoroutine(OnAnimation(0, 90));

        StartCoroutine(nameof(OnTargetUp));
    }

    private IEnumerator OnTargetUp()
    {
        yield return new WaitForSeconds(_targetUpDelayTime);

        _audioSource.clip = _clipTargetUp;
        _audioSource.Play();

        yield return StartCoroutine(OnAnimation(90, 0));

        _isPossibleHit = true;
    }

    private IEnumerator OnAnimation(float start, float end)
    {
        float percent = 0;
        float current = 0;
        float time = 1;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.rotation = Quaternion.Slerp(
                Quaternion.Euler(start, 0, 0),
                Quaternion.Euler(end, 0, 0),
                percent
            );

            yield return null;
        }
    }
}
