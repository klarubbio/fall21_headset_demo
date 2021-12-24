using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEditor;

public class RayCast : MonoBehaviour
{
	public Material highlightMaterial, baseMaterial; //A few things that need to be set from the outside
	public TextMeshProUGUI textDisplayed;
	public TextMeshProUGUI tasks;

	//this is for ordering, we can probably use multi step stuff using math and setting if number < something.
	private double stepCounter;
	//public GameObject Sphere1\

	Info[] gameobjects, countobjects; 
	//Two different sets, one to show all objects in the scene, the other listing each object in our step-by-step order.

	Info[] GetAllObjectsOnlyInScene() //This sets the former
	{

		Info[] cubeComponents = FindObjectsOfType<Info>();
		///Probably don't need this function, but could modify 
        ///it if need be in order to look for objects of different types.

		/* Debug
		foreach (Info go in cubeComponents)
		{
			Debug.Log(go.gameObject.name);
			Debug.Log("Test Works");
		}
		*/
		return cubeComponents;
	}

	Info[] GetAllSteppedObjects() //This finds the latter of those two arrays
    {
		Info[] cubeComponents = FindObjectsOfType<Info>();
		List<Info> returnArray1 = new List<Info>();
		//Decided to use a list, the amount of objects in sequence might vary

		foreach (Info go in cubeComponents)
		{
			if (go.gameObject.GetComponent<Info>().stepPart)
            {
				returnArray1.Add(go);
            }				
		}

		Info[] returnArray = new Info[returnArray1.Count]; 
		//Decided to copy over to an array, probably isn't necessary

		for (int i = 0; i < returnArray1.Count; i++)
        {
			returnArray[i] = returnArray1.ElementAt(i);

        }

		return returnArray;
	}

	void Start()
	{
		stepCounter = 0;
		//SetText();
		Cursor.visible = true;


		gameobjects = GetAllObjectsOnlyInScene();
		// Set these two arrays
		countobjects = GetAllSteppedObjects();

		//Test
		/* Debug

		MyScript[] allFoundScripts = Resources.FindObjectsOfTypeAll<MyScript>();

		foreach (MyScript foundScript in allFoundScripts)
		{
			Debug.Log("Found the script in: " + foundScript.gameObject);

			// Select the script in the inspector, if you want to
			UnityEditor.Selection.activeGameObject = foundScript.gameObject;

			// You can also change variables on the found script
			foundScript.someVariable = 13;
			// Set dirty forces the inspector to save the change (there may be a better way to do this)
			UnityEditor.EditorUtility.SetDirty(foundScript);
		}
		*/
	}

	void ResetBut() // If r is pressed, resets the scene to start of sequence
	{
		foreach (Info go in gameobjects)
		{
			go.gameObject.transform.GetComponent<Renderer>().material = baseMaterial;
			go.gameObject.GetComponent<Info>().alreadySelected = false;
		}

		foreach (Info go in countobjects)
        {
			go.gameObject.GetComponent<Info>().stepPart = true; 
        }
		
		textDisplayed.text = "";
		stepCounter = 0; // This enables us to start at the beginning
	}

	void GenReset() //Will simply deselect anything that should not be selected (leaves already selected stuff in sequence highlighted)
    {
		foreach (Info go in gameobjects)
		{
			if (!go.gameObject.GetComponent<Info>().alreadySelected)
			{
				go.gameObject.transform.GetComponent<Renderer>().material = baseMaterial;
			}
		}

		textDisplayed.text = "";
	}



	void SetText()
	{
		//The steps are hardcoded in, with the increment on each object used to go through each step
		if (stepCounter < 1)
		{
			tasks.text = "Verify secured control room";
		}
		else if (stepCounter < 2)
		{
			tasks.text = "Initiate log-book notes";
		}
		else if (stepCounter < 3)
		{
			tasks.text = "Verify all status and alarm conditions\n (2)";
		}
		else if (stepCounter < 4)
		{
			tasks.text = "Access console controls with key";
		}
		else if (stepCounter < 5)
		{
			tasks.text = "Start power recorder and verify functionality";
		}
		else if (stepCounter < 6)
		{
			tasks.text = "Start moving out safety rod\n  (3)";
			//Should make it such that each thing needs to be looked at, would just
			//require making separate tags
			// Uses the increment here to allow for multiple - increment set to .34 to allow for 3 parts
		}
		else if (stepCounter < 7)
		{
			tasks.text = "Start moving out control blade";
		}
		else if (stepCounter < 8)
		{
			tasks.text = "Maintain suitable reactor period";
		}
		else if (stepCounter < 9)
		{
			tasks.text = "Reach desired steady power-level";
		}
		else if (stepCounter < 10)
		{
			tasks.text = "Activate auto-controls to maintain power-level";
		}
		else if (stepCounter < 11)
		{
			tasks.text = "You powered up the reactor safely";
		}

	}
	void Update()
	{
		RaycastHit hit;

		SetText(); // Set what the step is.
		GenReset(); // Resets the textDisplayed and deselects something that is not part of sequence.

		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit)) // The basic raycasting works
		{
			//prep for color change ability
			var selection = hit.transform;
			var selectionRenderer = selection.GetComponent<Renderer>();

			foreach (Info go in gameobjects)
			{
				if (go.gameObject == hit.collider.gameObject) // find which object the ray is hitting
				{
					var theObject = go.gameObject.GetComponent<Info>(); // for simplicity sake
					textDisplayed.text = theObject.name; //Display name at top
					selectionRenderer.material = highlightMaterial; // change to highlighted texture
					//Add step stuff I believe

					if (stepCounter + 1 >= theObject.stepOrder && theObject.stepPart) // This allows for step changing
                    {
						stepCounter += (double)theObject.increment; // Basis here allows for settext to change the instructions
						theObject.stepPart = false; // Cannot be added again
						theObject.alreadySelected = true; // Gives signal to not change back to deselected in GenReset function
                    }
					break; // Don't need to keep looking -- POTENTIAl BUG here.
					//This break might not be placed correctly - could save time if placed correctly
				}
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				ResetBut();
			}
		}

		
	}
}
