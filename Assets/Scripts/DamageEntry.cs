using System;


public enum DamageType { 
    Physical,
    Distance,
    Poison
}

public class DamageEntry
{
    private readonly string source;
    private readonly string target;
    private readonly int damage;
    private readonly DamageType damageType;
    private readonly DateTime timestamp;



    public const float CRIT_MULTIPLIER = 2.0f;


    // =>  -  readonly private
    public string Source => source;
    public string Target => target;
    public int Damage => damage;
    public DamageType Type  => damageType;
    public DateTime TimeStamp => timestamp;

    private static int entriesInBattle = 0;
    public static int EntriesInBattle => entriesInBattle;

    public static void ResetCounter() {
        entriesInBattle = 0;
    }


    public DamageEntry(string source, string target, int damage, DamageType damageType, DateTime timestamp) {
        if (damage <= 0) {
            throw new ArgumentException("урон должен быть больше нуля", nameof(damage));
        }
        if (!Enum.IsDefined(typeof(DamageType), damageType)) {
            throw new ArgumentException("не разрешенный тип урона", nameof(damageType));
        }
        if (string.IsNullOrWhiteSpace(source)) {
            throw new ArgumentException("не указан источник урона");
        }
        if (string.IsNullOrWhiteSpace(target)) {
            throw new ArgumentException("не указана цель урона");
        }

        this.source = source;
        this.target = target;
        this.damage = damage;
        this.damageType = damageType;
        this.timestamp = timestamp;

        entriesInBattle++;
    }

    public DamageEntry(
        string source,
        string target,
        int damage,
        DamageType damageType)
        : this(source, target, damage, damageType, DateTime.Now) {
    }

    public static int ApplyCrit(int damage) {
        return (int)Math.Round(damage * CRIT_MULTIPLIER);
    }


    public override string ToString() {
        return $"{source} -> {target} | " +
               $"урон: {damage} | " +
               $"тип: {damageType} | " +
               $"время: {timestamp:HH:mm:ss}";
    }
}