
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Pvr_UnitySDKAPI;

public class Target
{
	public bool Text;           //indicate if corresponding text should be on
	public bool Revealed;      //indicate if pixel border is on
	public string Description;
	public bool Step;	//true when the step is complete in appropriate order

	//sequential function
	public int Order;
	public string Prompt;
	public int Width;
	public int Height;

	//texture coord setup
	public List<Vector2> TargetVertices;
	public List<Vector2> NormalTargetVertices;
	public Vector2 ImagePixels;

	public Target(Vector2 minPixels, Vector2 maxPixels, string description, int order, string prompt)
    {
		Description = description;
		Order = order;
		Prompt = prompt;
		Text = false;
		Revealed = false;
		Step = false;
		ImagePixels = new Vector2(3840.0f, 2160.0f);
		TargetVertices = new List<Vector2>();
		NormalTargetVertices = new List<Vector2>();
		SetTargetVertices(minPixels, maxPixels);
		Width = (int)(maxPixels.x - minPixels.x);
		Height = (int)(maxPixels.y - minPixels.y);
    }


	public void SetTargetVertices(Vector2 minPixels, Vector2 maxPixels)
	{
		//set each vertex of target rectangle
			//max and min are inverse due to 1-norm value later
		TargetVertices.Add(new Vector2(maxPixels.x, maxPixels.y));
		TargetVertices.Add(new Vector2(minPixels.x, maxPixels.y));
		TargetVertices.Add(new Vector2(maxPixels.x, minPixels.y));
		TargetVertices.Add(new Vector2(minPixels.x, minPixels.y));

		//normalize these vertices
		for (int i = 0; i < TargetVertices.Count; i++)
		{
			NormalTargetVertices.Add(new Vector2((1-(TargetVertices[i].x / ImagePixels.x)), TargetVertices[i].y / ImagePixels.y));
			
		}
		//Debug.Log(this.Description + "y min" + this.NormalTargetVertices[3].y + "y max" + this.NormalTargetVertices[1].y);
	}

}

public class RayCast_Demo : MonoBehaviour
{
	//public Material highlightMaterial;
	public GameObject plane;
	public float TargetDistance;
	public TextMeshProUGUI textDisplayed;
	public TextMeshProUGUI textPrompt;

	public GameObject eyeSphere;

	private List<Target> targets;

	//uv coordinate prep
	private List<Vector3> LocalVertices;
	private List<Vector3> CornerVertices;

	private List<Vector2> NormalVertices;
	

	private Vector2 ImagePixels;
	

	private Color[] pix;
	private Color[] leftPix, rightPix, topPix, bottomPix;
	private int TargetStep;
	private int ResetIndex;

	//file reading
	public TextAsset file;
	string path;
	StreamWriter sw;
	file_naming name;

	//CONTROLLER INPUT
	public GameObject controllerRight;
	private Ray controllerRay;


	Color AverageColor(Color input)
    {
		float[] colorVals = { 0, 0, 0, 0 };
		colorVals[0] = ((1.2f + (0.8f * input[0])) / 2.0f);
		colorVals[1] = ((1.2f + (0.8f * input[1])) / 2.0f);
		colorVals[2] = ((1.2f + (0.8f * input[2])) / 2.0f);
		if (colorVals[0] > 1.0f)
			colorVals[0] = 1.0f;
		if (colorVals[1] > 1.0f)
			colorVals[1] = 1.0f;
		if (colorVals[2] > 1.0f)
			colorVals[2] = 1.0f;
		colorVals[3] = 1.0f;
		Color color = new Color(colorVals[0], colorVals[1], colorVals[2], colorVals[3]);
		return color;
	}

