using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLogic.Entity.Player
{
    public class Execution : MonoBehaviour
    {
        public Enemy.Enemy[] Target { get; private set; }
        public LineRenderer executionLine;
        public ParticleSystem executionEffect;
        private bool IsExecuting { get; set; }

        public void FixedUpdate()
        {
            if (Player.Instance.playerInputManager.AttackRequest)
            {
                Player.Instance.playerInputManager.AttackRequest = false;
                Execute();
            }
            if (IsExecuting) return;
            Target = GetNearestExecutionTarget();
            executionLine.enabled = Target.Length > 0;
            if (Target.Length <= 0) return;
            var localPos = Player.Instance.spriteRenderer.bounds.center;
            var direction = Target[0].transform.position - localPos;
            executionLine.positionCount = Target.Length + 1;
            executionLine.SetPosition(0, localPos + direction.normalized * Mathf.Lerp(0f, 0.25f, Time.time * 5 % 1));
            for (var i = 0; i < Target.Length; i++) executionLine.SetPosition(i + 1, Target[i].transform.position);
        }
        
        public void Execute()
        {
            StartCoroutine(ExecuteFlow());
        }

        private IEnumerator ExecuteFlow()
        {
            if (Target.Length <= 0) yield break;
            Player.Instance.playerDash.IsInvincible = true;
            var gravity = Player.Instance.rb.gravityScale;
            Player.Instance.rb.gravityScale = 0;
            Player.Instance.rb.linearVelocity = Vector2.zero;
            Player.Instance.selfCollider.enabled = false;
            IsExecuting = true;
            foreach (var o in Target)
            {
                var localPos = Player.Instance.spriteRenderer.bounds.center;
                var p = Instantiate(executionEffect, localPos, Quaternion.identity);
                p.transform.LookAt(o.transform.position);
                var main = p.main;
                main.startSpeed = (o.transform.position - localPos).magnitude * 6.7f;
                p.Play();
                transform.position = o.transform.position;
                o.Execute();
                yield return new WaitForSeconds(0.1f);
            }
            IsExecuting = false;
            Player.Instance.rb.gravityScale = gravity;
            Player.Instance.selfCollider.enabled = true;
            Player.Instance.playerDash.IsInvincible = false;
        }

        private Enemy.Enemy[] GetNearestExecutionTarget()
        {
            var e = FindObjectsByType<Enemy.Enemy>(FindObjectsSortMode.None);
            return e.Where(x => x.IsExecutable).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToArray();
        }
    }
}
