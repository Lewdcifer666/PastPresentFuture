using UnityEngine;

namespace PPF.Core.Bootstrap
{
    /// <summary>
    /// Ensures the Boot root (and its children like GameplaySystems) survive scene loads.
    /// Also prevents duplicates if the Boot scene is ever loaded again.
    /// </summary>
    public sealed class PersistentRoot : MonoBehaviour
    {
        private static PersistentRoot _instance;

        private void Awake()
        {
            // If another instance already exists, destroy this duplicate.
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
