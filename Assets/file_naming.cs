using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class file_naming : MonoBehaviour
{
    public string fileName = "";
    public string max_id = "";

    void Start()
    {
        if (fileName == "")
        {
            string dir = "";
#if UNITY_EDITOR
		    dir = "./Data";
#else
            dir = Application.persistentDataPath;
#endif
            string[] data_files = Directory.GetFiles(dir);
            int maxID = -1;

            //360_videos repo
            string pat = "S(\\d\\d\\d\\d).csv";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);

            int max_id = -1;
            string curr;
            int curr_val;
            for (int i = 0; i < data_files.Length; i++)
            {
                Match match = r.Match(data_files[i]);
                if (match.Success)
                {
                    //check value
                    curr = match.Groups[1].Value;
                    curr_val = Int32.Parse(curr);
                    if (curr_val > max_id)
                        max_id = curr_val;
                }
            }

            max_id++;

            fileName = dir;
            fileName += "/S" + max_id.ToString("D4") + ".csv";

            Debug.Log(fileName);
        }
    }

    public string GetFileName()
    {
        return fileName;
    }

    public string GetMaxID()
    {
        return max_id;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
