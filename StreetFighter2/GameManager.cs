using System;
using static System.Console;

namespace StreetFighter2
{
    // ============================================================
    // ABSTRACTION: Interface for game modes
    // ============================================================
    public interface IGameMode
    {
        void Execute(AudioManager audioManager);
    }

    // ============================================================
    // POLYMORPHISM: Different game mode implementations
    // ============================================================
    public class StoryModeGame : IGameMode
    {
        public void Execute(AudioManager audioManager)
        {
            CharacterSelect characterSelect = new CharacterSelect(audioManager);
            characterSelect.Show();
        }
    }

    public class VersusModeGame : IGameMode
    {
        public void Execute(AudioManager audioManager)
        {
            WriteLine("V.S. Battle mode - Coming Soon!");
            ReadKey(true);
        }
    }

    public class OptionModeGame : IGameMode
    {
        public void Execute(AudioManager audioManager)
        {
            WriteLine("Options menu - Coming Soon!");
            ReadKey(true);
        }
    }

    class GameManager
    {
        // ============================================================
        // ENCAPSULATION: Private fields
        // ============================================================
        private readonly AudioManager audioManager;
        private readonly string menuMusicPath;
        private readonly float defaultVolume;

        public GameManager()
        {
            audioManager = new AudioManager();
            menuMusicPath = "Audio/menu_theme.mp3";
            defaultVolume = 0.5f;
        }

        // ============================================================
        // ENCAPSULATION: Extract checkpoint logic
        // ============================================================
        public void ShowCheckpoint()
        {
            Clear();
            WriteLine(@"
===========================================================
For the best Experience, Please set the console to fullscreen

Once in fullscreen press any key to continue......
=============================================================
            ");
            ReadKey(true);
            Clear();
        }

        public void StartGame()
        {
            Title = "Street Fighter Demo";
            ShowCheckpoint();
            RunMainMenu();
        }

        // ============================================================
        // ENCAPSULATION: Private menu logic
        // ============================================================
        private void RunMainMenu()
        {
            CursorVisible = false;
            Clear();

            audioManager.PlayMusic(menuMusicPath, loop: true);
            audioManager.SetVolume(defaultVolume);

            DisplayTitle();
            int selectedOption = DisplayMenuOptions();

            CursorVisible = true;

            ExecuteMenuOption(selectedOption);
        }

        private void DisplayTitle()
        {
            PanelConstructor titlePanel = new PanelConstructor(10, 10)
            {
                CenterAlignH = HorizontalAlign.Center,
                CenterAlignV = VerticalAlign.Top,
                ShowBorders = false
            };

            string title = @"
   ▄████████     ███        ▄████████    ▄████████    ▄████████     ███     
  ███    ███ ▀█████████▄   ███    ███   ███    ███   ███    ███ ▀█████████▄ 
  ███    █▀     ▀███▀▀██   ███    ███   ███    █▀    ███    █▀     ▀███▀▀██ 
  ███            ███   ▀  ▄███▄▄▄▄██▀  ▄███▄▄▄      ▄███▄▄▄         ███   ▀ 
▀███████████     ███     ▀▀███▀▀▀▀▀   ▀▀███▀▀▀     ▀▀███▀▀▀         ███     
         ███     ███     ▀███████████   ███    █▄    ███    █▄      ███     
   ▄█    ███     ███       ███    ███   ███    ███   ███    ███     ███     
 ▄████████▀     ▄████▀     ███    ███   ██████████   ██████████    ▄████▀   
                           ███    ███                                       


   ▄████████  ▄█     ▄██████▄     ▄█    █▄        ███        ▄████████    ▄████████       ▄█   ▄█  
  ███    ███ ███    ███    ███   ███    ███   ▀█████████▄   ███    ███   ███    ███      ███  ███  
  ███    █▀  ███▌   ███    █▀    ███    ███      ▀███▀▀██   ███    █▀    ███    ███      ███▌ ███▌ 
 ▄███▄▄▄     ███▌  ▄███         ▄███▄▄▄▄███▄▄     ███   ▀  ▄███▄▄▄      ▄███▄▄▄▄██▀      ███▌ ███▌ 
▀▀███▀▀▀     ███▌ ▀▀███ ████▄  ▀▀███▀▀▀▀███▀      ███     ▀▀███▀▀▀     ▀▀███▀▀▀▀▀        ███▌ ███▌ 
  ███        ███    ███    ███   ███    ███       ███       ███    █▄  ▀███████████      ███  ███  
  ███        ███    ███    ███   ███    ███       ███       ███    ███   ███    ███      ███  ███  
  ███        █▀     ████████▀    ███    █▀       ▄████▀     ██████████   ███    ███      █▀   █▀   
                                                                         ███    ███                
";

            string[] titleLines = title.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            titlePanel.SetCenterContent(titleLines);
            titlePanel.Render();
        }

        private int DisplayMenuOptions()
        {
            var menuPanel = new PanelConstructor(80, 80, 30)
            {
                CenterAlignH = HorizontalAlign.Center,
                CenterAlignV = VerticalAlign.Middle,
                ShowBorders = false
            };

            string[] options = { "GAME START", "V.S. BATTLE", "OPTION MODE", "QUIT" };
            return menuPanel.RenderInteractiveMenu(options);
        }

        // ============================================================
        // POLYMORPHISM: Use strategy pattern for menu options
        // ============================================================
        private void ExecuteMenuOption(int selectedIndex)
        {
            IGameMode gameMode = selectedIndex switch
            {
                0 => new StoryModeGame(),
                1 => new VersusModeGame(),
                2 => new OptionModeGame(),
                3 => null,
                _ => new StoryModeGame()
            };

            if (gameMode == null)
            {
                Quit();
            }
            else
            {
                gameMode.Execute(audioManager);
            }
        }

        private void Quit()
        {
            audioManager.Dispose();
            Environment.Exit(0);
        }
    }
}
