using System;
using StreetFighter2.Fighters;

namespace StreetFighter2.Combat
{
    // ============================================================
    // ABSTRACTION: Define interface for AI difficulty strategies
    // ============================================================
    public interface IAIStrategy
    {
        Move DecideMove(Fighter aiFighter, Fighter opponent);
    }

    // ============================================================
    // POLYMORPHISM: Different AI difficulty implementations
    // ============================================================
    public class RandomAIStrategy : IAIStrategy
    {
        private Random random = new Random();

        public Move DecideMove(Fighter aiFighter, Fighter opponent)
        {
            return (Move)random.Next(0, 5);
        }
    }

    public class SmartAIStrategy : IAIStrategy
    {
        private Random random = new Random();

        public Move DecideMove(Fighter aiFighter, Fighter opponent)
        {
            if (aiFighter.CanUseSpecialMove() && opponent.CurrentHealth < 50)
            {
                return Move.Special;
            }

            if (opponent.CurrentHealth < aiFighter.CurrentHealth)
            {
                return random.Next(0, 2) == 0 ? Move.LightAttack : Move.HeavyAttack;
            }

            if (aiFighter.CurrentHealth < opponent.CurrentHealth * 0.3)
            {
                return random.Next(0, 2) == 0 ? Move.Block : Move.Dodge;
            }

            return (Move)random.Next(0, 5);
        }
    }

    public class AIOpponent
    {
        // ============================================================
        // ENCAPSULATION: Private fields
        // ============================================================
        private Random random;
        private IAIStrategy strategy;
        private readonly string[] availableCharacters = 
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

        public AIOpponent(IAIStrategy aiStrategy = null)
        {
            random = new Random();
            strategy = aiStrategy ?? new RandomAIStrategy();
        }

        // ============================================================
        // ENCAPSULATION: Public method to change strategy at runtime
        // ============================================================
        public void SetStrategy(IAIStrategy newStrategy)
        {
            strategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy));
        }

        // ============================================================
        // POLYMORPHISM: Uses strategy pattern for decision making
        // ============================================================
        public Move GetMove(Fighter aiFighter = null, Fighter opponent = null)
        {
            return strategy.DecideMove(aiFighter, opponent);
        }

        public Fighter GetRandomCharacter()
        {
            int index = random.Next(0, availableCharacters.Length);
            string characterName = availableCharacters[index];
            return FighterFactory.CreateFighter(characterName);
        }
    }

    // ============================================================
    // ABSTRACTION: Factory Pattern for creating fighters
    // ============================================================
    public static class FighterFactory
    {
        public static Fighter CreateFighter(string characterName)
        {
            return characterName switch
            {
                "Ryu" => new Ryu(),
                "Ken" => new Ken(),
                "Chun-Li" => new ChunLi(),
                "Guile" => new Guile(),
                "Blanka" => new Blanka(),
                "E. Honda" => new eHonda(),
                "Dhalsim" => new Dhalsim(),
                "Zangief" => new Zangief(),
                _ => new Ryu()
            };
        }

        public static string[] GetAllCharacterNames()
        {
            return new[] 
            { 
                "Ryu", "Ken", "Chun-Li", "Guile", 
                "Blanka", "E. Honda", "Dhalsim", "Zangief" 
            };
        }
    }
}