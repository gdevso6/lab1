using System;
using UnityEngine;
using Random = UnityEngine.Random;



public class DamageLogView : MonoBehaviour {
    [SerializeField, Min(1)] private int baseDamage = 20;
    [SerializeField, Range(0f, 1f)] private float critChance = 0.25f;

    private DamageLog log;

    public int TotalDamage => log == null ? 0 : log.TotalDamage();

    private void Awake() {
        log = new DamageLog();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            int damage = Random.Range(Mathf.Max(1, baseDamage / 2), baseDamage + baseDamage / 2 + 1);
            bool isCritical = Random.value < critChance;

            try {
                DamageEntry entry = log.Register(damage, isCritical);
                Debug.Log($"{(isCritical ? "КРИТ! " : "")}{entry}");
                Debug.Log($"ударов: {log.Count}, суммарный урон: {TotalDamage}, самый сильный: {log.Strongest()}");
            }
            catch (ArgumentException e) {
                Debug.Log($"удар не записан: {e.Message}");
            }
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            try {
                log.Register(0);
            }
            catch (ArgumentException e) {
                Debug.Log($"некорректный удар отклонён: {e.Message}. суммарный урон: {TotalDamage}");
            }
        }
    }
}