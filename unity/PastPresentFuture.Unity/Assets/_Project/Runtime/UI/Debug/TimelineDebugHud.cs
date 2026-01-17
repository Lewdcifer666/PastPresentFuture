using TMPro;
using UnityEngine;
using PPF.Core.Timeline;

namespace PPF.UI.Debug
{
    /// <summary>
    /// Debug HUD for TL-010.
    /// Shows timeline recording stats and view time offsets.
    /// </summary>
    public sealed class TimelineDebugHud : MonoBehaviour
    {
        [SerializeField] private TimelineService _timeline;
        [SerializeField] private TMP_Text _text;

        private void Awake()
        {
            if (_timeline == null)
            {
                _timeline = FindFirstObjectByType<TimelineService>();
            }

            if (_text == null)
            {
                UnityEngine.Debug.LogError("TimelineDebugHud: TMP_Text not assigned.");
            }
        }

        private void Update()
        {
            if (_timeline == null || _timeline.Config == null)
            {
                Set("Timeline: (missing)");
                return;
            }

            if (!_timeline.Config.EnableDebugHud)
            {
                Set(string.Empty);
                return;
            }

            float now = Time.time;
            float pastView = _timeline.GetPastViewTime(now);
            var range = _timeline.GetTimeRange();

            string msg =
                $"Timeline Debug\n" +
                $"Now: {now:0.000}\n" +
                $"PastViewTime: {pastView:0.000} (Delay: {_timeline.Config.PastDelaySeconds:0.00}s)\n" +
                $"SamplesPerSecond: {_timeline.Config.SamplesPerSecond}\n" +
                $"HistorySeconds: {_timeline.Config.HistorySeconds:0.00}\n" +
                $"Tracked: {_timeline.TrackedCount}\n" +
                $"Range: {range.oldest:0.000} → {range.newest:0.000}\n";

            Set(msg);
        }

        private void Set(string value)
        {
            if (_text != null && _text.text != value)
                _text.text = value;
        }
    }
}
