using System;

namespace StreetFighter2.Fighters
{
    // ============================================================
    // ABSTRACTION: Fighter is now abstract - defines contract
    // ============================================================
    public abstract class Fighter
    {
        // ============================================================
        // ENCAPSULATION: Private fields with controlled access
        // ============================================================
        private string name;
        private int maxHealth;
        private int currentHealth;
        private int attackPower;
        private int defensePower;
        private int specialMovePower;
        private int specialMoveCost;
        private int spBar;
        private int spBarMax;
        private int wins;
        private const int maxWins = 2;
        private string nationality;
        private string background;
        private string[] characterSelectLeft;
        private string[] characterSelectRight;

        // ============================================================
        // ENCAPSULATION: Public properties control access to private fields
        // ============================================================
        public string Name
        {
            get => name;
            protected set => name = value;
        }

        public int MaxHealth
        {
            get => maxHealth;
            protected set => maxHealth = value > 0 ? value : 100;
        }

        public int CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = Math.Clamp(value, 0, maxHealth);
        }

        public int AttackPower
        {
            get => attackPower;
            protected set => attackPower = value > 0 ? value : 10;
        }

        public int DefensePower
        {
            get => defensePower;
            protected set => defensePower = value > 0 ? value : 10;
        }

        public int SpecialMovePower
        {
            get => specialMovePower;
            protected set => specialMovePower = value > 0 ? value : 50;
        }

        public int SpecialMoveCost
        {
            get => specialMoveCost;
            protected set => specialMoveCost = value > 0 ? value : 50;
        }

        public int SpBar
        {
            get => spBar;
            set => spBar = Math.Clamp(value, 0, spBarMax);
        }

        public int SpBarMax
        {
            get => spBarMax;
            protected set => spBarMax = value > 0 ? value : 100;
        }

        public int Wins
        {
            get => wins;
            set => wins = Math.Clamp(value, 0, maxWins);
        }

        public string Nationality
        {
            get => nationality;
            protected set => nationality = value;
        }

        public string Background
        {
            get => background;
            protected set => background = value;
        }

        public string[] CharacterSelectLeft
        {
            get => characterSelectLeft;
            protected set => characterSelectLeft = value;
        }

        public string[] CharacterSelectRight
        {
            get => characterSelectRight;
            protected set => characterSelectRight = value;
        }

        // ============================================================
        // ABSTRACTION: Abstract methods - each fighter MUST implement
        // ============================================================
        public abstract string GetSpecialMoveName();
        public abstract string GetSpecialMoveDescription();
        public abstract int CalculateDamageMultiplier(Move attackMove, Move defenseMove);

        // ============================================================
        // POLYMORPHISM: Virtual methods - can be overridden by subclasses
        // ============================================================
        public virtual void OnAttackLanded(Fighter target, int damage)
        {
            SpBar = Math.Min(SpBar + 10, SpBarMax);
        }

        public virtual void OnDamageTaken(Fighter attacker, int damage)
        {
            SpBar = Math.Min(SpBar + 5, SpBarMax);
        }

        public virtual bool CanUseSpecialMove()
        {
            return SpBar >= SpecialMoveCost;
        }

        public virtual void ResetForNewRound()
        {
            CurrentHealth = MaxHealth;
            SpBar = 0;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Fighter Name: {Name}");
            Console.WriteLine($"Max Health: {MaxHealth}");
            Console.WriteLine($"Current Health: {CurrentHealth}");
            Console.WriteLine($"Attack Power: {AttackPower}");
            Console.WriteLine($"Defense Power: {DefensePower}");
            Console.WriteLine($"Special Move: {GetSpecialMoveName()}");
            Console.WriteLine($"Special Power: {SpecialMovePower}");
            Console.WriteLine($"SP Bar: {SpBar}/{SpBarMax}");
            Console.WriteLine($"Special Move Cost: {SpecialMoveCost}");
        }
    }
}
