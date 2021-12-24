using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogData : MonoBehaviour
{
    private string path;
    private string ID;
    private Ray controllerRay;
    
    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    Pvr_UnitySDKAPI.EyeTrackingData eyePoseData;
    public GameObject head;
    public Camera cam;
    public GameObject controllerLeft;
    public GameObject controllerRight;
    StreamWriter sw;
    file_naming name;
    ImageView viewer;
    RayCast_Demo target;

    // Start is called before the first frame update
    void Start()
    {
        name = GameObject.Find("FileName").GetComponent<file_naming>();
        

        if (name != null)
            path = name.GetFileName();
        else
            path = Application.persistentDataPath + "/Serror";

        ID = name.GetFileName();
        ID = ID.Substring(ID.Length - 8, 4);

        //Debug.Log(path);
        //path = Application.persistentDataPath + "/Serror.csv";

        if (!File.Exists(path))
        {
            using (sw = File.CreateText(path))
            {
#if UNITY_EDITOR
				sw.WriteLine("Editor");
#else
                sw.WriteLine("SUB_ID,SCENE,IMG_INDEX,IS_GRAY,GRAY_IDX,UNITY_TIMESTAMP,GIW_X,GIW_Y,GIW_Z,GIW_TEXTURE_X,GIW_TEXTURE_Y,HIW_X,HIW_Y,HIW_Z,HIW_TEXTURE_X,HIW_TEXTURE_Y,GAZE_DIR_X,GAZE_DIR_Y,GAZE_DIR_Z,HEAD_POS_X,HEAD_POS_Y,HEAD_POS_Z,HEAD_EUL_X,HEAD_EUL_Y,HEAD_EUL_Z,CIW0_X,CIW0_Y,CIW0_Z,CIW0_TEXTURE_X,CIW0_TEXTURE_Y,CONT0_POS_X,CONT0_POS_Y,CONT0_POS_Z,CIW1_X,CIW1_Y,CIW1_Z,CIW1_TEXTURE_X,CIW1_TEXTURE_Y,CONT1_POS_X,CONT1_POS_Y,CONT1_POS_Z");
#endif
            }

        }

        
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * SUB_ID - subject ID: file name path from dnd object
         * SCENE - current scene: name of current scene (Menu1->DotsVideo->ImageViewing->Menu2->TextRectDemo)
         * IMG_INDEX - image index: if image viewing, include index
         * IS_GRAY - is gray: if image viewing, include gray
         * GRAY_IDX - 
         * UNITY_TIMESTAMP
         * GIW_X - gaze ray position
         * GIW_Y - ""
         * GIW_Z - ""
         * GIW_TEXTURE_X - gaze ray texture coord intersection
         * GIW_TEXTURE_Y - ""
         * HIW_X - head ray position
         * HIW_Y - ""
         * HIW_Z - ""
         * HIW_TEXTURE_X - head ray texture coord intersection
         * HIW_TEXTURE_Y - ""
         * GAZE_DIR_X - gaze ray rotation
         * GAZE_DIR_Y - ""
         * GAZE_DIR_Z - ""
         * HEAD_POS_X - head position
         * HEAD_POS_Y - ""
         * HEAD_POS_Z - ""
         * HEAD_EUL_X - head rotation
         * HEAD_EUL_Y - ""
         * HEAD_EUL_Z - ""
         * all below available for left (0) or right (1)
         * CIW_X - controller ray position
         * CIW_Y - ""
         * CIW_Z - ""
         * CIW_TEXTURE_X - controller ray texture coord intersection
         * CIW_TEXTURE_Y - ""
         * CONT_POS_X - controller position (UPvr_GetControllerPOS)
         * CONT_POS_Y - ""
         * CONT_POS_Z - ""
         * only available in quaternion - should we use? 
         * CONT_DIR_X - controller rotation
         * CONT_DIR_Y - ""
         * CONT_DIR_Z - ""
         */
        using (StreamWriter sw = File.AppendText(path))
        {
            string output = ID + ",";
            Scene scene = SceneManager.GetActiveScene();
            output = output + scene.name + ",";

            //TEST
            //sw.WriteLine(output);

            if(scene.name.Contains("ImageViewing"))
            {
               // sw.WriteLine("Entered img");
                viewer = GameObject.Find("360 Sphere").GetComponent<ImageView>();
                //img index
                if (viewer != null)
                {
                    output = output + (viewer.GetIndex()) + ",";
                    //is gray
                    if (viewer.GetIsGray())
                        output += "1,";
                    else
                        output += "0,";
                    //gray idx TODO: UPDATE THIS
                    output += "3,";
                }
                else
                    output += "-1,-1,-1,";
                
            }
            else if(scene.name == "TextRectDemo")
            {
                //sw.WriteLine("Entered textrect");
                target = GameObject.Find("Head").GetComponent<RayCast_Demo>();

                if(target != null)
                {
                    output = output + target.GetTargetStep() + ",-1,-1,";
                }
                else
                    output += "-1,-1,-1,";
            }
            else
                output += "-1,-1,-1,";

            //sw.WriteLine(output);
            output = output + (Time.time).ToString() + ",";

            

            Vector3 eyetrack = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingPos();
            output = output + eyetrack.x.ToString() + "," + eyetrack.y.ToString() + "," + eyetrack.z.ToString() + ",";

            bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
            if (result)
            {
                Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
                RaycastHit hit2;
                if (scene.name.Contains("ImageViewing") || scene.name == "TextRectDemo")
                {
                    ray.origin = ray.GetPoint(1.5f + .1f);
                    ray.direction = -ray.direction;
                }

                if (Physics.Raycast(ray.origin, ray.direction, out hit2))
                {
                    Vector2 pixelUV = hit2.textureCoord;
                    output += pixelUV.x.ToString() + "," + pixelUV.y.ToString() + ",";
                }
                else
                    output += "-1,-1,";
            }
            else
                output += "-1,-1,";

            //TEST
            //sw.WriteLine(output);

            //head ray data
            RaycastHit hit;
            Ray ret = new Ray(head.transform.position, head.transform.TransformDirection(Vector3.forward));
            if (scene.name.Contains("ImageViewing") || scene.name == "TextRectDemo")
            {
                ret.origin = ret.GetPoint(1.5f + .1f);
                ret.direction = -ret.direction;
            }

            if (Physics.Raycast(ret.origin, ret.direction, out hit))
            {
                output += hit.point.x.ToString() + "," + hit.point.y.ToString() + "," + hit.point.z.ToString() + ",";
                Vector2 pixelUV = hit.textureCoord;
                output += pixelUV.x.ToString() + "," + pixelUV.y.ToString() + ",";
            }
            else
                output += "-1,-1,-1,-1,-1,";

            //eye gaze rotation data
            
            bool data = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingData(ref eyePoseData);
            if (data)
            {
                output += eyePoseData.combinedEyeGazeVector.x.ToString() + "," + eyePoseData.combinedEyeGazeVector.y.ToString() + "," + eyePoseData.combinedEyeGazeVector.z.ToString() + ",";
            }
            else
                output += "-1,-1,-1,";
            
            //head pos and rotation

            output += head.transform.position.x.ToString() + "," + head.transform.position.y.ToString() + "," + head.transform.position.z.ToString() + ",";

            Vector3 rot = cam.transform.eulerAngles;
            output += rot.x.ToString() + "," + rot.y.ToString() + "," + rot.z.ToString() + ",";

            //TEST
            //sw.WriteLine(output);

            //controller info
            if (controllerLeft.activeSelf)
            {
                //left controller
                controllerRay.direction = controllerLeft.transform.Find("dot").position - controllerLeft.transform.Find("start").position;
                controllerRay.origin = controllerLeft.transform.Find("start").position;

                if (scene.name.Contains("ImageViewing") || scene.name == "TextRectDemo")
                {
                    controllerRay.origin = controllerRay.GetPoint(1.5f + .1f);
                    controllerRay.direction = -controllerRay.direction;
                }

                RaycastHit hit3;

                if (Physics.Raycast(controllerRay, out hit3))
                {
                    output += hit3.point.x.ToString() + "," + hit3.point.y.ToString() + "," + hit3.point.z.ToString() + ",";
                    Vector2 pixelUV = hit3.textureCoord;
                    output += pixelUV.x + "," + pixelUV.y + ",";
                }
                else
                    output += "-1,-1,-1,-1,-1,";

                Vector3 leftPos = Pvr_UnitySDKAPI.Controller.UPvr_GetControllerPOS(0);
                output += leftPos.x.ToString() + "," + leftPos.y.ToString() + "," + leftPos.z.ToString() + ",";
            }

            if (controllerRight.activeSelf)
            {
                //right controller
                controllerRay.direction = controllerRight.transform.Find("dot").position - controllerRight.transform.Find("start").position;
                controllerRay.origin = controllerRight.transform.Find("start").position;

                if (scene.name.Contains("ImageViewing") || scene.name == "TextRectDemo")
                {
                    controllerRay.origin = controllerRay.GetPoint(1.5f + .1f);
                    controllerRay.direction = -controllerRay.direction;
                }

                RaycastHit hit4;

                if (Physics.Raycast(controllerRay, out hit4))
                {
                    output += hit4.point.x.ToString() + "," + hit4.point.y.ToString() + "," + hit4.point.z.ToString() + ",";
                    Vector2 pixelUV = hit4.textureCoord;
                    output += pixelUV.x + "," + pixelUV.y + ",";
                }
                else
                    output += "-1,-1,-1,-1,-1,";

                Vector3 rightPos = Pvr_UnitySDKAPI.Controller.UPvr_GetControllerPOS(1);
                output += rightPos.x.ToString() + "," + rightPos.y.ToString() + "," + rightPos.z.ToString() + ",";
            }
            sw.WriteLine(output);
        }


    }
}
