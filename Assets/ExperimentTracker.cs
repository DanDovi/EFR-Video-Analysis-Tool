using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ExperimentTracker : MonoBehaviour
{
   
    public List<PelletOccurence> pelletOccurrences = new List<PelletOccurence>();
    public List<ChowOccurence> chowOccurrences = new List<ChowOccurence>();
    public List<MouseEvent> mouseEvents = new List<MouseEvent>();

    public mouseJump mouseJump;
    
    public Button exportButton;
    
    public Text pelletCountText;
    public Text chowCountText;
    public Text lastChowTimeText;
    [FormerlySerializedAs("timeSinceStart")] public Text timeSinceStartText;

    public Animator mouseAnimator;
    
    public Dropdown timerDropdown;
    
    public bool timingChow;
    
    private System.Diagnostics.Stopwatch _stopwatch;
    private int _pelletCount;
    private int _chowCount;
    private float _lastChowTime;
    private float _timer;
    private int _videoSpeedMultiplier = 1;

    private float TimerLimit => 30.0f * 60.0f / _videoSpeedMultiplier;
    
    private bool _timerIsRunning;
    
    private float _chowTimer;

 

    // Use this for initialization
    private void Start()
    {
        pelletCountText.text = _pelletCount.ToString();
        chowCountText.text = _chowCount.ToString();
        lastChowTimeText.text = _lastChowTime.ToString(CultureInfo.InvariantCulture);
        _stopwatch = new System.Diagnostics.Stopwatch();

        timeSinceStartText.text = "Seconds Since Start: " + _stopwatch.ElapsedMilliseconds * _videoSpeedMultiplier;

        timerDropdown.onValueChanged.AddListener(delegate
        {
            TimerDropdownValueChangedHandler(timerDropdown);
        });
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (_timerIsRunning)
        {
            if (_timer >= TimerLimit)
                StopTimer();
            else
            {
                // timer += Time.deltaTime * videoSpeedMultiplier;

                if (Input.GetKeyUp(KeyCode.A))
                {
                    pelletOccurrences.Add(new PelletOccurence());
                    pelletOccurrences[_pelletCount].time = GetTimeSinceStart();
                    mouseEvents.Add(new MouseEvent(pelletOccurrences[_pelletCount].time, "pellet"));
                   
                    Debug.Log("pellet logged");
                    _pelletCount++;
                }

                if (Input.GetKeyUp(KeyCode.B))
                {
                    if (!timingChow)
                    {
                        chowOccurrences.Add(new ChowOccurence());
                        chowOccurrences[_chowCount].timeStart = GetTimeSinceStart();
                        timingChow = true;
                        mouseEvents.Add(new MouseEvent(chowOccurrences[_chowCount].timeStart, "chow start"));
                        _chowTimer = 0.0f;
                    }
                    else
                    {
                        float timeSinceStart = GetTimeSinceStart();
                        mouseEvents.Add(new MouseEvent(timeSinceStart, "chow end"));
                        chowOccurrences[_chowCount].duration = timeSinceStart - chowOccurrences[_chowCount].timeStart;
                        timingChow = false;
                        _lastChowTime = chowOccurrences[_chowCount].duration;
                        _chowCount++;
                        chowCountText.text = _chowCount.ToString();
                        lastChowTimeText.text = _lastChowTime.ToString(CultureInfo.InvariantCulture);


                    }
                }
                if (timingChow)
                {
                    _chowTimer = GetTimeSinceStart() - (chowOccurrences[_chowCount].timeStart);
                    lastChowTimeText.text = _chowTimer.ToString(CultureInfo.InvariantCulture);
                }

                pelletCountText.text = _pelletCount.ToString();
                chowCountText.text = _chowCount.ToString();
                timeSinceStartText.text = "Seconds Since Start: " + GetTimeSinceStart().ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    public void StartTimer()
    {
        if (!_timerIsRunning)
        {
            ResetTimer();
            _timerIsRunning = true;
        }

        exportButton.enabled = false;
        mouseAnimator.SetBool("ExperimentRunning", true);
        mouseJump.canJump = true;
        _stopwatch.Start();
    }

    public void StopTimer()
    {
        _timerIsRunning = false;
        exportButton.enabled = true;
        mouseAnimator.SetBool("ExperimentRunning", false);
        _stopwatch.Stop();
        mouseJump.canJump = false;
    }

    public void ResetTimer()
    {
        _timerIsRunning = false;
        _timer = 0;
        _pelletCount = 0;
        _chowCount = 0;
        _lastChowTime = 0;
        pelletOccurrences.Clear();
        chowOccurrences.Clear();
        mouseEvents.Clear();

        pelletCountText.text = _pelletCount.ToString();
        chowCountText.text = _chowCount.ToString();
        timeSinceStartText.text = "Time Since Start: " + (_timer * _videoSpeedMultiplier).ToString(CultureInfo.InvariantCulture);
        lastChowTimeText.text = _lastChowTime.ToString(CultureInfo.InvariantCulture);

        _stopwatch.Reset();
        exportButton.enabled = true;
        mouseAnimator.SetBool("ExperimentRunning", false);
        mouseJump.canJump = false;
    }

    private void TimerDropdownValueChangedHandler(Dropdown target)
    {
        _videoSpeedMultiplier = target.options[target.value].text[0] - '0';
    }

    private float GetTimeSinceStart()
    {
        return _stopwatch.ElapsedMilliseconds / 1000.0f * _videoSpeedMultiplier;
    }
}

[System.Serializable]
public class ChowOccurence
{
    public float timeStart;
    public float duration;
}

[System.Serializable]
public class PelletOccurence
{
    public float time;
}

[System.Serializable]
public class MouseEvent: IEquatable<MouseEvent> , IComparable<MouseEvent>
{
    public float time;
    public string type;

    public MouseEvent(float time, string type)
    {
        this.time = time;
        this.type = type;
    }

    public bool Equals(MouseEvent other)
    {
        return time == other.time && type == other.type;
    }

    public int CompareTo(MouseEvent other)
    {
        return time > other.time ? 1 : -1;
    }
}

