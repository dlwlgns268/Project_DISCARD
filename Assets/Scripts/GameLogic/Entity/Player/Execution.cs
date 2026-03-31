using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLogic.Entity.Player
{
    public class Execution : MonoBehaviour
    {
        public Enemy[] Target { get; private set; }
        public LineRenderer executionLine;
        public ParticleSystem executionEffect;
        private bool IsExecuting { get; set; }

        public void FixedUpdate()
        {
            if (IsExecuting) return;
            Target = GetNearestExecutionTarget();
            executionLine.enabled = Target.Length > 0;
            if (Target.Length <= 0) return;
            var direction = Target[0].transform.position - transform.position;
            executionLine.positionCount = Target.Length + 1;
            executionLine.SetPosition(0, transform.position + direction.normalized * Mathf.Lerp(0f, 0.25f, Time.time * 5 % 1));
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
            StartCoroutine(ExecuteFlow());
        }

        private IEnumerator ExecuteFlow()
        {
            if (Target.Length <= 0) yield break;
            // 가능하면 무적 코드 추가할 것
            IsExecuting = true;
            foreach (var o in Target)
            {
                var p = Instantiate(executionEffect, transform.position, Quaternion.identity);
                p.transform.LookAt(o.transform.position);
                var main = p.main;
                main.startSpeed = (o.transform.position - transform.position).magnitude * 6.7f;
                p.Play();
                transform.position = o.transform.position;
                o.Execute();
                yield return new WaitForSeconds(0.1f);
            }
            IsExecuting = false;
            // 무적 해제
        }

        private Enemy[] GetNearestExecutionTarget()
        {
            var e = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            return e.Where(x => x.IsExecutable).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToArray();
        }
    }
}