	void Start()
    {
		
		//SetText();

		NormalVertices = new List<Vector2>();
		
		CornerVertices = new List<Vector3>();
		targets = new List<Target>();
		//LoadInventory();

		//currently hardcoded
		ImagePixels = new Vector2(3840.0f, 2160.0f);

		//data to be imported from a CSV file 

		//targets.Add(new Target(new Vector2(1527.0f, 879.0f), new Vector2(1659.0f, 1255.0f), "Control room access"));
		targets.Add(new Target(new Vector2(2181.0f, 879.0f), new Vector2(2313.0f, 1255.0f), "Control room access", 1, "Verify secured control room"));
		targets.Add(new Target(new Vector2(1570.0f, 867.0f), new Vector2(1700.0f, 1100.0f), "Radiation monitor units", 0, "na"));
		targets.Add(new Target(new Vector2(560.0f, 1060.0f), new Vector2(620.0f, 1100.0f), "Power level indicator", 0, "na"));
		targets.Add(new Target(new Vector2(660.0f, 1060.0f), new Vector2(710.0f, 1100.0f), "Power level indicator", 0, "na"));
		targets.Add(new Target(new Vector2(1285.0f, 910.0f), new Vector2(1435.0f, 1060.0f), "Digital temperature recorder and indicator", 0, "na"));
		targets.Add(new Target(new Vector2(65.0f, 980.0f), new Vector2(420.0f, 1135.0f), "Power level recorder", 5, "Start power recorder and verify functionality"));
		targets.Add(new Target(new Vector2(3515.0f, 1060.0f), new Vector2(3570.0f, 1105.0f), "Reactor period (kinetic behavior)", 8, "Maintain suitable reactor period"));
		targets.Add(new Target(new Vector2(3625.0f, 1060.0f), new Vector2(3700.0f, 1105.0f), "Power level (log-scale) recorder", 9, "Approach desired power level"));
		targets.Add(new Target(new Vector2(15.0f, 685.0f), new Vector2(680.0f, 880.0f), "Operator log book", 2, "Initiate log book notes"));
		targets.Add(new Target(new Vector2(455.0f, 1040.0f), new Vector2(487.0f, 1123.0f), "Status and alarm indicator", 3, "Verify all status and alarm conditions")); //another one is required, implement later
		targets.Add(new Target(new Vector2(20.0f, 900.0f), new Vector2(55.0f, 915.0f), "Key slot", 4, "Access console controls with key"));
		targets.Add(new Target(new Vector2(140.0f, 910.0f), new Vector2(355.0f, 960.0f), "Safety rod indicators", 6, "Start moving out safety rods")); //could be all separate AOIs
		targets.Add(new Target(new Vector2(415.0f, 900.0f), new Vector2(450.0f, 930.0f), "Control blade", 7, "Start moving out control blade"));
		targets.Add(new Target(new Vector2(3615.0f, 950.0f), new Vector2(3655.0f, 1000.0f), "Auto controls", 10, "Activate auto controls to maintain steady power level"));

		
		GetPlaneVertices();
		
		//copying texture
		Renderer rend = plane.GetComponent<Renderer>();
		Texture2D texture = rend.material.mainTexture as Texture2D;

		pix = texture.GetPixels(0, 0, texture.width, texture.height);

		//set all initial white borders
		
		for(int i = 0; i < targets.Count; i++)
        {
			Target AOI = targets[i];
			float[] colorVals = { 1, 1, 1, 1 };
			for (int y = (int)AOI.TargetVertices[2].y; y < ((int)AOI.TargetVertices[2].y + 15); y++)
			{
				for (int x = (int)AOI.TargetVertices[3].x; x < AOI.TargetVertices[0].x; x++)
				{
					Color original = texture.GetPixel(x, y);
					texture.SetPixel(x, y, AverageColor(original));
				}
			}
			for (int y = ((int)AOI.TargetVertices[2].y); y < ((int)AOI.TargetVertices[0].y + 15); y++)
			{
				for (int x = (int)AOI.TargetVertices[0].x; x < ((int)AOI.TargetVertices[0].x + 15); x++)
				{
					Color original = texture.GetPixel(x, y);
					texture.SetPixel(x, y, AverageColor(original));
				}

				for (int x = ((int)AOI.TargetVertices[3].x - 15); x < ((int)AOI.TargetVertices[3].x); x++)
				{
					Color original = texture.GetPixel(x, y);
					texture.SetPixel(x, y, AverageColor(original));
				}
			}
			//top border 
			for (int y = ((int)AOI.TargetVertices[0].y); y < ((int)AOI.TargetVertices[0].y + 15); y++)
			{
				for (int x = (int)AOI.TargetVertices[3].x; x < AOI.TargetVertices[0].x; x++)
				{
					/*Color original = texture.GetPixel(x, y);
					colorVals[0] = ((1.0f + original[0]) / 2.0f) % 1.0f;
					colorVals[1] = ((1.0f + original[1]) / 2.0f) % 1.0f;
					colorVals[2] = ((1.0f + original[2]) / 2.0f) % 1.0f;
					colorVals[3] = 1.0f;
					Color color = new Color(colorVals[0], colorVals[1], colorVals[2], colorVals[3]);
					texture.SetPixel(x, y, color);*/
					Color original = texture.GetPixel(x, y);
					texture.SetPixel(x, y, AverageColor(original));
				}
			}
		}

		texture.Apply();

		Debug.Log("borders drawn");

		
		
		//reset = true;

		TargetStep = 1;
		ResetIndex = -1;


		/*
		name = GameObject.Find("FileName").GetComponent<file_naming>();

		if (name != null)
			path = name.GetFileName();
		else
			path = "./error";


		if (!File.Exists(path))
		{
			using (sw = File.CreateText(path))
			{
#if UNITY_EDITOR
				sw.WriteLine("Timestamp," + "Quat_w," + "Quat_x," + "Quat_y," + "Quat_z," + "X Angle," + "Y Angle," + "Z Angle");
#else
				sw.WriteLine("Timestamp," + "Quat_w," + "Quat_x," + "Quat_y," + "Quat_z," + "X Angle," + "Y Angle," + "Z Angle," + "X EYE," + "Y EYE," + "Z EYE");
#endif
			}

		}*/
		eyeSphere.SetActive(false);
	}
	
