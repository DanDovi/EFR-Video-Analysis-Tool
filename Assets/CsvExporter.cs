using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class CsvExporter : MonoBehaviour {

    [DllImport("user32.dll")]
    private static extern void SaveFileDialog(); //in your case : OpenFileDialog

    public ExperimentTracker thisExperimentTracker;
    
    public void Export_CSV()
    {
        string pelletString = "";
        string chowString = "";
        string mouseEventString = "";
        
        foreach (PelletOccurence po in thisExperimentTracker.pelletOccurrences)
        {
            pelletString += po.time + "\n";
        }
        foreach (ChowOccurence co in thisExperimentTracker.chowOccurrences)
        {
            chowString += co.timeStart + ", " + co.duration + "\n";
        }
        
        List<MouseEvent> mouseEvents = new List<MouseEvent>(thisExperimentTracker.mouseEvents);
        mouseEvents.Sort();
        foreach (MouseEvent mo in thisExperimentTracker.mouseEvents)
        {
            mouseEventString += mo.time + ", " + mo.type + "\n";
        }

        string wholeString = "Pellet Time \n" + pelletString + "\n \nChow Time, Duration\n" + chowString + "\n \nEvent time, type\n" + mouseEventString;
        
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
        sfd.FilterIndex = 1;
        sfd.RestoreDirectory = true;
        sfd.FileName = "MouseData_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"); 

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            using (StreamWriter sw = File.CreateText(sfd.FileName))
            {
                sw.Write(wholeString);
            }
        }
    }
}
