using DarnDroidz.Core;
using DarnDroidz.Systems;
using RLNET;
using RogueSharp;
using RogueSharp.Random;
using System;

namespace DarnDroidz
{
    public class Game
    {
        private const int ScreenWidth = 100;
        private const int ScreenHeight = 70;
        private const int MapWidth = 80;
        private const int MapHeight = 48;
        private const int MessageWidth = 80;
        private const int MessageHeight = 11;
        private const int StatWidth = 20;
        private const int StatHeight = 70;
        private const int InventoryWidth = 80;
        private const int InventoryHeight = 11;

        private static RLRootConsole _rootConsole;
        private static RLConsole _mapConsole;
        private static RLConsole _messageConsole;
        private static RLConsole _statConsole;
        private static RLConsole _inventoryConsole;

        private static bool _renderRequired = true;
        private static int _mapLevel = 1;
        private static GameState _gameState = GameState.TitleScreen;

        public static SchedulingSystem SchedulingSystem { get; private set; }
        public static WastelandMap WastelandMap { get; private set; }
        public static Player Player { get; set; }
        public static CommandSystem CommandSystem { get; private set; }
        public static IRandom Random { get; private set; }

        public static MessageLog MessageLog { get; private set; } = MessageLog.Instance;

        public static void Main()
        {
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            InitializeConsoles("DARNDROIDZ");

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;
            _rootConsole.Run();
        }

        private static void InitializeConsoles(string title)
        {
            _rootConsole = new RLRootConsole(
                "terminal8x8.png",
                ScreenWidth,
                ScreenHeight,
                8, 8, 1f, title);

            _mapConsole = new RLConsole(MapWidth, MapHeight);
            _messageConsole = new RLConsole(MessageWidth, MessageHeight);
            _statConsole = new RLConsole(StatWidth, StatHeight);
            _inventoryConsole = new RLConsole(InventoryWidth, InventoryHeight);
        }

        private static void StartNewGame()
        {
            _mapLevel = 1;
            Player = new Player();

            SchedulingSystem = new SchedulingSystem();
            CommandSystem = new CommandSystem();

            InitializeGameObjects();
            InitializeStaticConsoles();

            MessageLog.Add($"Tucker enters Ruin Layer {_mapLevel}!");
            MessageLog.Add($"Hostile droids detected... Stay sharp!");

            _gameState = GameState.Playing;
            _renderRequired = true;
        }

        private static void InitializeGameObjects()
        {
            MapGenerator generator = new MapGenerator(MapWidth, MapHeight, 20, 13, 7, _mapLevel);
            WastelandMap = generator.CreateMap();
            WastelandMap.UpdatePlayerFieldOfView(Player);
        }

        private static void InitializeStaticConsoles()
        {
            _messageConsole.SetBackColor(0, 0, MessageWidth, MessageHeight, Swatch.AlternateDarker);
            _messageConsole.Print(1, 1, "[MESSAGES]", Swatch.AlternateLightest);

            _inventoryConsole.SetBackColor(0, 0, InventoryWidth, InventoryHeight, Swatch.SecondaryDarker);
            _inventoryConsole.Print(1, 1, "[INVENTORY]", Swatch.DbLight);
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            if (_gameState == GameState.TitleScreen)
            {
                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.Enter || keyPress.Key == RLKey.Space)
                    {
                        StartNewGame();
                    }
                    else if (keyPress.Key == RLKey.Escape)
                    {
                        _rootConsole.Close();
                    }
                }
                _renderRequired = true;
                return;
            }

