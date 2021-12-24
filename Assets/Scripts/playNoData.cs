//using SMI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Video;

//List shuffle code: https://stackoverflow.com/questions/273313/randomize-a-listt
/*public static class listShuffle
{
    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    public static void Shuffle<T>(this IList<T> list, System.Random rnd)
    {
        for (var i = 0; i < list.Count; i++)
            list.Swap(i, rnd.Next(i, list.Count));
    }

}*/

public class playNoData : MonoBehaviour {

    private static System.Random rng = new System.Random();
    //private SMICalibrationVisualizer calibVis;
    private string filePath;
    private bool playing;
    private VideoPlayer vp;
    private StreamReader csvReader;
    private string lastRow;
    private GameObject lastSphere;
    private Camera cam;
    private Material mat;
    //private Vector3 lastIntersectionPosition;
    public Texture2D[] photos;
    public int curr_picture;
    public List<int> photo_indicies;

    void Start()
    {
        //filePath = "results.csv";
        //csvReader = new StreamReader(File.OpenRead(filePath));

        //read the header
        //string header = csvReader.ReadLine();
        //lastRow = header;

        //get video player
        vp = GetComponent<VideoPlayer>();

        mat = GetComponent<MeshRenderer>().material;

        //filename to read in
        //TODO: better filenames
        //filePath = "results.csv";

        playing = false;
        curr_picture = 0;

        //keep list of photo indicies, randomize for experiments
        photo_indicies = new List<int>();

        for (int i = 0; i < photos.Length; i++)
        {
            photo_indicies.Add(i);
        }

        //randomize the list
        //listShuffle.Shuffle(photo_indicies, rng);

        
        cam = Camera.main;
        //cam = GameObject.Find("Camera").GetComponent<Camera>();
        PlayVideo();
    }

    public void PlayVideo()
    {
        playing = true;
        //vp.Play();
        GetComponent<MeshRenderer>().enabled = true;

        //get current texture, load it 
        mat = GetComponent<MeshRenderer>().material;
        mat.SetTexture("_MainTex", photos[photo_indicies[curr_picture]]);

    }

    public void StopVideo()
    {
        playing = false;
        //vp.Play();
        GetComponent<MeshRenderer>().enabled = false;

    }

    public bool isPlaying()
    {
        return playing;
    }

    void Update()
    {
        
        /*
        //Check for key press for interaction. This gameobject is activated when spacebar is pressed so maybe use different input keys
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!playing)
            {
                PlayVideo();

            }
            else
            {
                StopVideo();
            }
        }

        //TODO: add left and right to switch image, move curr_picture based on length of photo_indicies
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (playing)
            {
                curr_picture -= 1;
                if (curr_picture < 0)
                {
                    curr_picture = photos.Length - 1;
                }

                mat.SetTexture("_MainTex", photos[photo_indicies[curr_picture]]);

            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (playing)
            {
                curr_picture += 1;
                if (curr_picture == photos.Length)
                {
                    curr_picture = 0;
                }

                mat.SetTexture("_MainTex", photos[photo_indicies[curr_picture]]);

            }
        }
        */

        //if (playing)
        //    drawGazePoint();
    }

    /**
     * This may be naive but going to keep reading lines until reach the frame that video is on,
     * could 
     */
    void drawGazePoint()
    {
        long frame = vp.frame;

        bool foundFrame = false;

        string line;
        string currFrame;
        //read line until we find the frame number
        while (!foundFrame && !csvReader.EndOfStream)
        {
            line = csvReader.ReadLine();
            lastRow = line;

            string[] line_fields = line.Split(',');

            currFrame = line_fields[0];
            if (System.Convert.ToInt64(currFrame) >= frame)
            {
                foundFrame = true;
                //delete old point then draw point, TODO: make a prefab, for now just draw a primitive

                if (lastSphere != null)
                {
                    Destroy(lastSphere);
                }

                float gazePosX = float.Parse(line_fields[1]);
                float gazePosY = float.Parse(line_fields[2]);
                float gazePosZ = float.Parse(line_fields[3]);
                float depthScale = float.Parse(line_fields[4]);
                float camPosX = float.Parse(line_fields[5]);
                float camPosY = float.Parse(line_fields[6]);
                float camPosZ = float.Parse(line_fields[7]);
                float camEulX = float.Parse(line_fields[8]);
                float camEulY = float.Parse(line_fields[9]);
                float camEulZ = float.Parse(line_fields[10]);

                Vector3 gazePos = new Vector3(gazePosX, gazePosY, gazePosZ); 

                //TODO: instantiate is bad, use a prefab in future
                lastSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                lastSphere.transform.position = gazePos;
                lastSphere.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
                lastSphere.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 1.0f, 0.0f);

                cam.transform.position = new Vector3(camPosX, camPosY, camPosZ);
                //cam.transform.eulerAngles = new Vector3(camEulX, camEulZ, camEulY);

                Debug.Log(camEulZ);
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(lastSphere.transform.position, 0.15f);
    }

    private void OnApplicationQuit()
    {
        //csvReader.Close();
    }
}
