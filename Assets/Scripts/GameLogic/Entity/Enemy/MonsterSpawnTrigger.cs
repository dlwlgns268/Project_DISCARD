using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterSpawnEntry
{
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector3 localOffset;

    public GameObject MonsterPrefab => monsterPrefab;

    public Vector3 GetWarningPosition(Transform triggerTransform)
    {
        return GetSpawnPosition(triggerTransform);
    }

    public Vector3 GetSpawnPosition(Transform triggerTransform)
    {
        if (spawnPoint != null)
        {
            return spawnPoint.position;
        }

        return triggerTransform.TransformPoint(localOffset);
    }
}

[RequireComponent(typeof(Collider2D))]
public class MonsterSpawnTrigger : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private string triggerTag = "Player";
    [SerializeField] private bool spawnOnlyOnce = true;
    [SerializeField] private bool disableTriggerAfterSpawn = true;

    [Header("Spawn")]
    [SerializeField] private List<MonsterSpawnEntry> spawnEntries = new List<MonsterSpawnEntry>();
    [SerializeField] private Transform spawnParent;

    [Header("Warning")]
    [SerializeField] private GameObject warningIndicatorPrefab;
    [SerializeField] private float warningDuration = 0.6f;
    [SerializeField] private Transform warningParent;

    [Header("Debug")]
    [SerializeField] private bool drawGizmosAlways = true;

    private bool _hasSpawned;
    private Collider2D _triggerCollider;

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider2D>();
        if (_triggerCollider != null && !_triggerCollider.isTrigger)
        {
            Debug.LogWarning($"{name}: MonsterSpawnTrigger는 Trigger Collider2D 사용을 권장합니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (spawnOnlyOnce && _hasSpawned) return;
        if (!other.CompareTag(triggerTag)) return;

        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        bool hasValidEntry = false;

        foreach (var entry in spawnEntries)
        {
            if (entry == null || entry.MonsterPrefab == null) continue;
            hasValidEntry = true;
            StartCoroutine(SpawnEntryWithWarning(entry));
        }

        if (!hasValidEntry) return;

        _hasSpawned = true;

        if (disableTriggerAfterSpawn && _triggerCollider != null)
        {
            _triggerCollider.enabled = false;
        }
    }

    private System.Collections.IEnumerator SpawnEntryWithWarning(MonsterSpawnEntry entry)
    {
        GameObject warningObj = null;
        Vector3 warningPosition = entry.GetWarningPosition(transform);

        if (warningIndicatorPrefab != null)
        {
            warningObj = Instantiate(warningIndicatorPrefab, warningPosition, Quaternion.identity, warningParent);
        }

        if (warningDuration > 0f)
        {
            yield return new WaitForSeconds(warningDuration);
        }

        if (warningObj != null)
        {
            Destroy(warningObj);
        }

        Vector3 spawnPosition = entry.GetSpawnPosition(transform);
        Instantiate(entry.MonsterPrefab, spawnPosition, Quaternion.identity, spawnParent);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!drawGizmosAlways) return;
        DrawSpawnGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        DrawSpawnGizmos();
    }

    private void DrawSpawnGizmos()
    {
        foreach (var entry in spawnEntries)
        {
            if (entry == null || entry.MonsterPrefab == null) continue;

            Vector3 warningPos = entry.GetWarningPosition(transform);
            Vector3 spawnPos = entry.GetSpawnPosition(transform);

            Gizmos.color = new Color(1f, 0.85f, 0.2f, 1f);
            Gizmos.DrawWireSphere(warningPos, 0.18f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPos, 0.22f);
            Gizmos.DrawLine(warningPos, spawnPos);
        }
    }
#endif
}