            if (_gameState == GameState.GameOver)
            {
                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.Enter || keyPress.Key == RLKey.Space)
                    {
                        StartNewGame();
                    }
                    else if (keyPress.Key == RLKey.Escape)
                    {
                        _rootConsole.Close();
                    }
                }
                _renderRequired = true;
                return;
            }

            bool didPlayerAct = false;

            if (CommandSystem.IsPlayerTurn)
            {
                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.Up)
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                    else if (keyPress.Key == RLKey.Down)
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                    else if (keyPress.Key == RLKey.Left)
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                    else if (keyPress.Key == RLKey.Right)
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                    else if (keyPress.Key == RLKey.Period)
                    {
                        if (WastelandMap.CanMoveDownToNextLevel())
                        {
                            didPlayerAct = TryMoveDownLevel();
                        }
                        else
                        {
                            MessageLog.Add("You need to find a collapse (>) to descend deeper!");
                        }
                    }
                    else if (keyPress.Key == RLKey.Escape)
                        _rootConsole.Close();
                }

                if (didPlayerAct)
                {
                    _renderRequired = true;
                    CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                CommandSystem.ActivateDroids();
                _renderRequired = true;
            }

            if (Player != null && Player.Health <= 0)
            {
                _gameState = GameState.GameOver;
                _renderRequired = true;
            }
        }

        private static bool TryMoveDownLevel()
        {
            if (!WastelandMap.CanMoveDownToNextLevel())
                return false;

            MessageLog.Add($"Tucker descends through the collapsed floor!");
            _mapLevel++;

            MapGenerator generator = new MapGenerator(MapWidth, MapHeight, 20, 13, 7, _mapLevel);
            WastelandMap = generator.CreateMap();
            CommandSystem = new CommandSystem();

            _rootConsole.Title = $"DARNDROIDZ - Ruin Layer {_mapLevel}";
            MessageLog.Add($"Ruin Layer {_mapLevel} - The wasteland goes deeper...");

            return true;
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (!_renderRequired)
                return;

            _rootConsole.Clear();

            if (_gameState == GameState.TitleScreen)
            {
                DrawTitleScreen();
            }
            else if (_gameState == GameState.GameOver)
            {
                DrawGameOverScreen();
            }
            else
            {
                DrawGameplay();
            }

            _rootConsole.Draw();
            _renderRequired = false;
        }

        private static void DrawTitleScreen()
        {
            _rootConsole.Clear();

            int centerX = ScreenWidth / 2;
            int startY = 15;

            string[] title = new string[]
            {
                "██████╗  █████╗ ██████╗ ███╗   ██╗██████╗ ██████╗  ██████╗ ██╗██████╗ ███████╗",
                "██╔══██╗██╔══██╗██╔══██╗████╗  ██║██╔══██╗██╔══██╗██╔═══██╗██║██╔══██╗╚══███╔╝",
                "██║  ██║███████║██████╔╝██╔██╗ ██║██║  ██║██████╔╝██║   ██║██║██║  ██║  ███╔╝ ",
                "██║  ██║██╔══██║██╔══██╗██║╚██╗██║██║  ██║██╔══██╗██║   ██║██║██║  ██║ ███╔╝  ",
                "██████╔╝██║  ██║██║  ██║██║ ╚████║██████╔╝██║  ██║╚██████╔╝██║██████╔╝███████╗",
                "╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚═╝╚═════╝ ╚══════╝"
            };

            for (int i = 0; i < title.Length; i++)
            {
                int x = centerX - (title[i].Length / 2);
                _rootConsole.Print(x, startY + i, title[i], Colors.Player);
            }

            string subtitle = "WASTELAND PROTOCOL";
            _rootConsole.Print(centerX - (subtitle.Length / 2), startY + title.Length + 2, subtitle, Swatch.DbSun);

            int storyY = startY + title.Length + 5;
            _rootConsole.Print(centerX - 35, storyY, "You are Tucker, a robotic dog exploring the ruins of a collapsed wasteland.", Swatch.AlternateLighter);
            _rootConsole.Print(centerX - 35, storyY + 1, "The world is layered with destroyed cities stacked on top of one another...", Swatch.AlternateLighter);
            _rootConsole.Print(centerX - 35, storyY + 2, "Hostile droids patrol each level, becoming more dangerous the deeper you go.", Swatch.AlternateLighter);

            int controlsY = storyY + 5;
            _rootConsole.Print(centerX - 10, controlsY, "=== CONTROLS ===", Colors.TextHeading);
            _rootConsole.Print(centerX - 20, controlsY + 2, "[ARROW KEYS] - Move", RLColor.White);
            _rootConsole.Print(centerX - 20, controlsY + 3, "[.] - Descend", RLColor.White);
            _rootConsole.Print(centerX - 20, controlsY + 4, "[ESC] - Exit", RLColor.White);

            _rootConsole.Print(centerX - 22, controlsY + 7, "Press [ENTER] or SPACE to begin scavenging...", Swatch.DbLight);
            _rootConsole.Print(centerX - 12, controlsY + 8, "Press [ESC] to quit", Swatch.DbOldStone);

        }

        private static void DrawGameOverScreen()
        {
            _rootConsole.Clear();

            int centerX = ScreenWidth / 2;
            int startY = 20;

            string[] gameOver = new string[]
            {
                "██████╗  █████╗ ███╗   ███╗███████╗     ██████╗ ██╗   ██╗███████╗██████╗ ",
                "██╔════╝ ██╔══██╗████╗ ████║██╔════╝    ██╔═══██╗██║   ██║██╔════╝██╔══██╗",
                "██║  ███╗███████║██╔████╔██║█████╗      ██║   ██║██║   ██║█████╗  ██████╔╝",
                "██║   ██║██╔══██║██║╚██╔╝██║██╔══╝      ██║   ██║╚██╗ ██╔╝██╔══╝  ██╔══██╗",
                "╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗    ╚██████╔╝ ╚████╔╝ ███████╗██║  ██║",
                " ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝     ╚═════╝   ╚═══╝  ╚══════╝╚═╝  ╚═╝"
            };

            for (int i = 0; i < gameOver.Length; i++)
            {
                int x = centerX - (gameOver[i].Length / 2);
                _rootConsole.Print(x, startY + i, gameOver[i], Swatch.DbBlood);
            }

            string deathMessage = "TUCKER'S CIRCUITS HAVE FAILED.";
            _rootConsole.Print(centerX - (deathMessage.Length / 2), startY + gameOver.Length + 3, deathMessage, RLColor.White);

            string loreMessage = "Your salvaged parts rust among the ruins...";
            _rootConsole.Print(centerX - (loreMessage.Length / 2), startY + gameOver.Length + 4, loreMessage, Swatch.AlternateLighter);

            string levelReached = $"You reached Ruin Layer {_mapLevel}";
            _rootConsole.Print(centerX - (levelReached.Length / 2), startY + gameOver.Length + 6, levelReached, Swatch.DbSun);

            _rootConsole.Print(centerX - 20, startY + gameOver.Length + 10, "Press [ENTER] or [SPACE] to reboot Tucker...", Swatch.DbLight);
            _rootConsole.Print(centerX - 18, startY + gameOver.Length + 11, "Press [ESC] to return to the surface.", Swatch.DbOldStone);
        }

        private static void DrawGameplay()
        {
            WastelandMap.Draw(_mapConsole, _statConsole);
            Player.Draw(_mapConsole, WastelandMap);
            Player.DrawStats(_statConsole);
            MessageLog.Draw(_messageConsole);

            RLConsole.Blit(_mapConsole, 0, 0, MapWidth, MapHeight, _rootConsole, 0, InventoryHeight);
            RLConsole.Blit(_statConsole, 0, 0, StatWidth, StatHeight, _rootConsole, MapWidth, 0);
            RLConsole.Blit(_messageConsole, 0, 0, MessageWidth, MessageHeight, _rootConsole, 0, ScreenHeight - MessageHeight);
            RLConsole.Blit(_inventoryConsole, 0, 0, InventoryWidth, InventoryHeight, _rootConsole, 0, 0);
        }
    }
}