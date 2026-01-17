using System.Collections.Generic;
using UnityEngine;

namespace PPF.Core.Timeline
{
    /// <summary>
    /// Records Transform snapshots for TimelineTrackable objects into ring buffers.
    /// TL-010 scope:
    /// - Trackables can self-register at runtime (supports network spawning).
    /// - Records snapshots at SamplesPerSecond.
    /// - Exposes query by time (closest snapshot).
    /// </summary>
    public sealed class TimelineService : MonoBehaviour
    {
        public static TimelineService Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private TimelineConfig _config;

        [Header("Tracking")]
        [SerializeField] private bool _autoDiscoverOnStart = true;

        private readonly Dictionary<TimelineTrackable, TimelineRingBuffer> _buffers = new();
        private float _nextSampleTime;

        public TimelineConfig Config => _config;
        public int TrackedCount => _buffers.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (_config == null)
            {
                UnityEngine.Debug.LogError("TimelineService: TimelineConfig is not assigned.");
            }
        }

        private void Start()
        {
            if (_autoDiscoverOnStart)
                DiscoverTrackables();

            ResetSampler();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            if (_config == null)
                return;

            float now = Time.time;

            if (now >= _nextSampleTime)
            {
                SampleAll(now);
                _nextSampleTime = now + (1f / _config.SamplesPerSecond);
            }
        }

        public void DiscoverTrackables()
        {
            var found = FindObjectsByType<TimelineTrackable>(FindObjectsSortMode.None);
            foreach (var t in found)
            {
                Register(t);
            }
        }

        public void Register(TimelineTrackable trackable)
        {
            if (trackable == null)
                return;

            if (_buffers.ContainsKey(trackable))
                return;

            int capacity = ComputeCapacity();
            _buffers.Add(trackable, new TimelineRingBuffer(capacity));
        }

        public void Unregister(TimelineTrackable trackable)
        {
            if (trackable == null)
                return;

            _buffers.Remove(trackable);
        }

        public bool TryGetSnapshot(TimelineTrackable trackable, float targetTime, out TransformSnapshot snapshot)
        {
            snapshot = default;
            if (trackable == null)
                return false;

            if (_buffers.TryGetValue(trackable, out var buffer))
            {
                return buffer.TryGetClosest(targetTime, out snapshot);
            }

            return false;
        }

        public float GetPastViewTime(float now)
        {
            if (_config == null)
                return now;

            return now - _config.PastDelaySeconds;
        }

        public (float oldest, float newest) GetTimeRange()
        {
            float oldest = Time.time;
            float newest = Time.time;

            bool any = false;
            foreach (var kv in _buffers)
            {
                var buf = kv.Value;
                float o = buf.GetOldestTimeOr(Time.time);
                float n = buf.GetNewestTimeOr(Time.time);

                if (!any)
                {
                    oldest = o;
                    newest = n;
                    any = true;
                }
                else
                {
                    if (o < oldest) oldest = o;
                    if (n > newest) newest = n;
                }
            }

            return (oldest, newest);
        }

        private void SampleAll(float now)
        {
            foreach (var kv in _buffers)
            {
                var trackable = kv.Key;
                if (trackable == null)
                    continue;

                var tr = trackable.TargetTransform;
                kv.Value.Add(new TransformSnapshot(now, tr.position, tr.rotation));
            }
        }

        private int ComputeCapacity()
        {
            if (_config == null)
                return 1;

            int capacity = Mathf.CeilToInt(_config.SamplesPerSecond * _config.HistorySeconds);
            return Mathf.Max(1, capacity);
        }

        private void ResetSampler()
        {
            if (_config == null)
                return;

            _nextSampleTime = Time.time + (1f / _config.SamplesPerSecond);
        }
    }
}
