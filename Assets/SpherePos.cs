using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpherePos : MonoBehaviour
{
    public GameObject head;
    public GameObject sphere;

    file_naming name;
    private string path;
    StreamWriter sw;

    void Start()
    {
        name = GameObject.Find("FileName").GetComponent<file_naming>();
        if(name != null)
        {
            path = name.GetFileName();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
       /* if (File.Exists(path))
        {
            using(StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("transform reset");
            }
        }*/
        sphere.transform.position = head.transform.position;
    }
}
