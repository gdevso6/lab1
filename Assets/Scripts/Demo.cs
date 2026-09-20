using System;
using UnityEngine;

public class Demo : MonoBehaviour {
    private void Start() {
        DamageEntry.ResetCounter();

        Debug.Log("=== 1. корректные операции ===");

        DamageEntry e1 = new DamageEntry("Игрок", "A", 25, DamageType.Physical, DateTime.Now);
        DamageEntry e2 = new DamageEntry("Игрок", "A", 40, DamageType.Distance, DateTime.Now);
        DamageEntry e3 = new DamageEntry("Маг", "Игрок", 15, DamageType.Distance, DateTime.Now);
        DamageEntry e4 = new DamageEntry("Яд", "Голем", 5, DamageType.Poison);
        Debug.Log($"счётчик записей: {DamageEntry.EntriesInBattle}");

        DamageLog log = new DamageLog();
        log.Add(e1);
        log.Add(e2);
        log.Add(e3);
        log.Add(e4);

        Debug.Log("перегрузка Register");
        Debug.Log($"Register(30)       -> {log.Register(30)}");
        Debug.Log($"Register(30, true) -> {log.Register(30, true)} (CRIT_MULTIPLIER = {DamageEntry.CRIT_MULTIPLIER})");

        Debug.Log($"суммарный урон: {log.TotalDamage()}");
        Debug.Log($"самый сильный удар: {log.Strongest()}");
        Debug.Log($"урон по цели «A»: {log.DamageToTarget("A")}");
        Debug.Log($"урон по цели «Дракон» (такой нет): {log.DamageToTarget("Дракон")}");
        Debug.Log($"самый сильный удар в пустом логе: {(new DamageLog().Strongest() == null ? "null" : "?")}");
        Debug.Log("последние 3 записи:");
        foreach (DamageEntry entry in log.Last(3))
            Debug.Log("  " + entry);
        Debug.Log($"состояние: {log} | счётчик записей: {DamageEntry.EntriesInBattle}");









        Debug.Log("=== 2. некорректные попытки (состояние должно остаться прежним) ===");

        Func<string> state = () => $"{log} | счётчик записей: {DamageEntry.EntriesInBattle}";

        Attempt("запись с уроном 0",
            () => new DamageEntry("Игрок", "A", 0, DamageType.Physical, DateTime.Now), state);

        Attempt("запись с отрицательным уроном (-10)",
            () => new DamageEntry("Игрок", "A", -10, DamageType.Physical, DateTime.Now), state);

        Attempt("log.Register(0)",
            () => log.Register(0), state);

        Attempt("log.Register(-5, true)",
            () => log.Register(-5, true), state);

        Attempt("неизвестный тип урона (DamageType)999",
            () => new DamageEntry("Игрок", "A", 10, (DamageType)999, DateTime.Now), state);

        Attempt("log.Add(null)",
            () => log.Add(null), state);

        Attempt("log.Last(0)",
            () => log.Last(0), state);

        Debug.Log("e1.Damage = 1; не компилируется, у свойства Damage нет set");
        Debug.Log($"    состояние: {e1}");

        DamageEntry[] stolen = log.GetEntries();   // копия
        stolen[0] = null;
        Array.Resize(ref stolen, stolen.Length + 1);
        stolen[stolen.Length - 1] = e1;
        Debug.Log($"изменена только копия (длина копии {stolen.Length})");
        Debug.Log($"    состояние: {log} | первая запись: {log.GetEntries()[0]}");
    }

    private static void Attempt(string title, Action action, Func<string> state) 
        {
        try {
            action();
            Debug.LogError($"{title}: класс пропустил некорректное действие");
        }
        catch (ArgumentException e)
        {
            Debug.Log($"{title}: {e.Message}");
        }
        Debug.Log($"    состояние: {state()}");
    }
}




// DamageEntry.damage    - строго больше нуля                   - конструктор DamageEntry (полный; упрощённый вызывает его);
//                                                                Register(int, bool) в DamageLog проверяет то же правило до пересчёта крита
// DamageEntry.type      - только значения из enum DamageType   - конструктор DamageEntry (Enum.IsDefined)
// DamageEntry.source    - не null и не пустая строка           - конструктор DamageEntry
// DamageEntry.target    - не null и не пустая строка           - конструктор DamageEntry
// DamageEntry (все поля) - неизменяемы после создания          - private readonly поля + свойства без set (защита на этапе компиляции)
// DamageEntry.entriesInBattle - растёт только от успешных созданий - конструктор DamageEntry (инкремент после всех проверок)
// DamageLog.entries     - внутренний массив недоступен снаружи - GetEntries() и Last(int) возвращают копии
// DamageLog.entries     - в лог нельзя добавить null            - Add(DamageEntry)
// DamageLog.defaultSource / defaultTarget - не пустые          - конструктор DamageLog
// DamageLog.Last(n)     - n строго больше нуля                 - Last(int)
