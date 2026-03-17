using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

#if UNITY_EDITOR // 테스트용 코드
        public void OnAttack(InputValue value)
        {
            if (value.isPressed) Execute();
        }
#endif
        
        public void Execute()
        {
            switch (Target.Length)
            {
                case 0:
                    return;
                case 1:
                    var o = Target[0];
                    var dir = o.transform.position - transform.position;
                    transform.position = o.transform.position + dir.normalized;
                    Destroy(o.gameObject); // 나중에 비활성화 코드로 바꿀 것
                    break;
                default:
                    var d = Target[^1].transform.position - Target[^2].transform.position;
                    transform.position = Target[^1].transform.position + d.normalized;
                    foreach (var i in Target) Destroy(i.gameObject); // 나중에 비활성화 코드로 바꿀 것
                    break;
            }
        }

        private Enemy[] GetNearestExecutionTarget()
        {
            var e = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            return e.Where(x => x.IsExecutable).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToArray();
        }
    }
}
