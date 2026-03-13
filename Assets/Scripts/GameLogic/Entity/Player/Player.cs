using UnityEngine;

namespace Entity.Player
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}
