using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRects : MonoBehaviour
{
    public TextAsset file;

    void printLine(string targe, string labe, float pox, float poy, float poz, float rox, float roy, float roz, float scalex, float scaley, float scalez)
    {
        Debug.Log(targe + ": " + labe + "- Position: " + pox + " " + poy + " " + poz + " -- Rotation: " + rox + " " + roy + " " + roz + " -- Scale: " + scalex + " " + scaley + " " + scalez);
    }
    
    
    
    void LoadInventory()
    {
        //These are the variables I set
        string target = "";
        string label = "";
        float pos_x = 0;
        float pos_y = 0;
        float pos_z = 0;
        float rot_x = 0;
        float rot_y = 0;
        float rot_z = 0;
        float scale_x = 0;
        float scale_y = 0;
        float scale_z = 0;

        //This is to get all the lines using Method 1
        string[] lines = file.text.Split("\n"[0]);

        for (var i = 1; i < lines.Length; i++)
        {
            //This is to get every thing that is comma separated
            string[] parts = lines[i].Split(","[0]);

            target = parts[0];
            
            label = parts[1];
            float.TryParse(parts[2], out pos_x);
            float.TryParse(parts[3], out pos_y);
            float.TryParse(parts[4], out pos_z);
            float.TryParse(parts[5], out rot_x);
            float.TryParse(parts[2], out rot_y);
            float.TryParse(parts[3], out rot_z);
            float.TryParse(parts[4], out scale_x);
            float.TryParse(parts[5], out scale_y);
            float.TryParse(parts[5], out scale_z);

            //THis just makes a cube because why not, should be what the object is
            //GameObject gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //This changes the name of the cube to the read name
            //gameObj.name = target;
            printLine(target, label, pos_x, pos_y, pos_z, rot_x, rot_y, rot_z, scale_x, scale_y, scale_z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadInventory();
    }

    // Update is called once per frame
    /*
    void Update()
    {
        
    }
    */
}
