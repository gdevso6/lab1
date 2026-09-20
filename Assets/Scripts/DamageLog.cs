using System;


public class DamageLog {
    private const int InitialCapacity = 8;

    private DamageEntry[] entries = new DamageEntry[InitialCapacity];
    private int count;
    private readonly string defaultSource;
    private readonly string defaultTarget;
    public int Count => count;
    public DamageLog() : this("Player", "Target") {
    }


    public DamageLog(string defaultSource, string defaultTarget) {
        if (string.IsNullOrWhiteSpace(defaultSource))
            throw new ArgumentException("не указан источник по умолчанию");
        if (string.IsNullOrWhiteSpace(defaultTarget))
            throw new ArgumentException("не указана цель по умолчанию");

        this.defaultSource = defaultSource;
        this.defaultTarget = defaultTarget;
    }

    public DamageEntry Register(int damage) {
        return Register(damage, false);
    }

    public DamageEntry Register(int damage, bool isCritical) {
        if (damage <= 0)
            throw new ArgumentException("урон <= 0");

        int finalDamage = isCritical ? DamageEntry.ApplyCrit(damage) : damage;
        DamageEntry entry = new DamageEntry(defaultSource, defaultTarget, finalDamage,
                                            DamageType.Physical, DateTime.Now);
        Add(entry);
        return entry;
    }


    public void Add(DamageEntry entry) {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry), "Запись не может быть null");

        if (count == entries.Length)
            Array.Resize(ref entries, entries.Length * 2);

        entries[count] = entry;
        count++;
    }

    public int TotalDamage() {
        int sum = 0;
        for (int i = 0; i < count; i++)
            sum += entries[i].Damage;
        return sum;
    }

    public DamageEntry Strongest() {
        DamageEntry best = null;
        for (int i = 0; i < count; i++) {
            if (best == null || entries[i].Damage > best.Damage)
                best = entries[i];
        }
        return best;
    }

    public int DamageToTarget(string target) {
        int sum = 0;
        for (int i = 0; i < count; i++) {
            if (entries[i].Target == target)
                sum += entries[i].Damage;
        }
        return sum;
    }
    public DamageEntry[] Last(int n) {
        if (n <= 0)
            throw new ArgumentException("N <= 0");

        int take = Math.Min(n, count);
        DamageEntry[] result = new DamageEntry[take];
        Array.Copy(entries, count - take, result, 0, take);
        return result;
    }

    public DamageEntry[] GetEntries() {
        DamageEntry[] copy = new DamageEntry[count];
        Array.Copy(entries, copy, count);
        return copy;
    }

    public override string ToString() {
        return $"DamageLog: записей {count}, суммарный урон {TotalDamage()}";
    }
}