using System;
using UnityEngine;

namespace GameLogic
{
    public class Execution : MonoBehaviour
    {
        public GameObject Target { get; private set; }
        public GameObject temp;
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
            return temp;
        }
    }
}
