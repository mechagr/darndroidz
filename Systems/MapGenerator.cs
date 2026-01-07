using DarnDroidz.Core;
using DarnDroidz.Droids;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarnDroidz.Systems
{
    public class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxSectors;
        private readonly int _sectorMaxSize;
        private readonly int _sectorMinSize;
        private readonly int _mapLevel;
        private readonly WastelandMap _map;

        public MapGenerator(
            int width,
            int height,
            int maxSectors,
            int sectorMaxSize,
            int sectorMinSize,
            int mapLevel)
        {
            _width = width;
            _height = height;
            _maxSectors = maxSectors;
            _sectorMaxSize = sectorMaxSize;
            _sectorMinSize = sectorMinSize;
            _mapLevel = mapLevel;
            _map = new WastelandMap();
        }

        public WastelandMap CreateMap()
        {
            _map.Initialize(_width, _height);

            GenerateSectors();
            CarveSectors();
            ConnectSectors();

            foreach (var sector in _map.Sectors)
                CreateDoors(sector);

            CreateCollapses();
            PlacePlayer();
            PlaceDroids();
            PlaceEquipment();

            return _map;
        }

        private void GenerateSectors()
        {
            for (int i = 0; i < _maxSectors; i++)
            {
                int w = Game.Random.Next(_sectorMinSize, _sectorMaxSize);
                int h = Game.Random.Next(_sectorMinSize, _sectorMaxSize);
                int x = Game.Random.Next(0, _width - w - 1);
                int y = Game.Random.Next(0, _height - h - 1);

                var sector = new Rectangle(x, y, w, h);

                if (!_map.Sectors.Any(s => s.Intersects(sector)))
                    _map.Sectors.Add(sector);
            }
        }

        private void CarveSectors()
        {
            foreach (Rectangle sector in _map.Sectors)
            {
                for (int x = sector.Left + 1; x < sector.Right; x++)
                {
                    for (int y = sector.Top + 1; y < sector.Bottom; y++)
                    {
                        _map.SetCellProperties(x, y, true, true, true);
                    }
                }
            }
        }

        private void ConnectSectors()
        {
            for (int i = 1; i < _map.Sectors.Count; i++)
            {
                var prev = _map.Sectors[i - 1].Center;
                var curr = _map.Sectors[i].Center;

                if (Game.Random.Next(2) == 0)
                {
                    CreateHTunnel(prev.X, curr.X, prev.Y);
                    CreateVTunnel(prev.Y, curr.Y, curr.X);
                }
                else
                {
                    CreateVTunnel(prev.Y, curr.Y, prev.X);
                    CreateHTunnel(prev.X, curr.X, curr.Y);
                }
            }
        }

        private void CreateHTunnel(int x1, int x2, int y)
        {
            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
            {
                _map.SetCellProperties(x, y, true, true, true);
            }
        }

        private void CreateVTunnel(int y1, int y2, int x)
        {
            for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
            {
                _map.SetCellProperties(x, y, true, true, true);
            }
        }

        private void CreateDoors(Rectangle sector)
        {
            int xMin = sector.Left;
            int xMax = sector.Right;
            int yMin = sector.Top;
            int yMax = sector.Bottom;

            List<ICell> borderCells = new List<ICell>();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMax, yMin));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            foreach (ICell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    _map.PlaceDoor(cell.X, cell.Y);

                    _map.SetCellProperties(
                        cell.X,
                        cell.Y,
                        isTransparent: false,
                        isWalkable: true,
                        isExplored: true
                    );
                }
            }
        }

        private bool IsPotentialDoor(ICell cell)
        {
            if (!cell.IsWalkable)
                return false;

            ICell right = _map.GetCell(cell.X + 1, cell.Y);
            ICell left = _map.GetCell(cell.X - 1, cell.Y);
            ICell top = _map.GetCell(cell.X, cell.Y - 1);
            ICell bottom = _map.GetCell(cell.X, cell.Y + 1);

            if (_map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null)
                return false;

            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
                return true;

            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
                return true;

            return false;
        }

        private void CreateCollapses()
        {
            Rectangle first = _map.Sectors.First();
            Rectangle last = _map.Sectors.Last();

            _map.CollapseUp = new Collapse
            {
                X = first.Center.X,
                Y = first.Center.Y,
                IsUp = true
            };

            _map.CollapseDown = new Collapse
            {
                X = last.Center.X,
                Y = last.Center.Y,
                IsUp = false
            };
        }

        private void PlacePlayer()
        {
            Player player = Game.Player ?? new Player();
            var start = _map.Sectors.First().Center;

            player.X = start.X;
            player.Y = start.Y;

            _map.AddPlayer(player);
        }

        private void PlaceDroids()
        {
            foreach (Rectangle sector in _map.Sectors)
            {
                int count = Game.Random.Next(0, 3);

                for (int i = 0; i < count; i++)
                {
                    Point? p = _map.GetRandomWalkableLocationInSector(sector);
                    if (!p.HasValue)
                        continue;

                    Droid droid = CreateRandomDroid();
                    droid.X = p.Value.X;
                    droid.Y = p.Value.Y;

                    _map.AddDroid(droid);
                }
            }
        }

        private void PlaceEquipment()
        {
            EquipmentGenerator equipmentGenerator = new EquipmentGenerator(_mapLevel);

            foreach (Rectangle sector in _map.Sectors)
            {
                if (Game.Random.Next(0, 2) == 0)
                {
                    Point? p = _map.GetRandomWalkableLocationInSector(sector);
                    if (!p.HasValue)
                        continue;

                    Core.Equipment equipment = equipmentGenerator.CreateEquipment();
                    equipment.X = p.Value.X;
                    equipment.Y = p.Value.Y;

                    _map.AddEquipment(equipment);
                }
            }
        }

        private Droid CreateRandomDroid()
        {
            int roll = Game.Random.Next(0, 3);

            return roll switch
            {
                0 => ScrapBot.Create(_mapLevel),
                1 => JunkRoller.Create(_mapLevel),
                _ => WireCrawler.Create(_mapLevel)
            };
        }
    }
}