using UnityEngine;

namespace PPF.Core.Timeline
{
    /// <summary>
    /// Attach this to any object that should be recorded by the TimelineService.
    /// Self-registers so network-spawned objects are tracked automatically.
    /// </summary>
    public sealed class TimelineTrackable : MonoBehaviour
    {
        [Tooltip("Optional stable id for debugging. Leave empty to use instance id.")]
        [SerializeField] private string _debugId;

        public string DebugId => string.IsNullOrWhiteSpace(_debugId) ? $"Instance:{gameObject.GetInstanceID()}" : _debugId;
        public Transform TargetTransform => transform;

        private void OnEnable()
        {
            // TimelineService is spawned from Boot, so this should exist in normal gameplay.
            if (TimelineService.Instance != null)
            {
                TimelineService.Instance.Register(this);
            }
        }

        private void OnDisable()
        {
            if (TimelineService.Instance != null)
            {
                TimelineService.Instance.Unregister(this);
            }
        }
    }
}
