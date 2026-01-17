using UnityEngine;

namespace PPF.Util
{
    /// <summary>
    /// Temporary motion helper to make timeline delay obvious.
    /// Remove after TL-020 validation.
    /// </summary>
    public sealed class AutoWiggle : MonoBehaviour
    {
        [SerializeField] private float _amplitude = 1f;
        [SerializeField] private float _speed = 1f;

        private Vector3 _start;

        private void Awake()
        {
            _start = transform.position;
        }

        private void Update()
        {
            float x = Mathf.Sin(Time.time * _speed) * _amplitude;
            transform.position = _start + new Vector3(x, 0f, 0f);
        }
    }
}
