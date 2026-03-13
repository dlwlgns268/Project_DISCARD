using System.Linq;
using UnityEngine;

namespace GameLogic.Entity.Player
{
    public class Execution : MonoBehaviour
    {
        public Enemy[] Target { get; private set; }
        public LineRenderer executionLine;

        public void FixedUpdate()
        {
            Target = GetNearestExecutionTarget();
            executionLine.enabled = Target.Length > 0;
            if (Target.Length <= 0) return;
            var direction = Target[0].transform.position - transform.position;
            executionLine.positionCount = Target.Length + 1;
            executionLine.SetPosition(0, transform.position + direction.normalized * 0.5f);
            for (var i = 0; i < Target.Length; i++) executionLine.SetPosition(i + 1, Target[i].transform.position);
        }

        public void Execute()
        {
            
        }

        private Enemy[] GetNearestExecutionTarget()
        {
            var e = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            return e.Where(x => x.IsExecutable).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToArray();
        }
    }
}
