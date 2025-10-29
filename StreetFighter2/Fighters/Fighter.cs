using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetFighter2.Fighters
{   

    enum Move
    {
        LightAttack,
        HeavyAttack,
        Block,
        Dodge,
        Throw,
        Special
    }

    class Fighter
    {
        //Fields
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
        const int maxWins = 2;

        //Constructor
        public Fighter(string name, int maxHealth, int currentHealth, int attackPower, int defensePower, int specialMovePower, int specialMoveCost, int spBar, int spBarMax, int wins)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.currentHealth = currentHealth;
            this.attackPower = attackPower;
            this.defensePower = defensePower;
            this.specialMovePower = specialMovePower;
            this.specialMoveCost = specialMoveCost;
            this.spBar = spBar;
            this.spBarMax = spBarMax;
            this.wins = wins;
        }

        public Fighter()
        {
            name = "Unkown";
            maxHealth = 200;
            currentHealth = 200;
            attackPower = 0;
            defensePower = 0;
            specialMovePower = 0;
            specialMoveCost = 50;
            spBar = 100;
            spBarMax = 100;
            wins = 0;
        }

        //Methods
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }
        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }
        public int AttackPower
        {
            get { return attackPower; }
            set { attackPower = value; }
        }

        public int DefensePower
        {
            get { return defensePower; }
            set { defensePower = value; }
        }

        public int SpecialMovePower
        {
            get { return specialMovePower; }
            set { specialMovePower = value; }
        }

        public int SpecialMoveCost
        {
            get { return specialMoveCost; }
            set { specialMoveCost = value; }
        }

        public int SpBar
        {
            get { return spBar; }
            set { spBar = value; }
        }

        public int SpBarMax
        {
            get { return spBarMax; }
            set { spBarMax = value; }
        }

        public int Wins
        {
            get { return wins; }
            set { wins = value; }
        }

        

        //Display Fighter Info
        public void DisplayInfo()
        {
            Console.WriteLine("Fighter Name: " + name);
            Console.WriteLine("Max Health: " + maxHealth);
            Console.WriteLine("Current Health: " + currentHealth);
            Console.WriteLine("Attack Power: " + attackPower);
            Console.WriteLine("Defense Power: " + defensePower);
            Console.WriteLine("Special Move Power: " + specialMovePower);
            Console.WriteLine("SP Bar: " + spBar + "/" + spBarMax);
            Console.WriteLine("Special Move Cost: " + specialMoveCost);
        }
    }
}
