using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tagSelect : MonoBehaviour
{
    private static objectFunction script_objectFunction;
    private static touchScript script_touchScript;
    private static buttonScript script_button;
    private static camera script_camera;

     void Start()
    {
        script_objectFunction = GetComponent<objectFunction>();
        script_touchScript = GetComponent<touchScript>();
        script_camera = GetComponent<camera>();

    }
    public void select(string Tag)
    {
        
        switch (Tag)
        {
            case "Switch":
                print("tag : switch");
                script_button = script_touchScript.selectedObject.GetComponent<buttonScript>();
                if (script_button.buttonOpen == true)
                {
                    print("switch pressed");
                    script_button.selectObject();
                    script_objectFunction.ChangeObjectMaterial(script_touchScript.selectedObject);
                }
                break;
            case "Selectable":
                print("tag : selectable");
                print("camera changed");
                script_camera.ActivateCamera(script_touchScript.selectedObject);
                script_button = script_touchScript.selectedObject.GetComponentInChildren<buttonScript>();
                script_button.buttonOpen = true;
                break;
            default:
                print($"tag : {Tag}");
                break;
        }
    }
 }

