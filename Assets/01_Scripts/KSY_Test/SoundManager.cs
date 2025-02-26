using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private AudioClip[] audioClips;
    private Dictionary<string, bool> sfxPlaying = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioClips();
        }
        else Destroy(gameObject);
    }

    private void LoadAudioClips()
    {
        audioClips = Resources.LoadAll<AudioClip>("Audio");
        Debug.Log("총 " + audioClips.Length + "개의 사운드 클립 로딩 완료.");
    }

    public void PlayBgm(string musicName, GameObject target)
    {
        AudioSource source = target.GetComponent<AudioSource>();
        if (source == null) source = target.AddComponent<AudioSource>();

        AudioClip clip = Array.Find(audioClips, a => a.name == musicName);
        if (clip != null)
        {
            if (source.isPlaying) source.Stop();
            source.clip = clip;
            source.loop = true;
            source.Play();
        }
        else Debug.LogWarning("음악 클립을 찾을 수 없습니다: " + musicName);
    }

    public void PlaySFX(string sfxName, GameObject target)
    {
        AudioClip clip = Array.Find(audioClips, a => a.name == sfxName);
        if (clip == null)
        {
            Debug.LogWarning("효과음 클립을 찾을 수 없습니다: " + sfxName);
            return;
        }

        // 중복 재생 방지: 같은 효과음이 이미 재생 중이면 실행하지 않음.
        if (sfxPlaying.TryGetValue(sfxName, out bool isPlaying) && isPlaying)
        {
            Debug.Log("이미 해당 효과음이 재생 중입니다: " + sfxName);
            return;
        }

        sfxPlaying[sfxName] = true;

        AudioSource source = target.GetComponent<AudioSource>();
        if (source == null) source = target.AddComponent<AudioSource>();

        source.PlayOneShot(clip);
        StartCoroutine(ResetSFXFlag(sfxName, clip.length));
    }

    private IEnumerator ResetSFXFlag(string sfxName, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfxPlaying[sfxName] = false;
    }
}
