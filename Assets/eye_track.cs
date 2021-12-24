using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class eye_track : MonoBehaviour
{
    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    // Update is called once per frame


    void Update()
    {

        //eye gaze ray raycast
        bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        if (result)
        {
            Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
            //Gizmos.DrawRay(gazeRay.Origin, gazeRay.Direction);
            RaycastHit hit2;
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "ImageViewing")
            {
                ray.origin = ray.GetPoint(1.5f + .1f);
                ray.direction = -ray.direction;
            }
            if (Physics.Raycast(ray, out hit2))
            {
                
                this.transform.position = hit2.point;
                //comment out
                //sw.WriteLine("Collider," + hit2.collider.gameObject.name);
            }

        }

    }

}
