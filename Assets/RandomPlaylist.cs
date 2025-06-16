using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomPlaylist : MonoBehaviour
{
    public AudioSource audioSource;      // Reference to the AudioSource component
    public List<AudioClip> playlist;     // List of songs (drag and drop your audio clips in the Inspector)
    private Queue<AudioClip> shuffledPlaylist;  // Queue to hold the shuffled playlist

    private void Awake()
    {
        //StartCoroutine(LoadAudioFiles());
    }

    void Start()
    {
        if (playlist[0] != null)
        {
            ShufflePlaylist();  // Shuffle and prepare the playlist
            PlayNextSong();     // Start playing
        }
    }

    void Update()
    {
        // Check if the current song is done playing
        if (!audioSource.isPlaying && shuffledPlaylist.Count > 0)
        {
            PlayNextSong();
        }
        else
        {
            ShufflePlaylist();
        }
    }

    // Shuffle the playlist
    void ShufflePlaylist()
    {
        List<AudioClip> tempPlaylist = new List<AudioClip>(playlist);  // Copy the original playlist
        shuffledPlaylist = new Queue<AudioClip>();  // Queue to store shuffled songs

        while (tempPlaylist.Count > 0)
        {
            int randomIndex = Random.Range(0, tempPlaylist.Count);
            shuffledPlaylist.Enqueue(tempPlaylist[randomIndex]);  // Add a random song to the queue
            tempPlaylist.RemoveAt(randomIndex);  // Remove that song from the temporary list
        }
    }

    // Play the next song from the shuffled playlist
    public void PlayNextSong()
    {
        if (shuffledPlaylist.Count > 0)
        {
            audioSource.clip = shuffledPlaylist.Dequeue();  // Get the next song
            audioSource.Play();  // Play it
        }
    }

    IEnumerator LoadAudioFiles()
    {
        string path = Application.streamingAssetsPath;
        DirectoryInfo info = new DirectoryInfo(path);
        FileInfo[] files = info.GetFiles("*.mp3");

        foreach (FileInfo file in files)
        {
            string audioPath = "file://" + file.FullName;
            using(var www = new WWW(audioPath))
            {
                yield return www;
                AudioClip audioClip = www.GetAudioClip(false, false);
                playlist.Add(audioClip);
            }
        }
    }
}
