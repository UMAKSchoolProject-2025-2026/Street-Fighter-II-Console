using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StreetFighter2.Combat; 

namespace StreetFighter2.Fighters
{   

    public class Fighter
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

       
        private string nationality;
        private string background;

        private string[] characterSelectLeft;
        private string[] characterSelectRight;

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
            
            
            this.nationality = "";
            this.background = "";
            
            
            this.characterSelectLeft = new string[0];
            this.characterSelectRight = new string[0];
        }

        public Fighter()
        {
            name = "Unknown";
            maxHealth = 200;
            currentHealth = 200;
            attackPower = 0;
            defensePower = 0;
            specialMovePower = 0;
            specialMoveCost = 50;
            spBar = 100;
            spBarMax = 100;
            wins = 0;
            
           
            nationality = "";
            background = "";
            
            
            characterSelectLeft = new string[0];
            characterSelectRight = new string[0];
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

        public string Nationality
        {
            get { return nationality; }
            set { nationality = value; }
        }

        public string Background
        {
            get { return background; }
            set { background = value; }
        }

        public string[] CharacterSelectLeft
        {
            get { return characterSelectLeft; }
            set { characterSelectLeft = value; }
        }

        public string[] CharacterSelectRight
        {
            get { return characterSelectRight; }
            set { characterSelectRight = value; }
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
