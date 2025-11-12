using System.Collections.Generic;
using System.Linq;

namespace StreetFighter2.Combat
{
    public class TurnResult
    {
        // ============================================================
        // ENCAPSULATION: Private fields with public properties
        // ============================================================
        private List<string> logs;
        private bool specialFailed;
        private bool cancelled;
        private int damageToPlayer1;
        private int damageToPlayer2;

        public bool SpecialFailed
        {
            get => specialFailed;
            set => specialFailed = value;
        }

        public bool Cancelled
        {
            get => cancelled;
            set => cancelled = value;
        }

        public int DamageToPlayer1
        {
            get => damageToPlayer1;
            set => damageToPlayer1 = value >= 0 ? value : 0;
        }

        public int DamageToPlayer2
        {
            get => damageToPlayer2;
            set => damageToPlayer2 = value >= 0 ? value : 0;
        }

        public IReadOnlyList<string> Logs => logs.AsReadOnly();

        public TurnResult()
        {
            logs = new List<string>();
            specialFailed = false;
            cancelled = false;
            damageToPlayer1 = 0;
            damageToPlayer2 = 0;
        }

        // ============================================================
        // ENCAPSULATION: Controlled methods for adding logs
        // ============================================================
        public void AddLog(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logs.Add(message);
            }
        }

        public void AddLogs(params string[] messages)
        {
            foreach (var message in messages)
            {
                AddLog(message);
            }
        }

        public void ClearLogs()
        {
            logs.Clear();
        }

        public int GetTotalDamageDealt()
        {
            return damageToPlayer1 + damageToPlayer2;
        }

        public bool HasDamage()
        {
            return damageToPlayer1 > 0 || damageToPlayer2 > 0;
        }

        public string GetResultSummary()
        {
            if (cancelled)
                return "Round cancelled - forces neutralized!";
            if (!HasDamage())
                return "No damage dealt this round.";
            
            return $"P1 dealt {damageToPlayer2} damage | P2 dealt {damageToPlayer1} damage";
        }
    }
}