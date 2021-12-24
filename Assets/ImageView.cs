using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class listShuffle
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

}


public class ImageView : MonoBehaviour
{
    private static System.Random rng = new System.Random();
    public Texture2D[] photos;
    public Texture2D gray;
    public int curr_picture;
    public string next;
    private List<int> photo_indicies;
    public int[] standard_indices;
    public GameObject button;
    public Canvas canvas;
    public GameObject controllerPos0;
    public GameObject controllerPos1;
    public GameObject eyeSphere;
    //public GameObject crosshair;
    public GameObject head;
    private string path;
    private Material mat;
    private bool isGray;
    private Ray ray;
    private float timer;
    StreamWriter sw;
    private bool skipped;
   // ImageSceneManager manageScene;

    // Start is called before the first frame update
    void Start()
    {
       // manageScene = GameObject.Find("SceneManagerObj").GetComponent<ImageSceneManager>();

        curr_picture = -1;
        isGray = true;
        skipped = false;

        photo_indicies = new List<int>();

        for (int i = 0; i < photos.Length; i++)
        {
            photo_indicies.Add(i);
        }

        listShuffle.Shuffle(photo_indicies, rng);
        //timer = Time.time + 5f;
        timer = Time.time;

        button.SetActive(false);
        canvas.gameObject.SetActive(false);

        controllerPos0.SetActive(true);
        controllerPos1.SetActive(true);

        eyeSphere.SetActive(false);
        //crosshair.SetActive(true);

        //keep list of photo indicies, randomize for experiments
        

        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timer || skipped) //switch images
        {
            if (((photo_indicies.Count == curr_picture + 1) && isGray) || skipped)
            {
                if (next == "Menu2" || next == "ImageViewing6")
                {
                    if (!controllerPos0.activeSelf)
                    {
                        controllerPos0.SetActive(true);
                        controllerPos1.SetActive(true);
                    }
                    button.SetActive(true);
                    canvas.gameObject.SetActive(true);

                    if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                        SceneManager.LoadScene(next, LoadSceneMode.Single);
                }
                else
                    SceneManager.LoadScene(next, LoadSceneMode.Single);
                /*
                //all images viewed, give button to proceed
                if (manageScene.GetScenesViewed() == 10 || skipped)
                {
                    if (!controllerPos0.activeSelf)
                    {
                        controllerPos0.SetActive(true);
                        controllerPos1.SetActive(true);
                    }
                    button.SetActive(true);
                    canvas.gameObject.SetActive(true);

                    if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                        SceneManager.LoadScene("Menu2", LoadSceneMode.Single);

                }
                //still have scenes to view, but have viewed half (give break)
                else if(manageScene.GetScenesViewed() == 5)
                {
                    if (!button.activeSelf)
                    {
                        //activate button
                        button.SetActive(true);
                        canvas.gameObject.SetActive(true);
                        controllerPos0.SetActive(true);
                        controllerPos1.SetActive(true);
                    }

                    if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                    {
                        //load next random scene
                        string nextScene = manageScene.GetNextScene();
                        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
                    }
                }
                //still have scenes to view, continue
                else
                {
                    string nextScene = manageScene.GetNextScene();
                    SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
                }*/



                /*
                ray.direction = controllerPos1.transform.Find("dot").position - controllerPos1.transform.Find("start").position;
                ray.origin = controllerPos1.transform.Find("start").position;
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                {
                    if(hit.transform.tag == "Button")
                        SceneManager.LoadScene("Menu2", LoadSceneMode.Single);
                }*/
            }
            else if (isGray)
            {
                //break over, set next picture
                //crosshair.SetActive(false);
                //if curr_picture is the halfway point, enable the button and wait for user input to proceed
                /*if (curr_picture == ((photos.Length-1) / 2))
                {
                    if (!button.activeSelf)
                    {
                        //activate button
                        button.SetActive(true);
                        canvas.gameObject.SetActive(true);
                        controllerPos0.SetActive(true);
                        controllerPos1.SetActive(true);
                    }

                    if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                    {
                        ++curr_picture;
                        button.SetActive(false);
                        canvas.gameObject.SetActive(false);
                        timer += 25f;
                        GetComponent<MeshRenderer>().enabled = true;
                        mat.SetTexture("_MainTex", photos[photo_indicies[curr_picture]]);
                        isGray = false;
                    }

                    
                    ray.direction = controllerPos1.transform.Find("dot").position - controllerPos1.transform.Find("start").position;
                    ray.origin = controllerPos1.transform.Find("start").position;
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                    {
                        if (hit.transform.tag == "Button")
                        {
                            ++curr_picture;
                            button.SetActive(false);
                            canvas.gameObject.SetActive(false);
                            timer += 25f;
                            GetComponent<MeshRenderer>().enabled = true;
                            mat.SetTexture("_MainTex", photos[photo_indicies[curr_picture]]);
                            isGray = false;
                        }
                    }
                }*/
                isGray = false;
                timer += 25f;
                GetComponent<MeshRenderer>().enabled = true;
                ++curr_picture;
                mat.SetTexture("_MainTex", photos[photo_indicies[curr_picture]]);


                if (controllerPos0.activeSelf)
                {
                    controllerPos0.SetActive(false);
                    controllerPos1.SetActive(false);
                }
            }
            else
            {
                //break starts
                //crosshair.SetActive(true);
                isGray = true;
                mat.SetTexture("_MainTex", gray);
                timer += 5f;
            }
        }


        /* eye gaze validation
        if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.B))
        {
            if (eyeSphere.activeSelf)
                eyeSphere.SetActive(false);
            else
                eyeSphere.SetActive(true);
        }
        */

        /*
         * skip functionality
         * 
        if(Pvr_UnitySDKAPI.Controller.UPvr_GetKeyLongPressed(0, Pvr_UnitySDKAPI.Pvr_KeyCode.X)){
            isGray = true;
            mat.SetTexture("_MainTex", gray);
            skipped = true;
            /*isGray = true;
            mat.SetTexture("_MainTex", gray);
            if (!controllerPos0.activeSelf)
            {
                controllerPos0.SetActive(true);
                controllerPos1.SetActive(true);
            }
            button.SetActive(true);
            canvas.gameObject.SetActive(true);
            
            bool buttonHit = false;
            while (!buttonHit)
            {
                ray.direction = controllerPos1.transform.Find("dot").position - controllerPos1.transform.Find("start").position;
                ray.origin = controllerPos1.transform.Find("start").position;
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                {
                    if (hit.transform.tag == "Button")
                    {
                        buttonHit = true;
                        SceneManager.LoadScene("Menu2", LoadSceneMode.Single);
                    }
                }
            }
        }*/
    
    }

    public int GetIndex()
    {
        return standard_indices[photo_indicies[curr_picture]];
    }

    public bool GetIsGray()
    {
        return isGray;
    }

    public float GetGrayIDX()
    {
        return head.transform.rotation.x;
    }
}
