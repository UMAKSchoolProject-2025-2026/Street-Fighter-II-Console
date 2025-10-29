using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace StreetFighter2
{
    class ConsoleMenu
    {
        private int SelectedIndex;
        private string[] Options;
        private string Prompt;
        private string Header;

        public ConsoleMenu(string header, string prompt, string[] options)
        {
            Header = header;
            Prompt = prompt;
            Options = options;
            SelectedIndex = 0;
        }

        public ConsoleMenu(string[] options) 
        {
            Header = "";
            Prompt = "";
            Options = options;
            SelectedIndex = 0;  
        }


        private void DisplayOptions()
        {   
            WriteLine(Header);
            WriteLine(Prompt);

            

            for (int i = 0; i < Options.Length; i++)
            {
                string currentOption = Options[i];
                string prefix;

                if (i == SelectedIndex)
                {
                    prefix = ">";
                    ForegroundColor = ConsoleColor.Black;
                    BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    prefix = " ";
                    ForegroundColor = ConsoleColor.White;
                    BackgroundColor = ConsoleColor.Black;
                }

                WriteLine($" {prefix} << {currentOption} >> ");
            }

            ResetColor();
        }

        public int Run()
        {
            CursorVisible = false;
            

            ConsoleKey keyPressed;
            do
            {
                ForceClear();
                DisplayOptions();

                ConsoleKeyInfo keyInfo = ReadKey(true);
                keyPressed = keyInfo.Key;

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    SelectedIndex--;
                    if (SelectedIndex == -1)
                    {
                        SelectedIndex = Options.Length - 1;
                    }

                }
                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    SelectedIndex++;
                    if (SelectedIndex == Options.Length)
                    {
                        SelectedIndex = 0;
                    }
                }

            }
            while (keyPressed != ConsoleKey.Enter);

            return SelectedIndex;
        }

        private static void ForceClear()
        {
            try
            {
                
                int width = Math.Min(Console.LargestWindowWidth, 120);
                int height = Math.Min(Console.LargestWindowHeight, 40);

                Console.SetWindowSize(width, height);
                Console.SetBufferSize(width, height);

                
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < height; i++)
                {
                    Console.WriteLine(new string(' ', width));
                }

                Console.SetCursorPosition(0, 0);
            }
            catch { }
        }

        
    }
}
