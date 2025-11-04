using System.Collections.Generic;

namespace StreetFighter2.Combat
{
    public class TurnResult
    {
        private List<string> logs;

        public bool SpecialFailed { get; set; }
        public bool Cancelled { get; set; }
        public int DamageToPlayer1 { get; set; }
        public int DamageToPlayer2 { get; set; }
        public List<string> Logs => logs;

        public TurnResult()
        {
            logs = new List<string>();
            SpecialFailed = false;
            Cancelled = false;
            DamageToPlayer1 = 0;
            DamageToPlayer2 = 0;
        }

        public void AddLog(string message)
        {
            logs.Add(message);
        }
    }
}