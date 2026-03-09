using System;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;

namespace Assets.StompyRobot.SRDebugger.Scripts.Services.Implementation
{
    [Service(typeof(IConsoleFilterState))]
    public sealed class ConsoleFilterStateService : IConsoleFilterState
    {
        public event ConsoleStateChangedEventHandler FilterStateChange;
        public event ConsoleTextFilterChangedEventHandler TextFilterChange;

        private readonly bool[] _states;
        private string _textFilter = "";

        public ConsoleFilterStateService()
        {
            _states = new bool[Enum.GetValues(typeof(LogType)).Length];
            for (var i = 0; i < _states.Length; i++)
            {
                _states[i] = true;
            }
        }

        public void SetConsoleFilterState(LogType type, bool newState)
        {
            type = GetType(type);
            if (_states[(int)type] == newState)
            {
                return;
            }

            //Debug.Log($"FilterState changed {type} {!newState} -> {newState}");

            _states[(int)type] = newState;
            FilterStateChange?.Invoke(type, newState);
        }

        public bool GetConsoleFilterState(LogType type)
        {
            type = GetType(type);
            return _states[(int)type];
        }

        public string TextFilter
        {
            get => _textFilter;
            set {
                if (value != _textFilter)
                {
                    _textFilter = value;
                    TextFilterChange?.Invoke(value);
                }
            }
        }

        private static LogType GetType(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    return LogType.Error;
                default:
                    return type;
            }
        }
    }
}