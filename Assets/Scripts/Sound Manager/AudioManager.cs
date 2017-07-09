using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    AudioSource audioSource;

    public void Initialize()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Audio/AudioSourcePrefab"));
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(0, 0, 0);
        audioSource = go.GetComponent<AudioSource>();
    }


    public void PlaySoundEffect(string clipName)
    {
        if (!GV.AUDIO)
            return;
        string path = "Audio/" + clipName;
        AudioClip audioClip = Resources.Load<AudioClip>(path);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
        else
        {
            Debug.Log("Failed to play sound effect: " + path);
        }
    }


    public void PlaySound(string clipName, bool looping)
    {
        if (!GV.AUDIO)
            return;

        string path = "Audio/" + clipName;
        AudioClip audioClip = Resources.Load<AudioClip>(path);
        if (audioClip != null)
        {
            audioSource.loop = looping;
            audioSource.clip = audioClip;
            audioSource.Play();
           // Debug.Log("successfully played: " + path);
        }
        else
        {
            Debug.Log("Failed to play audio: " + path);
        }
    }

    public void PlayRandomSoundFromFolder(string folderPath, bool isLooping, bool isSoundEffect)
    {
        if (!GV.AUDIO)
            return;

        List<string> fileNames = XMLEncoder.GetAllFilesInPath("Assets/Resources/Audio/" + folderPath);
        if(fileNames.Count == 0)
        {
            Debug.Log("no audio " + "Assets/Resources/Audio/" + folderPath);
            return;
        }
        int rando = Random.Range(0, fileNames.Count);
        string soundPath = folderPath + "/" + fileNames[rando];
        if (isSoundEffect)
            PlaySoundEffect(soundPath);
        else
            PlaySound(soundPath, isLooping);
    }
}
