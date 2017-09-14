using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public HmdRecorder hmdRecorder;
    public AudioListenerRecorder listenerRecorder;

    private string[] microphoneNames = new string[2];

    private bool isRecording = false;
    private string folderPath;
    private AudioClip[] clips = new AudioClip[2];
    public int lengthSeconds = 1;

    public AudioSource audioOrigin;
    private string audioSourcePath;
    private AudioClip audioSourceClip = null;
    private bool shouldLoop = true;

    public List<string> GetMicrophoneDevices()
    {
        return new List<string>(Microphone.devices);
    }

    public void SetMicrophoneName(int channel, string name)
    {
        if (channel < microphoneNames.Length && !(channel < 0))
        {
            microphoneNames[channel] = name;
        }
    }

    public void ToggleRecording()
    {
        if (!isRecording)
        {
            if (audioSourceClip == null)
            {
                EditorUtility.DisplayDialog("Audio source not set", "An audio source file must be set to start recording. Check that you have selected a file and that it is of .wav format.", "OK");
                return;
            }

            Debug.Log("Started a recording...");

            // Define a folder to save files to
            folderPath = EditorUtility.SaveFolderPanel("Select folder to save files", "", "");

            Debug.Log("Save folder: " + folderPath);

            // Start recording a clip for each selected device (same devices cannot be recorded to two clips simultaneously)
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i] = Microphone.Start(microphoneNames[i], false, lengthSeconds, AudioSettings.outputSampleRate);
            }

            listenerRecorder.StartRecording(lengthSeconds, 2, AudioSettings.outputSampleRate);

            audioOrigin.clip = audioSourceClip;
            audioOrigin.loop = shouldLoop;
            audioOrigin.Play();

            hmdRecorder.ToggleRecording();

            isRecording = true;

            StartCoroutine(EndRecordingAfterSeconds(lengthSeconds));
        }
        else
        {
            audioOrigin.Stop();

            // End all recordings
            for (int i = 0; i < clips.Length; i++)
            {
                Microphone.End(microphoneNames[i]);
            }

            hmdRecorder.ToggleRecording();

            AudioClip[] listenerClips = listenerRecorder.StopRecording();

            // Save audio recordings
            for (int i = 0; i < clips.Length; i++)
            {
                SavWav.Save(folderPath, "audio" + (i+1), clips[i]);
                SavWav.Save(folderPath, "source" + (i + 1), listenerClips[i]);
            }

            // Save hmd recording
            Vector3[] hmdRotations = hmdRecorder.GetArrayOfSize(clips[0].samples, AudioSettings.outputSampleRate);
            hmdRecorder.SaveArrayAsCsv(hmdRotations, folderPath);

            isRecording = false;
        }
    }

    public bool GetIsRecording()
    {
        return isRecording;
    }

    public void SetAudiosourceFilePath(string path)
    {
        audioSourcePath = path;
        Debug.Log("Source audio file: " + audioSourcePath);

        WWW audioLoader = new WWW("file:///" + audioSourcePath);
        while (!audioLoader.isDone)
        {
            Debug.Log("Reading audio source file...");
        }

        audioSourceClip = audioLoader.GetAudioClip(false, false, AudioType.WAV);
    }

    public void SetLooping(bool newShouldLoop)
    {
        shouldLoop = newShouldLoop;
    }

    IEnumerator EndRecordingAfterSeconds(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        ToggleRecording();

        GuiController gui = FindObjectOfType<GuiController>();
        gui.ToggleRecordingButton(false);
    }
}
