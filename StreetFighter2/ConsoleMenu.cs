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

        
        public const string HIGHLIGHT_MARKER = "###HIGHLIGHT###";

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

       
        public static int RunMultiPanelMenu(string[][] panelOptions, PanelConstructor panel, 
            string[] panelHeaders = null, bool showHeaders = true, bool showActiveIndicator = true)
        {
            if (panelOptions == null || panelOptions.Length != 3)
            {
                throw new ArgumentException("Must provide exactly 3 arrays of options for left, center, and right panels.");
            }

            int selectedPanel = 1; 
            int selectedIndex = 0;
            ConsoleKey keyPressed;

            CursorVisible = false;

            do
            {
                Clear();
                SetCursorPosition(0, 0);

                
                var leftContent = BuildPanelContent(panelOptions[0], selectedPanel == 0, selectedIndex, 
                    panelHeaders != null ? panelHeaders[0] : "", showHeaders, showActiveIndicator);
                var centerContent = BuildPanelContent(panelOptions[1], selectedPanel == 1, selectedIndex, 
                    panelHeaders != null ? panelHeaders[1] : "", showHeaders, showActiveIndicator);
                var rightContent = BuildPanelContent(panelOptions[2], selectedPanel == 2, selectedIndex, 
                    panelHeaders != null ? panelHeaders[2] : "", showHeaders, showActiveIndicator);

                
                panel.SetLeftContent(leftContent.ToArray());
                panel.SetCenterContent(centerContent.ToArray());
                panel.SetRightContent(rightContent.ToArray());

               
                panel.RenderWithColors();

              
                ConsoleKeyInfo keyInfo = ReadKey(true);
                keyPressed = keyInfo.Key;

                string[] currentPanelOptions = panelOptions[selectedPanel];

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if (selectedIndex < 0)
                    {
                        selectedIndex = currentPanelOptions.Length - 1;
                    }
                }
                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex >= currentPanelOptions.Length)
                    {
                        selectedIndex = 0;
                    }
                }
                else if (keyPressed == ConsoleKey.LeftArrow)
                {
                    selectedPanel--;
                    if (selectedPanel < 0)
                    {
                        selectedPanel = 2;
                    }
                    selectedIndex = Math.Min(selectedIndex, panelOptions[selectedPanel].Length - 1);
                }
                else if (keyPressed == ConsoleKey.RightArrow)
                {
                    selectedPanel++;
                    if (selectedPanel > 2)
                    {
                        selectedPanel = 0;
                    }
                    selectedIndex = Math.Min(selectedIndex, panelOptions[selectedPanel].Length - 1);
                }

            } while (keyPressed != ConsoleKey.Enter);

            CursorVisible = true;

            
            return (selectedPanel * 100) + selectedIndex;
        }

       
        private static List<string> BuildPanelContent(string[] options, bool isActive, int selectedIndex, 
            string header, bool showHeader = true, bool showActiveIndicator = true)
        {
            List<string> content = new List<string>();

            
            if (showHeader && !string.IsNullOrEmpty(header))
            {
                content.Add(header);
                content.Add("");
            }

           
            if (showActiveIndicator)
            {
                if (isActive)
                {
                    content.Add(">>> ACTIVE <<<");
                }
                else
                {
                    content.Add("");
                }

                content.Add("");
            }

            
            for (int i = 0; i < options.Length; i++)
            {
                if (isActive && i == selectedIndex)
                {
                    
                    content.Add($"{HIGHLIGHT_MARKER}>>> {options[i]} <<<");
                }
                else
                {
                    content.Add($"    {options[i]}");
                }
            }

            return content;
        }

        
        public static (int panel, int index) DecodeMultiPanelResult(int result)
        {
            int panel = result / 100;
            int index = result % 100;
            return (panel, index);
        }

        public static void ForceClear()
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
