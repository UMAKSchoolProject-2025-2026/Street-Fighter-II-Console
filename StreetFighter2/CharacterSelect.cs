using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using StreetFighter2.Fighters;
using StreetFighter2.Combat;

namespace StreetFighter2
{
    class CharacterSelect
    {
        private string[] leftCharacters;
        private string[] centerCharacters;
        private const int INPUT_COOLDOWN_MS = 150; 
        private AudioManager audioManager; 

        public CharacterSelect(AudioManager audioManager = null)
        {
            this.audioManager = audioManager; 
            
            
            leftCharacters = new string[] 
            { 
                "Ryu", 
                "Ken", 
                "Chun-Li",
                "Guile"
            };
            
            centerCharacters = new string[] 
            { 
                "Blanka", 
                "E. Honda",
                "Dhalsim",
                "Zangief" 
            };
        }

        public void Show()
        {
            ConsoleKey keyPressed;
            int selectedPanel = 0; 
            int selectedIndex = 0;

            CursorVisible = false;

            
            try
            {
                SetBufferSize(WindowWidth, WindowHeight);
            }
            catch { }

            
            RenderScreen(selectedPanel, selectedIndex);

            do
            {
                
                while (KeyAvailable)
                {
                    ReadKey(true); 
                }

                ConsoleKeyInfo keyInfo = ReadKey(true);
                keyPressed = keyInfo.Key;

                string[] currentList = selectedPanel == 0 ? leftCharacters : centerCharacters;
                bool stateChanged = false;

                
                switch (keyPressed)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex--;
                        if (selectedIndex < 0)
                        {
                            selectedIndex = currentList.Length - 1;
                        }
                        stateChanged = true;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex++;
                        if (selectedIndex >= currentList.Length)
                        {
                            selectedIndex = 0;
                        }
                        stateChanged = true;
                        break;

                    case ConsoleKey.LeftArrow:
                        if (selectedPanel != 0)
                        {
                            selectedPanel = 0;
                            selectedIndex = Math.Min(selectedIndex, leftCharacters.Length - 1);
                            stateChanged = true;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (selectedPanel != 1)
                        {
                            selectedPanel = 1;
                            selectedIndex = Math.Min(selectedIndex, centerCharacters.Length - 1);
                            stateChanged = true;
                        }
                        break;

                    case ConsoleKey.Enter:
                        break;

                    default:
                        break;
                }

                
                if (stateChanged)
                {
                    RenderScreen(selectedPanel, selectedIndex);
                    
                    
                    Thread.Sleep(INPUT_COOLDOWN_MS);
                    
                    
                    while (KeyAvailable)
                    {
                        ReadKey(true);
                    }
                }

            } while (keyPressed != ConsoleKey.Enter);

            CursorVisible = true;

            
            string finalCharacter = selectedPanel == 0 
                ? leftCharacters[selectedIndex] 
                : centerCharacters[selectedIndex];

            
           
            HandleCharacterSelection(finalCharacter);
           
        }

