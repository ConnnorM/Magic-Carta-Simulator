using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

//Code For the CSVReader setup from:
// https://www.youtube.com/watch?v=tI9NEm02EuE
public class CSVreader : MonoBehaviour
{
    public TextAsset[] textAssetData;

    public Deck currDeck = new Deck();

    public int csvListIndex = 0;

    public void SetCsvToRead(int chosenSongIndex)
    {
        csvListIndex = chosenSongIndex;
    }
    
    public void ReadCSV()
    {
        //split the csv on commas and new lines: each string in the array is now each value from the csv
        string[] data = textAssetData[csvListIndex].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        
        //divide by number of columns and take one off for indexing to get the number of entries in the table
        int tableSize = data.Length / 2 - 1;
        
        //currNoteList's array of notes is assigned a new note array that's as big as the number
        //of entries table
        // currNoteList.note = new Note[tableSize];

        //based on the number of entries in the table, allocate that many indexes in the array of
        //notes so that each can be stored with its data
        // for (int i = 0; i < tableSize; i++)
        // {
        //     //see note above loop
        //     currNoteList.note[i] = new Note();
        //     
        //     //now populate that note with the info it needs:
        //     //from data, which is just an array of strings storing each entry
        //     //on first time through: grab the 1 * 0 + 1 = 2nd thing/#1 index which ignores the first row
        //     //and grabs the first thing for that note: arrowDir
        //     //on second time through, grab the 1 * 1 + 1 = 3rd thing which is note2's arrowDir
        //     currNoteList.note[i].arrowDir = data[2 * (i + 1) + 0];
        //     //Repeat for the other attributes
        //     currNoteList.note[i].targetTime = int.Parse(data[2 * (i + 1) + 1]);
        // }
    }
}