	void WriteFile()
    {

		//documentation: https://docs.microsoft.com/en-us/dotnet/api/system.io.file.appendtext?view=net-5.0
		
		
		Quaternion temp = transform.rotation;
		Vector3 angles = transform.eulerAngles;
#if !UNITY_EDITOR
		Vector3 eyetrack = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingPos();
#endif
		using (StreamWriter sw = File.AppendText(path))
        {
#if UNITY_EDITOR
			sw.WriteLine(Time.time + "," + temp.w + "," + temp.x + "," + temp.y + "," + temp.z + "," + angles.x + "," + angles.y + "," + angles.z);
#else
			sw.WriteLine(Time.time + "," + temp.ToString() + "," + angles.x + "," + angles.y + "," + angles.z + "," + eyetrack.x + "," + eyetrack.y + "," + eyetrack.z);
#endif

		}
		
	}

	void SetAllTextFalse()
	{
		
		for (int i = 0; i < targets.Count; i++)
		{
			targets[i].Text = false;
		}

	}

	void LoadInventory()
	{
		//placeholder variables
		string label, seq_step = "";
		int seq_num = -1;
		float x_min, y_min, x_max, y_max = 0;

		//This is to get all the lines
		string[] lines = file.text.Split("\n"[0]);
		
		for (var i = 1; i < lines.Length-1; i++)
		{
			//This is to get every thing that is comma separated
			string[] parts = lines[i].Split(","[0]);

			label = parts[0];
			seq_step = parts[1];
			int.TryParse(parts[2], out seq_num);
			float.TryParse(parts[3], out x_min);
			float.TryParse(parts[4], out y_min);
			float.TryParse(parts[5], out x_max);
			float.TryParse(parts[6], out y_max);

			//Debug.Log("label: " + label + " seq_step: " + seq_step + " y_min: " + y_min + " y_max: " + y_max);
			//add as an object
			targets.Add(new Target(new Vector2(x_min, y_min), new Vector2(x_max, y_max), label, seq_num, seq_step));
		}
	}
	//if green is true, the pixels are being set green and prev pixels must be saved
	void SetBorders(bool green, Target AOI)
    {
		Renderer rend = plane.GetComponent<Renderer>();
		Texture2D test = rend.material.mainTexture as Texture2D;
		if (green)
        {
			//save pixels before setting green
			leftPix = test.GetPixels(((int)AOI.TargetVertices[3].x-15), (int)AOI.TargetVertices[3].y, 15, (AOI.Height+15));
			rightPix = test.GetPixels((int)AOI.TargetVertices[0].x, (int)AOI.TargetVertices[3].y, 15, (AOI.Height+15));
			topPix = test.GetPixels((int)AOI.TargetVertices[3].x, (int)AOI.TargetVertices[0].y, AOI.Width, 15);
			bottomPix = test.GetPixels((int)AOI.TargetVertices[3].x, (int)AOI.TargetVertices[3].y, AOI.Width, 15);

			//set all of the pixels green
			//bottom border
			for (int y = (int)AOI.TargetVertices[2].y; y < ((int)AOI.TargetVertices[2].y + 15); y++)
			{
				for (int x = (int)AOI.TargetVertices[3].x; x < AOI.TargetVertices[0].x; x++)
				{
					Color color = Color.green;
					test.SetPixel(x, y, color);
				}
			}
			for (int y = ((int)AOI.TargetVertices[2].y); y < ((int)AOI.TargetVertices[0].y + 15); y++)
			{
				for (int x = (int)AOI.TargetVertices[0].x; x < ((int)AOI.TargetVertices[0].x + 15); x++)
				{
					Color color = Color.green;
					test.SetPixel(x, y, color);
				}

				for (int x = ((int)AOI.TargetVertices[3].x - 15); x < ((int)AOI.TargetVertices[3].x); x++)
				{
					Color color = Color.green;
					test.SetPixel(x, y, color);
				}
			}
			//top border 
			for (int y = ((int)AOI.TargetVertices[0].y); y < ((int)AOI.TargetVertices[0].y + 15); y++)
			{
				for (int x = (int)AOI.TargetVertices[3].x; x < AOI.TargetVertices[0].x; x++)
				{
					Color color = Color.green;
					test.SetPixel(x, y, color);
				}
			}
			test.Apply();
		}
        else
        {
			//remove all borders, set back to prev pix
			test.SetPixels(((int)AOI.TargetVertices[3].x-15), (int)AOI.TargetVertices[3].y, 15, (AOI.Height+15), leftPix); 
			test.SetPixels((int)AOI.TargetVertices[0].x, (int)AOI.TargetVertices[3].y, 15, (AOI.Height+15), rightPix);
			test.SetPixels((int)AOI.TargetVertices[3].x, (int)AOI.TargetVertices[0].y, AOI.Width, 15, topPix);
			test.SetPixels((int)AOI.TargetVertices[3].x, (int)AOI.TargetVertices[3].y, AOI.Width, 15, bottomPix);
			test.Apply();
		}


		
        
    }

