using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class VideoProgress : MonoBehaviour
{

    public GameObject videoPlane;
    public GameObject controllerPos;
    public GameObject continueButton;
    public GameObject eyeSphere;
    public GameObject controllerPos0;
    public GameObject controllerPos1;
    public GameObject imageSetup;
    public Canvas canvas;
    private bool start;
    private UnityEngine.Video.VideoPlayer videoPlayer;
    private Ray ray;
    //ImageSceneManager manageScene;

    // Start is called before the first frame update
    void Start()
    {
       // manageScene = GameObject.Find("SceneManagerObj").GetComponent<ImageSceneManager>();

        videoPlayer = videoPlane.GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        start = false;
        continueButton.SetActive(true);
        canvas.gameObject.SetActive(true);
        controllerPos0.SetActive(true);
        controllerPos1.SetActive(true);
        eyeSphere.SetActive(false);
        //eyeSphere.SetActive(false);
        //eyeGazeSphere.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (!videoPlayer.isPlaying)
        {
            if (!start)
            {
                if(Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                {
                    start = true;
                    continueButton.SetActive(false);
                    canvas.gameObject.SetActive(false);
                    controllerPos0.SetActive(false);
                    controllerPos1.SetActive(false);
                    eyeSphere.SetActive(false);
                    //eyeGazeSphere.SetActive(true);
                    videoPlayer.Play();
                }

                /*
                ray.direction = controllerPos.transform.Find("dot").position - controllerPos.transform.Find("start").position;
                ray.origin = controllerPos.transform.Find("start").position;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Button" && Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                    {
                        start = true;
                        continueButton.SetActive(false);
                        canvas.gameObject.SetActive(false);
                        controllerPos0.SetActive(false);
                        controllerPos1.SetActive(false);
                        //eyeGazeSphere.SetActive(true);
                        videoPlayer.Play();
                    }
                }*/
            }
            else
            {
                continueButton.SetActive(true);
                canvas.gameObject.SetActive(true);
                controllerPos0.SetActive(true);
                controllerPos1.SetActive(true);

                //start scene management for image viewing
                if(Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                {
                    //string nextScene = manageScene.GetNextScene();
                    //SceneManager.LoadScene("ImageViewing1", LoadSceneMode.Single);
                    SceneManager.LoadScene("ImageViewing1", LoadSceneMode.Single);
                }
                    
                /*
                ray.direction = controllerPos.transform.Find("dot").position - controllerPos.transform.Find("start").position;
                ray.origin = controllerPos.transform.Find("start").position;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Button" && Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
                    {
                        SceneManager.LoadScene("ImageViewing", LoadSceneMode.Single);
                    }
                }*/

            }
        }
        else
        {
            if (continueButton.activeSelf)
            {
                continueButton.SetActive(false);
                canvas.gameObject.SetActive(false);
                controllerPos0.SetActive(false);
                controllerPos1.SetActive(false);
                //eyeGazeSphere.SetActive(true);
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

        /*skip functionality
         * 
         * if(Pvr_UnitySDKAPI.Controller.UPvr_GetKeyLongPressed(0, Pvr_UnitySDKAPI.Pvr_KeyCode.X))
        {
            videoPlayer.Pause();
        }*/

    }
}



