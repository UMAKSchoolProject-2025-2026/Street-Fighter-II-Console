using System;
using static System.Console;

namespace StreetFighter2
{
    class Program
    {
        static void Main(string[] args)
        {
            OutputEncoding = System.Text.Encoding.UTF8;
            Clear();

            GameManager game = new GameManager();

            game.checkpoint();
            game.StartGame();

            WriteLine("\n\nPress any key...");
            ReadKey(true);
        }
    }
}