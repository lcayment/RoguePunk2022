using Roguelike_BAIETTO_CAYMENT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike_BAIETTO_CAYMENT.Systems
{
    // Cette classe a été directement récupérée du tutoriel roguesharp
    // https://roguesharp.wordpress.com/2016/08/27/roguesharp-v3-tutorial-scheduling-system/

    public class SchedulingSystem
    {
        private int _time;
        private readonly SortedDictionary<int, List<ISpeed>> _scheduleables;

        public SchedulingSystem()
        {
            _time = 0;
            _scheduleables = new SortedDictionary<int, List<ISpeed>>();
        }

        // Add a new object to the schedule 
        // Place it at the current time plus the object's Time property.
        public void Add(ISpeed scheduleable)
        {
            int key = _time + scheduleable.Time;
            if (!_scheduleables.ContainsKey(key))
            {
                _scheduleables.Add(key, new List<ISpeed>());
            }
            _scheduleables[key].Add(scheduleable);
        }

        // Remove a specific object from the schedule.
        // Useful for when an enemy is killed to remove it before it's action comes up again.
        public void Remove(ISpeed scheduleable)
        {
            KeyValuePair<int, List<ISpeed>> scheduleableListFound
                = new KeyValuePair<int, List<ISpeed>>(-1, null);

            foreach (var scheduleablesList in _scheduleables)
            {
                if (scheduleablesList.Value.Contains(scheduleable))
                {
                    scheduleableListFound = scheduleablesList;
                    break;
                }
            }
            if (scheduleableListFound.Value != null)
            {
                scheduleableListFound.Value.Remove(scheduleable);
                if (scheduleableListFound.Value.Count <= 0)
                {
                    _scheduleables.Remove(scheduleableListFound.Key);
                }
            }
        }

        // Get the next object whose turn it is from the schedule. Advance time if necessary
        public ISpeed Get()
        {
            var firstScheduleableGroup = _scheduleables.First();
            var firstScheduleable = firstScheduleableGroup.Value.First();
            Remove(firstScheduleable);
            _time = firstScheduleableGroup.Key;
            return firstScheduleable;
        }

        // Get the current time (turn) for the schedule
        public int GetTime()
        {
            return _time;
        }

        // Reset the time and clear out the schedule
        public void Clear()
        {
            _time = 0;
            _scheduleables.Clear();
        }
    }
}
