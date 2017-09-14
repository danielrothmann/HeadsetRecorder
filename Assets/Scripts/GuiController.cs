using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GuiController : MonoBehaviour
{
    public AudioController audioController;
    public HmdRecorder hmdRecorder;

    public Dropdown leftMicDropdown;
    public Dropdown rightMicDropdown;
    public Slider lengthSlider;
    public Text sliderText;
    public Button recordButton;
    public Text recordButtonText;
    public Text hmdStatsText;

    // Use this for initialization
    void Start ()
	{
        UpdateMicrophoneOptions();
        UpdateMicrophoneDevices(0);
        UpdateClipLength();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    hmdStatsText.text = "x: " + hmdRecorder.GetCurrentRotation().x.ToString("F0") + "°\n" +
	                        "y: " + hmdRecorder.GetCurrentRotation().y.ToString("F0") + "°\n" +
                            "z: " + hmdRecorder.GetCurrentRotation().z.ToString("F0") + "°";
	}

    void UpdateMicrophoneOptions()
    {
        List<string> devices = audioController.GetMicrophoneDevices();
        List<Dropdown.OptionData> newOptions = new List<Dropdown.OptionData>();

        foreach (string device in devices)
        {
            newOptions.Add(new Dropdown.OptionData(device));
        }

        leftMicDropdown.options = newOptions;
        rightMicDropdown.options = newOptions;
    }

    public void UpdateMicrophoneDevices(int optionIndex)
    {
        audioController.SetMicrophoneName(0, leftMicDropdown.options[leftMicDropdown.value].text);
        audioController.SetMicrophoneName(1, rightMicDropdown.options[rightMicDropdown.value].text);
        Debug.Log("Left mic " + leftMicDropdown.options[leftMicDropdown.value].text);
        Debug.Log("Right mic " + rightMicDropdown.options[rightMicDropdown.value].text);
    }

    public void UpdateClipLength()
    {
        int length = (int)lengthSlider.value;
        audioController.lengthSeconds = length;
        sliderText.text = "Clip Length: " + length + "s";
    }

    public void ToggleRecording()
    {
        audioController.ToggleRecording();

        ToggleRecordingButton(audioController.GetIsRecording());
    }

    public void ToggleRecordingButton(bool isRecording)
    {
        ColorBlock newColor = ColorBlock.defaultColorBlock;

        if (isRecording)
        {
            recordButtonText.text = "Recording...";

            newColor.highlightedColor = Color.red;
            recordButton.colors = newColor;
        }
        else
        {
            recordButtonText.text = "Start Recording";
            recordButton.colors = newColor;
        }
    }

    public void CalibrateHmd()
    {
        hmdRecorder.CalibrateHmdDirection();
    }

    public void SetAudiosource()
    {
        audioController.SetAudiosourceFilePath(EditorUtility.OpenFilePanel("Select a wav file to open", "", "wav"));
    }

    public void SetLooping(bool shouldLoop)
    {
        audioController.SetLooping(shouldLoop);
    }
}
