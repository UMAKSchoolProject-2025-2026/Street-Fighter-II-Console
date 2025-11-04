using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace StreetFighter2
{
    class GameManager
    {
        private AudioManager audioManger = new AudioManager();
        public void checkpoint()
        {
            Console.Clear();
            WriteLine(@"
===========================================================
For the best Experience, Please set the console to fullscreen

Once in fullscreen pressy any key to continue......
=============================================================
            ");
            Console.ReadKey(true);

            Console.Clear();
        }

        public void StartGame()
        {
            Title = "Street Fighter Demo";
            runMainMenu();
        }
        private void runMainMenu()
        {
            CursorVisible = false;
            Clear();

            audioManger.PlayMusic("Audio/menu_theme.mp3", loop: true);
            audioManger.SetVolume(0.5f);

            PanelConstructor titlePanel = new PanelConstructor(10, 10);

            titlePanel.CenterAlignH = HorizontalAlign.Center;
            titlePanel.CenterAlignV = VerticalAlign.Top;
            titlePanel.ShowBorders = false;



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

            var menuPanel = new PanelConstructor(80, 80, 30); 
            menuPanel.CenterAlignH = HorizontalAlign.Center;
            menuPanel.CenterAlignV = VerticalAlign.Middle;
            menuPanel.ShowBorders = false;
            string[] options = { "GAME START", "V.S. BATTLE", "OPTION MODE", "QUIT" };

            
            int selectedIndex = menuPanel.RenderInteractiveMenu(options);

            CursorVisible = true;

            
            switch (selectedIndex)
            {
                case 0:
                    gameStart();
                    break;
                case 1:
                    v_s_Battle();
                    break;
                case 2:
                    optionMode();
                    break;
                case 3:
                    quit();
                    break;
            }
        }

        private void gameStart()
        {
            CharacterSelect characterSelect = new CharacterSelect(audioManger);
            characterSelect.Show();
        }

        private void v_s_Battle()
        {

        }

        private void optionMode()
        {

        }

        private void quit()
        {
            Environment.Exit(0);
        }
    }
}
