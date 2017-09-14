using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerRecorder : MonoBehaviour
{
    private AudioClip[] audioClips;
    private bool isRecording = false;

    private int desiredLength = 0;
    private int currentSample = 0;

    private float[][] tempData;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (isRecording)
        {
            if (currentSample >= desiredLength)
            {
                Debug.Log("Audioclips filled");
                isRecording = false;
                return;
            }

            // Fill temp buffers
            for (int channel = 0; channel < channels; channel++)
            {
                for (int i = 0; i < data.Length; i = i + channels)
                {
                    tempData[channel][(i / channels) + currentSample] = data[i + channel];
                }
            }

            currentSample = currentSample + data.Length / channels;
        }
    }

    public void StartRecording(int lengthSeconds, int channels, int sampleRate)
    {
        audioClips = new AudioClip[channels];
        tempData = new float[channels][];
        desiredLength = lengthSeconds * sampleRate;
        currentSample = 0;

        for (int i = 0; i < audioClips.Length; i++)
        {
            audioClips[i] = AudioClip.Create("listenerAudioChannel" + i, desiredLength, 1, sampleRate, false);
            tempData[i] = new float[desiredLength];
        }

        isRecording = true;
    }

    public AudioClip[] StopRecording()
    {
        // Fill audioclips
        for (int channel = 0; channel < audioClips.Length; channel++)
        {
            audioClips[channel].SetData(tempData[channel], 0);
        }

        isRecording = false;
        return audioClips;
    }
}
