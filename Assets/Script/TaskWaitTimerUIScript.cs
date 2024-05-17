using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Show task Timer Stack in UI
public class TaskWaitTimerUIScript : MonoBehaviour
{
    private TMP_Text timerText;
    public ScriptableObjectScript script_scriptable;
    public ObjConditionScript script_objCon;
    public Dictionary<int, Timer> global_taskTimerActive = new Dictionary<int, Timer>();
    public Dictionary<int, Timer> global_acTimerActive = new Dictionary<int, Timer>();
    
    // Start is called before the first frame update
    void Start()
    {
        //Find objTimer in canvas
        timerText = GameObject.Find("ObjTimer").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update and display timers
        UpdateTimers();
    }

    // Start a new timer
    public void StartTimer(float duration, int index, bool isAC)
    {
        if(isAC)
        {
            if (!global_acTimerActive.ContainsKey(index))
            {
                Timer newTimer = new Timer(duration);
                global_acTimerActive.Add(index, newTimer);
            }
        } else
        {
            if (!global_taskTimerActive.ContainsKey(index))
            {
                Timer newTimer = new Timer(duration);
                global_taskTimerActive.Add(index, newTimer);
            }
        }
        
    }

    // Update and display all timers
    void UpdateTimers()
    {
        // Remove finished timers
        List<int> keysToRemove = new List<int>();
        List<int> acKeysToRemove = new List<int>();
        foreach (var pair in global_taskTimerActive)
        {
            int index = pair.Key;
            Timer timer = pair.Value;
            
            if (timer.IsFinished || !script_scriptable.global_tronicDataList[index].tronic_active_Q || !script_scriptable.global_tronicDataList[index].tronic_correct_Q)
            {
               
                    keysToRemove.Add(index);
            }
        }

        foreach (var pair in global_acTimerActive)
        {
            int index = pair.Key;
            Timer timer = pair.Value;

            if (timer.IsFinished || !script_scriptable.global_tronicDataList[index].tronic_active_Q ||  ObjConditionScript.global_acStatsIndex[index] != ObjConditionScript.acObjBehaviour.correct || !script_scriptable.global_tronicDataList[index].tronic_correct_Q)
            {
                acKeysToRemove.Add(index);
            }
        }


        foreach (int index in keysToRemove)
        {
            global_taskTimerActive.Remove(index);
        }

        foreach (int index in acKeysToRemove)
        {
            global_acTimerActive.Remove(index);
        }

        // Display active timers in UI
        DisplayTimers();

        // Update each timer
        foreach (var pair in global_taskTimerActive)
        {
            pair.Value.UpdateTimer(Time.deltaTime);
        }

        foreach (var pair in global_acTimerActive)
        {
            pair.Value.UpdateTimer(Time.deltaTime);
        }
    }

    // Display active timers in UI
    void DisplayTimers()
    {
        if (timerText != null)
        {
            // Clear existing text
            timerText.text = "";

            // Display each active timer
            foreach (var pair in global_taskTimerActive)
            {
                int index = pair.Key;
                string nameObj = script_scriptable.global_tronicDataList[index].tronic_name;
                Timer timer = pair.Value;
                timerText.text += $"{nameObj} Timer: {timer.Duration - timer.ElapsedTime:F1}s\n";
            }

            foreach (var pair in global_acTimerActive)
            {
                int index = pair.Key;
                string nameObj = script_scriptable.global_tronicDataList[index].tronic_name;
                Timer timer = pair.Value;
                timerText.text += $"{nameObj} Timer: {timer.Duration - timer.ElapsedTime:F1}s\n";
            }
        }
    }
}

// Timer class to track elapsed time
public class Timer
{
    public float Duration { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsFinished => ElapsedTime >= Duration;

    public Timer(float duration)
    {
        Duration = duration;
        ElapsedTime = 0f;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (!IsFinished)
        {
            ElapsedTime += deltaTime;
        }
    }
}
