using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR.InteractionSystem;

public class HmdRecorder : MonoBehaviour
{
    public GameObject hmd;
    public GameObject playSpace;

    private List<HmdFrame> hmdFrames = new List<HmdFrame>();
    private bool isRecording = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (isRecording)
	    {
	        HmdFrame currentHmdFrame = new HmdFrame(GetCurrentRotation(), AudioSettings.dspTime);
	        hmdFrames.Add(currentHmdFrame);
	    }
	}

    public void ToggleRecording()
    {
        if (!isRecording)
        {
            hmdFrames.Clear();
            isRecording = true;
        }
        else
        {
            isRecording = false;
        }
    }

    public Vector3 GetCurrentRotation()
    {
        return hmd.transform.eulerAngles;
    }

    public void CalibrateHmdDirection()
    {
        playSpace.transform.eulerAngles = new Vector3(0f, (GetCurrentRotation().y * -1f) + playSpace.transform.eulerAngles.y, 0f);
    }

    public List<HmdFrame> GetHmdFrames()
    {
        return hmdFrames;
    }

    public Vector3[] GetArrayOfSize(int size, int samplerate)
    {
        Assert.AreNotEqual(0, hmdFrames.Count);

        // Interpolation might be dangerous due to angle wrapping

        Vector3[] vectorsToReturn = new Vector3[size];
        float scalingFactor = (float) hmdFrames.Count / size;

        for (int i = 0; i < vectorsToReturn.Length; i++)
        {
            vectorsToReturn[i] = hmdFrames[(int)(i * scalingFactor)].eulerRotation;
        }

        return vectorsToReturn;
    }

    public void SaveArrayAsCsv(Vector3[] arrayToSave, string savePath)
    {
        List<string[]> rowData = new List<string[]>(arrayToSave.Length + 1);

        // Add headers
        string[] tempRowData = new string[3];
        tempRowData[0] = "X";
        tempRowData[1] = "Y";
        tempRowData[2] = "Z";
        rowData.Add(tempRowData);

        // Add data
        for (int i = 0; i < arrayToSave.Length; i++)
        {
            tempRowData = new string[3];
            tempRowData[0] = arrayToSave[i].x.ToString("F1");
            tempRowData[1] = arrayToSave[i].y.ToString("F1");
            tempRowData[2] = arrayToSave[i].z.ToString("F1");
            rowData.Add(tempRowData);
        }

        // Construct an output string
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        // Get length of outer array and define delimiter (for csv formatting)
        int length = output.GetLength(0);
        string delimiter = ",";

        // Build the full string as comma seperated values
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }

        savePath = Path.Combine(savePath, "hmddata.csv");

        // Open a filestream and write the file
        StreamWriter outStream = File.CreateText(savePath);
        outStream.WriteLine(sb);
        outStream.Close();

        Debug.Log("Saved csv file: " + savePath);
    }
}