	void SetText()
    {
		//set description text
		textDisplayed.text = "Find a target!";
		int progressIndex = -1;
		for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].Text)
            {
				textDisplayed.text = targets[i].Description;
				//Debug.Log("text set for " + i + targets[i].Description);
            }
        }
		//set prompt text
		if (TargetStep == 1)
        {
			for(int j = 0; j < targets.Count; j++)
            {
				if(TargetStep == targets[j].Order)
                {
					textPrompt.text = targets[j].Prompt;
					//break here?
                }
            }
        }
		else if(TargetStep == 11)
        {
			textPrompt.text = "Reactor successfully powered up";
        }
        else
        {
			for(int k = 0; k < targets.Count; k++)
            {
                //find the index of the last completed step, for loop finds last completed
                if(targets[k].Step && (targets[k].Order > progressIndex))
                {
					progressIndex = targets[k].Order;
                }
            }
			//find the appropriate step prompt
			progressIndex++;
			for(int x = 0; x < targets.Count; x++)
            {
				if(progressIndex == targets[x].Order)
                {
					textPrompt.text = targets[x].Prompt;
                }
            }
        }
	}

	void GetPlaneVertices()
    {
		LocalVertices = new List<Vector3>(plane.GetComponent<MeshFilter>().mesh.vertices);

		CornerVertices.Clear();

		CornerVertices.Add(LocalVertices[0]);
		CornerVertices.Add(LocalVertices[10]);
		CornerVertices.Add(LocalVertices[110]);
		CornerVertices.Add(LocalVertices[120]);

		for(int i = 0; i < CornerVertices.Count; i++)
        {
			NormalVertices.Add(new Vector2(((CornerVertices[i].x * -1)+5)/10, ((CornerVertices[i].z + 5)/10)));
			//Debug.Log("Point " + i + " x val = " + NormalVertices[i].x + "y val = " + NormalVertices[i].y);
		}
		
	}
	void Reset(Color[] pixels)
    {
		//reset the image once user looks away from target/at the end of the program
		Renderer rend = plane.GetComponent<Renderer>();
		Texture2D texture = rend.material.mainTexture as Texture2D;
		texture.SetPixels(pixels);
		texture.Apply();
		//reset = true;

		//Debug.Log("pixels reset");
	}


	void Update()
	{
		bool buttonDown = false;
#if !UNITY_EDITOR
		if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
			buttonDown = true;
        else
			buttonDown = false;
#endif


		/*HEAD RAY INPUT
		Ray ret = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
		ret.origin = ret.GetPoint(1.5f + .1f);
		ret.direction = -ret.direction;*/
		controllerRay.direction = controllerRight.transform.Find("dot").position - controllerRight.transform.Find("start").position;
		controllerRay.origin = controllerRight.transform.Find("start").position;
		//invert for 360 sphere
		controllerRay.origin = controllerRay.GetPoint(1.5f + .1f);
		controllerRay.direction = -controllerRay.direction;

		RaycastHit hit;
		//Debug.DrawRay(transform.position, ret.direction * 50, Color.green);
		//if(Physics.Raycast(ret.origin, ret.direction, out hit)
		if (Physics.Raycast(controllerRay.origin, controllerRay.direction, out hit))
		{
			//Debug.Log("hit");
			Renderer rend = hit.transform.GetComponent<Renderer>();
			MeshCollider meshCollider = hit.collider as MeshCollider;



			if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
				return;

			//Texture2D tex = rend.material.mainTexture as Texture2D;
			Vector2 pixelUV = hit.textureCoord;
			
			Texture2D test = rend.material.mainTexture as Texture2D;
			//WriteFile();
			//check each target's coordinates to see if they were hit
			for (int i = 0; i < targets.Count; i++)
			{			
				//Debug.Log("pixel hit: " + pixelUV.x + " " + pixelUV.y);
				if (pixelUV.x < targets[i].NormalTargetVertices[1].x && pixelUV.x > targets[i].NormalTargetVertices[0].x && pixelUV.y < targets[i].NormalTargetVertices[0].y && pixelUV.y > targets[i].NormalTargetVertices[2].y)
				{
					targets[i].Text = true;
					//if intersecting unhighlighted AOI or button triggers progress
					if ((!targets[i].Revealed && test.isReadable) || (targets[i].Order == TargetStep && buttonDown))
					{

						SetBorders(true, targets[i]); //currenttest
						targets[i].Revealed = true;

						//if (targets[i].Order == TargetStep) check for intersection & controller input
						if (targets[i].Order == TargetStep && buttonDown)
						{
							//progress in sequence
							TargetStep++;
							targets[i].Step = true;
							ResetIndex = -1;
							//Debug.Log("target step = " + TargetStep);
							//change text - loop through and find next prompt - should be initialized to prompt 1
						}
						else
						{
							//reset = false;
							
							ResetIndex = i; //once user looks away revealed boolean is reset to false
											//Debug.Log("reset index = " + ResetIndex);
							
						}

					}
				}
				else
				{
					if (i == ResetIndex)
					{
						//Debug.Log("reset would occur here");
						//Debug.Log("reset occurs");
						SetBorders(false, targets[i]); //currenttest
						targets[ResetIndex].Revealed = false;
						ResetIndex = -1;
					}
					//Debug.Log("else loop entered");
					targets[i].Text = false;

				}
			}




			//Debug.Log("Hit x = " + pixelUV.x + "Hit y = " + pixelUV.y);	
			/*if (Input.GetKeyDown(KeyCode.R))
			{
				ResetSequence();
			}*/
#if !UNITY_EDITOR
			if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyLongPressed(0, Pvr_UnitySDKAPI.Pvr_KeyCode.X))
				ResetSequence();
#endif
		}
		else
		{
			SetAllTextFalse();
		}

		SetText();

		/* eye gaze validation
		if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.B))
		{
			if (eyeSphere.activeSelf)
				eyeSphere.SetActive(false);
			else
				eyeSphere.SetActive(true);
		}
		*/
	}


	void ResetSequence()
    {
		for(int i = 0; i < targets.Count; i++)
        {
			//reset all targets to initialization defaults
			targets[i].Text = false;
			targets[i].Revealed = false;
			targets[i].Step = false;
        }
		//reset variables for sequential functionality
		TargetStep = 1;
		ResetIndex = -1;
		//reset pixel updates
		Reset(pix);
		Renderer rend = plane.GetComponent<Renderer>();
		Texture2D texture = rend.material.mainTexture as Texture2D;
	}

	void OnApplicationQuit()
    {
		Reset(pix);
		//sw.Close();
    }

	void OnApplicationPause()
	{
		//Reset(pix);
		//sw.Close();
	}


	/*
	
	// Start is called before the first frame update
    GameObject canvas;
    void Start()
    {
        canvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
		RaycastHit hit;
		// get the forward vector of the player's camera 
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		//create a ray using fwd vector as direction and a max size of 20.0f 
		//hit is the out parameter if something is detected
		if (Physics.Raycast(transform.position, fwd, out hit, 20.0F))
		{
			//if there something in our front, check if it's the monolith
			if (hit.collider.gameObject.name.Equals("monolith"))
			{
				//get the animator and set monolith parameter to true 
				canvas.GetComponent<Animator>().SetBool("monolithDetected", true);
				print(hit.collider.gameObject.name);
			}
			else
				//get the animator and set monolith parameter to false since it's not the monolith 
				canvas.GetComponent<Animator>().SetBool("monolithDetected", false);
		}
		else
			//get the animator and set monolith parameter to false since there is nothing in our front
			canvas.GetComponent<Animator>().SetBool("monolithDetected", false);
	}
	*/

	public int GetTargetStep()
	{
		return TargetStep;
	}
}


