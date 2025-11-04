using System;
using StreetFighter2.Fighters;

namespace StreetFighter2.Combat
{
    public class AIOpponent
    {
        private Random random;
        private string[] availableCharacters = 
        { 
            "Ryu", 
            "Ken", 
            "Chun-Li", 
            "Guile", 
            "Blanka", 
            "E. Honda", 
            "Dhalsim", 
            "Zangief" 
        };

        public AIOpponent()
        {
            random = new Random();
        }

        public Move GetMove()
        {
            int choice = random.Next(0, 5);
            return (Move)choice;
        }

        public Fighter GetRandomCharacter()
        {
            int index = random.Next(0, availableCharacters.Length);
            string characterName = availableCharacters[index];
            return CreateFighterInstance(characterName);
        }

        private Fighter CreateFighterInstance(string characterName)
        {
            switch (characterName)
            {
                case "Ryu":
                    return new Ryu();
                case "Ken":
                    return new Ken();
                case "Chun-Li":
                    return new ChunLi();
                case "Guile":
                    return new Guile();
                case "Blanka":
                    return new Blanka();
                case "E. Honda":
                    return new eHonda();
                case "Dhalsim":
                    return new Dhalsim();
                case "Zangief":
                    return new Zangief();
                default:
                    return new Ryu();
            }
        }
    }
}