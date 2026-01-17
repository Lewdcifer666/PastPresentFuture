using System;
using UnityEngine;

namespace PPF.Core.Timeline
{
    /// <summary>
    /// Fixed-size ring buffer for TransformSnapshot.
    /// Supports query by target time using nearest (or simple linear scan).
    /// TL-010 uses a simple scan for correctness; we can optimize later.
    /// </summary>
    public sealed class TimelineRingBuffer
    {
        private readonly TransformSnapshot[] _buffer;
        private int _count;
        private int _head; // next write index

        public int Capacity => _buffer.Length;
        public int Count => _count;

        public TimelineRingBuffer(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _buffer = new TransformSnapshot[capacity];
            _count = 0;
            _head = 0;
        }

        public void Add(in TransformSnapshot snapshot)
        {
            _buffer[_head] = snapshot;
            _head = (_head + 1) % _buffer.Length;

            if (_count < _buffer.Length)
                _count++;
        }

        /// <summary>
        /// Returns the snapshot closest to the target time.
        /// If buffer is empty, returns false.
        /// </summary>
        public bool TryGetClosest(float targetTime, out TransformSnapshot snapshot)
        {
            snapshot = default;
            if (_count == 0)
                return false;

            // Scan all valid entries.
            float bestDelta = float.MaxValue;
            bool found = false;

            for (int i = 0; i < _count; i++)
            {
                int index = (_head - 1 - i);
                if (index < 0) index += _buffer.Length;

                var s = _buffer[index];
                float delta = Mathf.Abs(s.Time - targetTime);
                if (delta < bestDelta)
                {
                    bestDelta = delta;
                    snapshot = s;
                    found = true;
                }
            }

            return found;
        }

        public float GetNewestTimeOr(float fallback)
        {
            if (_count == 0)
                return fallback;

            int newestIndex = _head - 1;
            if (newestIndex < 0) newestIndex += _buffer.Length;
            return _buffer[newestIndex].Time;
        }

        public float GetOldestTimeOr(float fallback)
        {
            if (_count == 0)
                return fallback;

            // Oldest is at head - count
            int oldestIndex = _head - _count;
            while (oldestIndex < 0) oldestIndex += _buffer.Length;
            return _buffer[oldestIndex].Time;
        }
    }
}
