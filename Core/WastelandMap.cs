using DarnDroidz;
using DarnDroidz.Core;
using RLNET;
using RogueSharp;
using System.Collections.Generic;
using System.Linq;

namespace DarnDroidz.Core
{
    public class WastelandMap : Map
    {
        private const char FloorSymbol = '.';
        private const char WallSymbol = '#';

        private readonly List<Droid> _droids;
        private readonly List<Core.Equipment> _equipment;
        public List<Rectangle> Sectors { get; }
        public List<Door> Doors { get; set; }

        public Collapse CollapseUp { get; set; }
        public Collapse CollapseDown { get; set; }

        public WastelandMap()
        {
            Game.SchedulingSystem.Clear();
            _droids = new List<Droid>();
            _equipment = new List<Equipment>();
            Sectors = new List<Rectangle>();
            Doors = new List<Door>();
        }

        public void Draw(RLConsole mapConsole, RLConsole statConsole)
        {
            foreach (Cell cell in GetAllCells())
                if (cell.IsExplored)
                    DrawCell(mapConsole, cell);

            foreach (Core.Equipment equip in _equipment)
                equip.Draw(mapConsole, this);

            int i = 0;
            foreach (Droid droid in _droids)
            {
                droid.Draw(mapConsole, this);
                if (IsInFov(droid.X, droid.Y))
                    droid.DrawStats(statConsole, i++);
            }

            foreach (Door door in Doors)
                door.Draw(mapConsole, this);

            CollapseUp?.Draw(mapConsole, this);
            CollapseDown?.Draw(mapConsole, this);
        }

        private void DrawCell(RLConsole console, Cell cell)
        {
            bool inFov = IsInFov(cell.X, cell.Y);
            char symbol = cell.IsWalkable ? FloorSymbol : WallSymbol;

            RLColor fg = cell.IsWalkable
                ? (inFov ? Colors.FloorFov : Colors.Floor)
                : (inFov ? Colors.WallFov : Colors.Wall);

            RLColor bg = cell.IsWalkable
                ? (inFov ? Colors.FloorBackgroundFov : Colors.FloorBackground)
                : (inFov ? Colors.WallBackgroundFov : Colors.WallBackground);

            console.Set(cell.X, cell.Y, fg, bg, symbol);
        }

        public void UpdatePlayerFieldOfView(Player player)
        {
            if (player == null)
                return;

            ComputeFov(player.X, player.Y, player.Awareness, true);

            foreach (Cell cell in GetAllCells())
                if (IsInFov(cell.X, cell.Y))
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            Cell targetCell = (Cell)GetCell(x, y);
            if (targetCell == null)
                return false;

            Door door = GetDoor(x, y);
            if (door != null && !door.IsOpen)
            {
                OpenDoor(actor, x, y);
                if (!door.IsOpen)
                    return false;
            }

            if (!targetCell.IsWalkable)
                return false;

            SetIsWalkable(actor.X, actor.Y, true);
            actor.X = x;
            actor.Y = y;
            SetIsWalkable(actor.X, actor.Y, false);

            if (actor is Player player)
            {
                UpdatePlayerFieldOfView(player);
                TryPickUpEquipment(player);
            }

            return true;
        }

        private void TryPickUpEquipment(Player player)
        {
            Core.Equipment equipment = _equipment.FirstOrDefault(e => e.X == player.X && e.Y == player.Y);
            if (equipment != null)
            {
                equipment.PickUp(player);
                _equipment.Remove(equipment);
            }
        }

        public void AddEquipment(Core.Equipment equipment)
        {
            if (equipment != null)
            {
                _equipment.Add(equipment);
            }
        }

        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = (Cell)GetCell(x, y);
            if (cell != null)
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView(player);
            Game.SchedulingSystem.Add(player);
        }

        public void AddDroid(Droid droid)
        {
            if (droid == null)
                return;

            _droids.Add(droid);
            SetIsWalkable(droid.X, droid.Y, false);
            Game.SchedulingSystem.Add(droid);
        }

        public void RemoveDroid(Droid droid)
        {
            if (droid == null)
                return;

            if (_droids.Remove(droid))
                SetIsWalkable(droid.X, droid.Y, true);

            Game.SchedulingSystem.Remove(droid);
        }

        public Droid GetDroidAt(int x, int y)
        {
            return _droids.FirstOrDefault(d => d.X == x && d.Y == y);
        }

        public IEnumerable<Point> GetDroidLocations()
        {
            return _droids.Select(d => new Point(d.X, d.Y));
        }

        public IEnumerable<Point> GetDroidLocationsInFieldOfView()
        {
            return _droids.Where(d => IsInFov(d.X, d.Y))
                          .Select(d => new Point(d.X, d.Y));
        }

        public Point? GetRandomWalkableLocationInSector(Rectangle sector)
        {
            if (!DoesSectorHaveWalkableSpace(sector))
                return null;

            for (int i = 0; i < 100; i++)
            {
                int x = Game.Random.Next(1, sector.Width - 2) + sector.X;
                int y = Game.Random.Next(1, sector.Height - 2) + sector.Y;

                if (IsWalkable(x, y))
                    return new Point(x, y);
            }

            return null;
        }

        public bool DoesSectorHaveWalkableSpace(Rectangle sector)
        {
            for (int x = 1; x <= sector.Width - 2; x++)
                for (int y = 1; y <= sector.Height - 2; y++)
                    if (IsWalkable(x + sector.X, y + sector.Y))
                        return true;

            return false;
        }

        public IEnumerable<Droid> GetAllDroids() => _droids;

        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        private bool TryGetSectorAt(int x, int y, out Rectangle sector)
        {
            foreach (var s in Sectors)
            {
                if (s.Contains(x, y))
                {
                    sector = s;
                    return true;
                }
            }

            sector = default;
            return false;
        }

        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door == null || door.IsOpen)
                return;

            if (TryGetSectorAt(x, y, out Rectangle sector) &&
                _droids.Any(d => sector.Contains(d.X, d.Y)))
            {
                Game.MessageLog.Add("Security lockdown engaged!");
                return;
            }

            door.IsOpen = true;

            Cell cell = (Cell)GetCell(x, y);
            SetCellProperties(x, y, true, true, cell.IsExplored);

            Game.MessageLog.Add($"{actor.Name} opened a door!");
        }

        public bool CanMoveDownToNextLevel()
        {
            return CollapseDown != null &&
                   Game.Player != null &&
                   CollapseDown.X == Game.Player.X &&
                   CollapseDown.Y == Game.Player.Y;
        }

        public void PlaceDoor(int x, int y)
        {
            var door = new Door { X = x, Y = y, IsOpen = false };
            Doors.Add(door);
            SetCellProperties(x, y, false, true, true);
        }
    }
}