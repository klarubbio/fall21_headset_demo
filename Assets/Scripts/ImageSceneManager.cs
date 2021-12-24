using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImageSceneManager : MonoBehaviour
{

    private List<string> scenes;
    private int scenesViewed;
    // Start is called before the first frame update
    void Start()
    {
        scenes.Add("ImageViewing1");
        scenes.Add("ImageViewing2");
        scenes.Add("ImageViewing3");
        scenes.Add("ImageViewing4");
        scenes.Add("ImageViewing5");
        scenes.Add("ImageViewing6");
        scenes.Add("ImageViewing7");
        scenes.Add("ImageViewing8");
        scenes.Add("ImageViewing9");
        scenes.Add("ImageViewing10");

        scenesViewed = 0;
    }


    public string GetNextScene()
    {
        //pick a random index from scenes to load next, remove to avoid repeats
        if (scenes.Count > 0)
        {
            scenesViewed++;
            System.Random rd = new System.Random();
            int index = rd.Next(0, scenes.Count - 1);
            string outvar = scenes[index];
            scenes.RemoveAt(index);
            return outvar;
        }
        else
            return "Menu2";
    }

    public int GetScenesViewed()
    {
        return scenesViewed;
    }


    //do not destroy on load
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
