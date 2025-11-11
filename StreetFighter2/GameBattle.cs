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
    class GameBattle
    {
        private Fighter player1;
        private Fighter player2;
        private AudioManager audioManager;
        private int selectedPanel = 0;
        private int selectedIndex = 0;

        private StatusBar player1HealthBar;
        private StatusBar player1SpecialBar;
        private StatusBar player2HealthBar;
        private StatusBar player2SpecialBar;

        private CombatResolver combatResolver;
        private AIOpponent aiOpponent;

        
        private TurnResult lastTurnResult;
        private Move lastPlayer1Move;
        private Move lastPlayer2Move;

        public GameBattle(Fighter player1, Fighter player2, AudioManager audioManager = null)
        {
            this.player1 = player1;
            this.player2 = player2;
            this.audioManager = audioManager;

            player1HealthBar = new StatusBar(player1.CurrentHealth, player1.MaxHealth);
            player1SpecialBar = new StatusBar(player1.SpBar, player1.SpBarMax);
            player2HealthBar = new StatusBar(player2.CurrentHealth, player2.MaxHealth);
            player2SpecialBar = new StatusBar(player2.SpBar, player2.SpBarMax);

            combatResolver = new CombatResolver();
            aiOpponent = new AIOpponent();

            lastTurnResult = null;
            lastPlayer1Move = Move.LightAttack;
            lastPlayer2Move = Move.LightAttack;
        }

        public void Show()
        {
            CursorVisible = false;

            try
            {
                SetBufferSize(WindowWidth, WindowHeight);
            }
            catch { }

            bool battleActive = true;

            while (battleActive)
            {
                int p1Result = GetPlayerAction();
                var (p1Panel, p1Index) = ConsoleMenu.DecodeMultiPanelResult(p1Result);
                Move player1Move = GetMoveFromSelection(p1Panel, p1Index);

                Move player2Move = aiOpponent.GetMove();

                
                lastPlayer1Move = player1Move;
                lastPlayer2Move = player2Move;

                TurnResult turnResult = combatResolver.ResolveTurn(
                    player1, player2, 
                    player1Move, player2Move,
                    player1.Name, player2.Name
                );

                
                lastTurnResult = turnResult;

                UpdateStatusBars();

                ShowTurnResult(turnResult);

                if (player1.CurrentHealth <= 0 || player2.CurrentHealth <= 0)
                {
                    battleActive = false;
                    ShowBattleResult();
                }
            }

            CursorVisible = true;
        }

        private Move GetMoveFromSelection(int panel, int index)
        {
            switch (panel)
            {
                case 0:
                    return index == 0 ? Move.LightAttack : Move.HeavyAttack;
                case 1:
                    return index == 0 ? Move.Block : Move.Dodge;
                case 2:
                    return Move.Special;
                default:
                    return Move.LightAttack;
            }
        }

        private void ShowTurnResult(TurnResult result)
        {
            
            RenderFullScreen();
            Thread.Sleep(2000);
        }

        private int GetPlayerAction()
        {
            ConsoleKey keyPressed;

            do
            {
                UpdateStatusBars();
                RenderFullScreen();

                ConsoleKeyInfo keyInfo = ReadKey(true);
                keyPressed = keyInfo.Key;

                string[][] panelOptions = new string[][]
                {
                    new string[] { "Light Attack", "Heavy Attack" },
                    new string[] { "Block", "Dodge" },
                    new string[] { "Special" }
                };

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

            return (selectedPanel * 100) + selectedIndex;
        }

        private void UpdateStatusBars()
        {
            player1HealthBar.UpdateValue(player1.CurrentHealth);
            player1SpecialBar.UpdateValue(player1.SpBar);
            player2HealthBar.UpdateValue(player2.CurrentHealth);
            player2SpecialBar.UpdateValue(player2.SpBar);
        }

        private void RenderFullScreen()
        {
            
            Clear();
            
            SetCursorPosition(0, 0);

            RenderTopPanel();
            RenderMiddlePanel();
            RenderBottomPanel();
        }

        private void RenderTopPanel()
        {
            int sideWidth = (WindowWidth - 2) / 3;

            PanelConstructor topPanel = new PanelConstructor(sideWidth, sideWidth, showBorders: false);

            topPanel.LeftAlignH = HorizontalAlign.Left;
            topPanel.CenterAlignH = HorizontalAlign.Center;
            topPanel.RightAlignH = HorizontalAlign.Right;

            topPanel.LeftAlignV = VerticalAlign.Top;
            topPanel.CenterAlignV = VerticalAlign.Middle;
            topPanel.RightAlignV = VerticalAlign.Top;

            string[] p1Content = BuildPlayerStats($"P1 - {player1.Name}", player1HealthBar, player1SpecialBar, player1);
            string[] vsContent = { "VS" };
            string[] p2Content = BuildPlayerStats($"P2 - {player2.Name}", player2HealthBar, player2SpecialBar, player2);

            topPanel.SetLeftContent(p1Content);
            topPanel.SetCenterContent(vsContent);
            topPanel.SetRightContent(p2Content);

            topPanel.Render();
        }

        private string[] BuildPlayerStats(string playerLabel, StatusBar healthBar, StatusBar specialBar, Fighter player)
        {
            List<string> stats = new List<string>();

            stats.Add(playerLabel);
            stats.Add("");
            stats.Add($"HEALTH: {healthBar.CurrentValue}/{healthBar.MaxValue}");
            stats.Add(healthBar.BuildBar());
            stats.Add("");
            
            
            bool specialReady = specialBar.IsReady(player.SpecialMoveCost);
            string readyIndicator = specialReady ? " * READY!" : "";
            
            stats.Add($"SPECIAL: {specialBar.CurrentValue}/{specialBar.MaxValue}{readyIndicator}");
            stats.Add(specialBar.BuildBar());

            return stats.ToArray();
        }

        private void RenderMiddlePanel()
        {
            PanelConstructor middlePanel = new PanelConstructor(60, 60, 45, showBorders: false);

            middlePanel.LeftAlignH = HorizontalAlign.Center;
            middlePanel.CenterAlignH = HorizontalAlign.Center;
            middlePanel.RightAlignH = HorizontalAlign.Center;

            middlePanel.LeftAlignV = VerticalAlign.Top;
            middlePanel.CenterAlignV = VerticalAlign.Middle;
            middlePanel.RightAlignV = VerticalAlign.Top;

            if (lastTurnResult != null)
            {
               
                string[] p1Actions = BuildPlayerActions("P1", player1.Name, lastPlayer1Move, lastTurnResult, true);
                
                
                string[] p2Actions = BuildPlayerActions("P2", player2.Name, lastPlayer2Move, lastTurnResult, false);

                
                string[] centerContent = lastTurnResult.Logs.ToArray();

                middlePanel.SetLeftContent(p1Actions);
                middlePanel.SetCenterContent(centerContent);
                middlePanel.SetRightContent(p2Actions);
            }
            else
            {
               
                middlePanel.SetLeftContent("");
                middlePanel.SetCenterContent("");
                middlePanel.SetRightContent("");
            }

            middlePanel.Render();
        }

        private string[] BuildPlayerActions(string playerPrefix, string playerName, Move move, TurnResult result, bool isPlayer1)
        {
            List<string> actions = new List<string>();

          
            string moveName = GetMoveName(move);
            actions.Add($"{playerPrefix} - {playerName}");
            actions.Add($"used {moveName}");
            actions.Add("");
            
            
            int damageDealt = isPlayer1 ? result.DamageToPlayer2 : result.DamageToPlayer1;
            int damageReceived = isPlayer1 ? result.DamageToPlayer1 : result.DamageToPlayer2;
            
            
            if (result.Cancelled)
            {
                actions.Add("Forces cancelled!");
            }
            else if (damageReceived > 0)  
    {
        actions.Add("<< Damage Taken:");
        actions.Add($"   {damageReceived} HP");
    }
    else if (damageDealt > 0)  
    {
        actions.Add(">> Damage Dealt:");
        actions.Add($"   {damageDealt} HP");
    }
    else
    {
        actions.Add("No damage");
    }

    return actions.ToArray();
}

        private string GetMoveName(Move move)
        {
            switch (move)
            {
                case Move.LightAttack:
                    return "Light Attack";
                case Move.HeavyAttack:
                    return "Heavy Attack";
                case Move.Block:
                    return "Block";
                case Move.Dodge:
                    return "Dodge";
                case Move.Special:
                    return "Special Move";
                default:
                    return "Unknown";
            }
        }

        private void RenderBottomPanel()
        {
            int totalWidth = WindowWidth;
            int borderSpace = 2;
            int columnWidth = (totalWidth - borderSpace) / 3;

            int column1Width = columnWidth;
            int column3Width = columnWidth;

            PanelConstructor bottomPanel = new PanelConstructor(column1Width, column3Width, showBorders: false);
            
            bottomPanel.LeftAlignH = HorizontalAlign.Center;
            bottomPanel.CenterAlignH = HorizontalAlign.Center;
            bottomPanel.RightAlignH = HorizontalAlign.Center;
            
            bottomPanel.LeftAlignV = VerticalAlign.Top;
            bottomPanel.CenterAlignV = VerticalAlign.Top;
            bottomPanel.RightAlignV = VerticalAlign.Top;

            string[] column1Options = { "Light Attack", "Heavy Attack" };
            string[] column2Options = { "Block", "Dodge" };
            string[] column3Options = { "Special" };

            var leftContent = BuildActionContent(column1Options, selectedPanel == 0, selectedIndex);
            var centerContent = BuildActionContent(column2Options, selectedPanel == 1, selectedIndex);
            var rightContent = BuildActionContent(column3Options, selectedPanel == 2, selectedIndex);

            bottomPanel.SetLeftContent(leftContent);
            bottomPanel.SetCenterContent(centerContent);
            bottomPanel.SetRightContent(rightContent);

            bottomPanel.RenderWithColors();
        }

        private string[] BuildActionContent(string[] options, bool isActive, int selectedIndex)
        {
            List<string> content = new List<string>();

            for (int i = 0; i < options.Length; i++)
            {
                if (isActive && i == selectedIndex)
                {
                    content.Add($"{ConsoleMenu.HIGHLIGHT_MARKER}>>> {options[i]} <<<");
                }
                else
                {
                    content.Add($"    {options[i]}");
                }
            }

            return content.ToArray();
        }

        private void ShowBattleResult()
        {
            SetCursorPosition(0, 0);
            string emptyLine = new string(' ', WindowWidth);

            for (int i = 0; i < WindowHeight; i++)
            {
                SetCursorPosition(0, i);
                Write(emptyLine);
            }

            SetCursorPosition(0, 0);

            RenderTopPanel();

            
            int sideWidth = (WindowWidth - 2) / 3;
            PanelConstructor middlePanel = new PanelConstructor(sideWidth, sideWidth, 45, showBorders: false);
            
            middlePanel.LeftAlignH = HorizontalAlign.Center;
            middlePanel.CenterAlignH = HorizontalAlign.Center;
            middlePanel.RightAlignH = HorizontalAlign.Center;
            
            middlePanel.LeftAlignV = VerticalAlign.Middle;
            middlePanel.CenterAlignV = VerticalAlign.Middle;
            middlePanel.RightAlignV = VerticalAlign.Middle;

            string winner = player1.CurrentHealth > 0 ? $"P1 - {player1.Name}" : $"P2 - {player2.Name}";
            string[] winContent = { 
                "═══════════════════",
                "",
                $"{winner}",
                "",
                "WINS!",
                "",
                "═══════════════════"
            };

            middlePanel.SetLeftContent("");
            middlePanel.SetCenterContent(winContent);
            middlePanel.SetRightContent("");

            middlePanel.Render();

            int totalWidth = WindowWidth;
            int borderSpace = 2;
            int columnWidth = (totalWidth - borderSpace) / 3;

            int column1Width = columnWidth;
            int column3Width = columnWidth;

            PanelConstructor bottomPanel = new PanelConstructor(column1Width, column3Width, showBorders: false);
            
            bottomPanel.LeftAlignH = HorizontalAlign.Center;
            bottomPanel.CenterAlignH = HorizontalAlign.Center;
            bottomPanel.RightAlignH = HorizontalAlign.Center;
            
            bottomPanel.LeftAlignV = VerticalAlign.Middle;
            bottomPanel.CenterAlignV = VerticalAlign.Middle;
            bottomPanel.RightAlignV = VerticalAlign.Middle;

            string[] continuePrompt = { "Press any key to continue..." };

            bottomPanel.SetLeftContent("");
            bottomPanel.SetCenterContent(continuePrompt);
            bottomPanel.SetRightContent("");
            
            bottomPanel.Render();

            ReadKey(true);
        }
    }
}