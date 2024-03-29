using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class touchCode : MonoBehaviour
{
    private cameraState script_cameraState;
    private objCondition script_objCondition;
    private GameObject selectedObject;
    private Collider objCollider;
    private gameState script_gameState;
    public objValue objVal;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    private levelToActivate script_levelToActivate;
    private objToActivate script_objToActivate;
    public bool secLevel = false;

    void Awake()
    {
        script_cameraState = GetComponent<cameraState>();
        script_objCondition = GetComponent<objCondition>();
        script_gameState = GetComponent<gameState>();

    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Assuming you're interested in the first touch

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                startTouchPos = touch.position;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedObject = hit.transform.gameObject;

                    selectedObject = touchedObject;
                    print($"hit {touchedObject.tag}");

                    select(touchedObject.tag);
                }
            }

            if(touch.phase == TouchPhase.Moved && script_cameraState.currentVirtualCamera.CompareTag("firVirtualCamera"))
            {
               script_objToActivate.obj.transform.Rotate(0, -touch.deltaPosition.x * script_objToActivate.turnspeed * Time.deltaTime,0);
            }
            
            if(touch.phase == TouchPhase.Ended)
            {
                endTouchPos = touch.position;
                if(script_cameraState.currentVirtualCamera.CompareTag("mainVirtualCamera"))
                {
                    if(startTouchPos.y < Screen.height/2)
                    {
                        if (endTouchPos.x < startTouchPos.x)
                        {
                            Debug.Log("Next Swipe");
                            script_cameraState.nextSwipe();
                        }

                        if (endTouchPos.x > startTouchPos.x)
                        {
                            Debug.Log("Prev Swipe");
                            script_cameraState.prevSwipe();
                        }
                    }   
                }
                
            }
        }
    }

    IEnumerator textPopUp()
    {
       Debug.Log("Text Pop Up!");
       float elapsedTime = 0f;
       textToActivate script_textToActivate = selectedObject.GetComponent<textToActivate>();
       while (elapsedTime < script_textToActivate.timer)
       {
          // Execute your function here
          script_textToActivate.textToShow.text = script_textToActivate.text;

          // Wait for the next frame
          yield return null;

          // Update the elapsed time
          elapsedTime += Time.deltaTime;
       }

        script_textToActivate.textToShow.text = "";
    }

    void changeValue(int val)
    {
        objVal.defValue += (objVal.changeVal * val);
    }

    public void activateObj()
    { 
        if(selectedObject.GetComponentInParent<objValue>() != null)
            {
                objVal = selectedObject.GetComponentInParent<objValue>();
                objVal._objActivate = true;
                Debug.Log("objActivated");
            }
    }
    public void deactivateObj()
    {
        if(objVal != null)
        {
            objVal._objActivate = false;
            Debug.Log("objDeactivated");
        }
        
    }

    public void activateSecLevel()
    {
        if (selectedObject.GetComponent<levelToActivate>() != null)
        {
            script_levelToActivate = selectedObject.GetComponent<levelToActivate>();
            if (!script_cameraState.currentVirtualCamera.CompareTag("mainVirtualCamera"))
            {
                script_levelToActivate.objCol.enabled = true;
                objCollider = selectedObject.GetComponent<Collider>();
                objCollider.enabled = false;
            }
        }
    }
    public void deactiveSecLevel()
    {
        if(script_cameraState.currentVirtualCamera.CompareTag("mainVirtualCamera"))
        {
            script_levelToActivate.objCol.enabled = false;
            objCollider.enabled = true;
        }
       
    }

    public void select(string Tag)
    {

        switch (Tag)
        {
            case "firstLevel":
                print($"tag : {Tag}");
                script_cameraState.activateNewCamera(selectedObject);
                script_cameraState.ChangeState(cameraState.state.first);
                activateObj();
                activateSecLevel();

                if(selectedObject.GetComponent<objToActivate>() != null)
                {
                    script_objToActivate = selectedObject.GetComponent<objToActivate>();
                }
                break;
            case "secondLevel":
                print($"tag : {Tag}");
                script_cameraState.activateNewCamera(selectedObject);
                script_cameraState.ChangeState(cameraState.state.second);
                activateObj();
                activateSecLevel();

                if (selectedObject.GetComponent<objToActivate>() != null)
                {
                    script_objToActivate = selectedObject.GetComponent<objToActivate>();
                }
                break;
            case "plusButton":
                print($"tag : {Tag}");
                changeValue(1);
                break;
            case "minButton":
                print($"tag : {Tag}");
                changeValue(-1);
                break;
            case "Switch":
                print($"tag : {Tag}");
                script_objCondition.objSwitch(selectedObject);
                break;
            case "Electricity":
                print($"tag : {Tag}");
                script_objCondition.eleButton();
                break;
            case "changeScene":
                print($"tag : {Tag}");
                SceneManager.LoadSceneAsync(2);
                script_objCondition.done = false;
                script_objCondition.deactivateObj();
                break;
            case "backScene":
                print($"tag : {Tag}");
                SceneManager.LoadSceneAsync(1);
                script_objCondition.done = false;
                script_objCondition.activateObj();
                script_cameraState.activateMove(script_cameraState.currentCameraIndex);
                break;
            case "Selectable":
                print($"tag : {Tag}");
                if (selectedObject.GetComponent<textToActivate>() != null)
                {
                    StartCoroutine(textPopUp());
                }
                break;
            default:
                print($"tag : {Tag}");
                break;
        }
    }

}
