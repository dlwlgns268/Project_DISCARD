using UnityEngine;

namespace GameLogic.Entity.Player
{
    public class Execution : MonoBehaviour
    {
        public GameObject Target { get; private set; }
        public LineRenderer executionLine;

        public void FixedUpdate()
        {
            Target = GetNearestExecutionTarget();
            executionLine.enabled = Target;
            if (!Target) return;
            var direction = Target.transform.position - transform.position;
            executionLine.SetPosition(0, transform.position + direction.normalized * 0.5f);
            executionLine.SetPosition(1, Target.transform.position);
        }

        public void Execute()
        {
            
        }

        private GameObject GetNearestExecutionTarget()
        {
            var e = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            return e[0].gameObject;
        }
    }
}
