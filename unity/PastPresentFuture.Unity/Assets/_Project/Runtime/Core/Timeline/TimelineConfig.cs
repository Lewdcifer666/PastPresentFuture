using UnityEngine;

namespace PPF.Core.Timeline
{
    [CreateAssetMenu(menuName = "PPF/Timeline/Timeline Config", fileName = "SO_TimelineConfig")]
    public sealed class TimelineConfig : ScriptableObject
    {
        [Header("Recording")]
        [Tooltip("How many snapshots per second per tracked object.")]
        [Min(1)]
        public int SamplesPerSecond = 15;

        [Tooltip("How many seconds of history to keep in the ring buffer.")]
        [Min(1f)]
        public float HistorySeconds = 6f;

        [Header("Role View Offsets")]
        [Tooltip("How far behind 'Past' should see. Used later for rendering proxies.")]
        [Min(0f)]
        public float PastDelaySeconds = 1.5f;

        [Header("Debug")]
        public bool EnableDebugHud = true;
    }
}
