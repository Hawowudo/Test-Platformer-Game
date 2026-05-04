using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardTimer : MonoBehaviour
{
    private Blackboard _blackboard;
    private List<string> _timerNames;

    private void Awake()
    {
        _blackboard = GetComponent<Blackboard>();
        _timerNames = new List<string>();
    }
    public void AddTimerToBlackboard(string key, float value = 0.01f)
    {
        _blackboard.Set<float>(key, (float)value);
        _timerNames.Add(key);
    }
    public void StopTimer(string key)
    {
        if (_blackboard.HasKey(key))
        {
            _blackboard.Set<float>(key, 0f);
            _timerNames.Remove(key);
        }
    }
    public void StopAllTimers()
    {
        foreach (string timerName in _timerNames)
        {
            if (_blackboard.HasKey(timerName))
            {
                _blackboard.Set<float>(timerName, 0f);
            }
        }
        _timerNames.Clear();
    }


    private void Update()
    {
        foreach (string timerName in _timerNames)
        {
            if (_blackboard.HasKey(timerName))
            {
                float currentValue = _blackboard.Get<float>(timerName);
                _blackboard.Set(timerName, currentValue + Time.deltaTime);

            }
        }
    }

}
