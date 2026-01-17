using UnityEngine;

namespace PPF.Core.Timeline
{
    public struct TransformSnapshot
    {
        public float Time;         // Time.time at record moment
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformSnapshot(float time, Vector3 position, Quaternion rotation)
        {
            Time = time;
            Position = position;
            Rotation = rotation;
        }
    }
}
