using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Pvr_UnitySDKAPI;

public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    public string nextScene;
    public GameObject button;
    public GameObject controllerPos;
    public GameObject eyeSphere;
    private Ray ray;

    void Update()
    {
        if(Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);

        /*ray.direction = controllerPos.transform.Find("dot").position - controllerPos.transform.Find("start").position;
        ray.origin = controllerPos.transform.Find("start").position;
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit) && Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A) && hit.transform.tag == "Button")
        {

            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);

        }*/


        if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.B))
        {
            if (eyeSphere.activeSelf)
                eyeSphere.SetActive(false);
            else
                eyeSphere.SetActive(true);
        }
    }
}