        private void RenderScreen(int selectedPanel, int selectedIndex)
        {
            
            SetCursorPosition(0, 0);
            string emptyLine = new string(' ', WindowWidth);
            
            for (int i = 0; i < WindowHeight; i++) 
            {
                SetCursorPosition(0, i); 
                Write(emptyLine);      
            }
            
            
            SetCursorPosition(0, 0);
            
            string selectedCharacter = selectedPanel == 0 
                ? leftCharacters[selectedIndex] 
                : centerCharacters[selectedIndex];

            var (leftContent, centerContent, rightContent) = GetCharacterDisplayData(selectedCharacter);

            
            PanelConstructor topPanel = new PanelConstructor(80, 80, 50, showBorders: false);
            
            topPanel.LeftAlignH = HorizontalAlign.Center;
            topPanel.CenterAlignH = HorizontalAlign.Center;
            topPanel.RightAlignH = HorizontalAlign.Left;
            
            topPanel.LeftAlignV = VerticalAlign.Middle;
            topPanel.CenterAlignV = VerticalAlign.Middle;
            topPanel.RightAlignV = VerticalAlign.Top;
            
            
            topPanel.AutoWrapRight = true;
            
            topPanel.SetLeftContent(leftContent);
            topPanel.SetCenterContent(centerContent);
            topPanel.SetRightContent(rightContent);

            topPanel.RenderWithLeftPanelInverted();

            WriteLine();
            WriteLine();

           
            int bottomRightWidth = WindowWidth - 42;
            PanelConstructor bottomPanel = new PanelConstructor(20, bottomRightWidth, showBorders: false);
            
            bottomPanel.LeftAlignH = HorizontalAlign.Left;
            bottomPanel.CenterAlignH = HorizontalAlign.Left;
            bottomPanel.RightAlignH = HorizontalAlign.Left;

            
            var leftList = PanelConstructor.BuildHighlightedList(
                leftCharacters, 
                selectedIndex, 
                ConsoleMenu.HIGHLIGHT_MARKER, 
                selectedPanel == 0
            );
            
            var centerList = PanelConstructor.BuildHighlightedList(
                centerCharacters, 
                selectedIndex, 
                ConsoleMenu.HIGHLIGHT_MARKER, 
                selectedPanel == 1
            );

            bottomPanel.SetLeftContent(leftList.ToArray());
            bottomPanel.SetCenterContent(centerList.ToArray());
            bottomPanel.SetRightContent("");

            bottomPanel.RenderWithColors();
        }

        
        private (string[] left, string[] center, string[] right) GetCharacterDisplayData(string characterName)
        {
            Fighter fighter = null;
            
          
            switch (characterName)
            {
                case "Ryu":
                    fighter = new Ryu();
                    break;
                
                case "Ken":
                    fighter = new Ken();
                    break;
                
                case "Chun-Li":
                    fighter = new ChunLi();
                    break;
                
                case "Guile":
                    fighter = new Guile();
                    break;
                
                case "Blanka":
                    fighter = new Blanka();
                    break;
                
                case "E. Honda":
                    fighter = new eHonda();
                    break;
                
                case "Dhalsim":
                    fighter = new Dhalsim();
                    break;
                
                case "Zangief":
                    fighter = new Zangief();
                    break;
                
                default:
                    return (
                        new string[0],
                        new string[] { "Unknown" },
                        new string[] { "Character not found" }
                    );
            }

            
            if (fighter != null)
            {
                string[] rightInfo = BuildRightPanelInfo(
                    fighter.Name,
                    fighter.Nationality,
                    fighter.Background,
                    fighter.MaxHealth,
                    fighter.AttackPower,
                    fighter.DefensePower,
                    fighter.SpecialMovePower,
                    fighter.SpecialMoveCost
                );

                return (
                    fighter.CharacterSelectLeft,  
                    new string[] { fighter.Name }, 
                    rightInfo                    
                );
            }

            
            return (
                new string[0],
                new string[] { "Error" },
                new string[] { "Could not load character" }
            );
        }

        
        private string[] BuildRightPanelInfo(string name, string nationality, string background, 
            int health, int attack, int defense, int specialPower, int specialCost)
        {
            List<string> info = new List<string>();
            
            info.Add($"Nationality: {nationality}");
            info.Add("");
            info.Add("Background:");
            
          
            string[] backgroundLines = background.Split('\n');
            foreach (string line in backgroundLines)
            {
                info.Add(line);
            }
            
            info.Add("");
            info.Add($"Health: {health}");
            info.Add($"Attack Power: {attack}");
            info.Add($"Defense Power: {defense}");
            info.Add($"Special Move Power: {specialPower}");
            info.Add($"Special Move Cost: {specialCost}");
            
            return info.ToArray();
        }

        private void HandleCharacterSelection(string character)
        {
            Fighter player1 = CreateFighterInstance(character);
            
            AIOpponent ai = new AIOpponent();
            Fighter player2 = ai.GetRandomCharacter();
            
            GameBattle battle = new GameBattle(player1, player2, audioManager);
            battle.Show();
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