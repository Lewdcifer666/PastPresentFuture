using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PPF.UI.Debug
{
    /// <summary>
    /// Displays simple timeline debug info on screen.
    /// Auto-wires TimelineService and a TMP_Text at runtime to avoid manual scene hookups.
    /// </summary>
    public sealed class TimelineDebugHud : MonoBehaviour
    {
        [Header("Optional (auto-wired if left empty)")]
        [SerializeField] private PPF.Core.Timeline.TimelineService _timeline;
        [SerializeField] private TMP_Text _text;

        [Header("HUD")]
        [SerializeField] private bool _visible = true;
        [SerializeField] private float _updateInterval = 0.1f;

        private float _nextUpdateTime;
        private readonly StringBuilder _sb = new StringBuilder(256);

        private void Awake()
        {
            // Auto-find timeline service if not assigned
            if (_timeline == null)
                _timeline = FindAnyObjectByType<PPF.Core.Timeline.TimelineService>(FindObjectsInactive.Include);

            // Auto-find a TMP_Text if not assigned
            if (_text == null)
                _text = FindAnyObjectByType<TMP_Text>(FindObjectsInactive.Include);

            // If still missing, create a minimal TMP HUD under a Canvas
            if (_text == null)
                _text = CreateHudText();
        }

        private void OnEnable()
        {
            _nextUpdateTime = 0f;
            SetVisible(_visible);
        }

        private void Update()
        {
            if (!_visible)
                return;

            if (Time.unscaledTime < _nextUpdateTime)
                return;

            _nextUpdateTime = Time.unscaledTime + Mathf.Max(0.02f, _updateInterval);

            // Re-acquire timeline if scene changed and reference was lost
            if (_timeline == null)
                _timeline = FindAnyObjectByType<PPF.Core.Timeline.TimelineService>(FindObjectsInactive.Include);

            if (_text == null)
                return;

            if (_timeline == null)
            {
                _text.text = "Timeline: (missing)";
                return;
            }

            // Keep this intentionally generic so it won't break if TimelineService changes.
            _sb.Clear();
            _sb.AppendLine("Timeline: (ok)");
            _sb.Append("Time: ").Append(Time.time.ToString("0.00")).AppendLine();
            _sb.Append("Unscaled: ").Append(Time.unscaledTime.ToString("0.00")).AppendLine();

            _text.text = _sb.ToString();
        }

        public void SetVisible(bool visible)
        {
            _visible = visible;
            if (_text != null)
                _text.gameObject.SetActive(visible);
        }

        private static TMP_Text CreateHudText()
        {
            // Find or create a canvas
            Canvas canvas = FindAnyObjectByType<Canvas>(FindObjectsInactive.Include);
            if (canvas == null)
            {
                GameObject canvasGo = new GameObject(
                    "DebugCanvas",
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster)
                );

                canvas = canvasGo.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = short.MaxValue;

                DontDestroyOnLoad(canvasGo);
            }

            // Create TMP text
            GameObject textGo = new GameObject("TimelineHUDText", typeof(TextMeshProUGUI));
            textGo.transform.SetParent(canvas.transform, false);

            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.raycastTarget = false;
            tmp.fontSize = 22;

            // Anchor top-left
            RectTransform rt = tmp.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(12f, -12f);
            rt.sizeDelta = new Vector2(800f, 200f);

            DontDestroyOnLoad(textGo);
            return tmp;
        }
    }
}
