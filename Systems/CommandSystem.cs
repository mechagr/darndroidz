using DarnDroidz.Core;
using DarnDroidz.Interfaces;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Text;

namespace DarnDroidz.Systems
{
    public class CommandSystem
    {
        public bool IsPlayerTurn { get; set; }

        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.X;
            int y = Game.Player.Y;

            switch (direction)
            {
                case Direction.Up: y--; break;
                case Direction.Down: y++; break;
                case Direction.Left: x--; break;
                case Direction.Right: x++; break;
                default: return false;
            }

            Droid droid = Game.WastelandMap.GetDroidAt(x, y);
            if (droid != null)
            {
                Attack(Game.Player, droid);
                return true;
            }

            return Game.WastelandMap.SetActorPosition(Game.Player, x, y);
        }

        public void Attack(Actor attacker, Actor defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);
            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            Game.MessageLog.Add(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                Game.MessageLog.Add(defenseMessage.ToString());
            }

            int damage = Math.Max(0, hits - blocks);
            ResolveDamage(defender, damage);
        }

        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            int hits = 0;
            attackMessage.AppendFormat("{0} attacks {1} and rolls: ", attacker.Name, defender.Name);

            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);
            DiceResult attackResult = attackDice.Roll();

            foreach (TermResult termResult in attackResult.Results)
            {
                attackMessage.Append(termResult.Value + ", ");
                if (termResult.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }
            }

            attackMessage.AppendFormat(" scoring {0} hits.", hits);
            return hits;
        }

        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;

            if (hits > 0)
            {
                defenseMessage.AppendFormat(" {0} defends and rolls: ", defender.Name);

                DiceExpression defenseDice = new DiceExpression().Dice(defender.Defense, 100);
                DiceResult defenseRoll = defenseDice.Roll();

                foreach (TermResult termResult in defenseRoll.Results)
                {
                    defenseMessage.Append(termResult.Value + ", ");
                    if (termResult.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }

                defenseMessage.AppendFormat(" resulting in {0} blocks.", blocks);
            }
            else
            {
                attackMessage.Append(" and misses completely.");
            }

            return blocks;
        }

        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                defender.Health -= damage;
                Game.MessageLog.Add($"{defender.Name} was hit for {damage} damage!");

                if (defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                Game.MessageLog.Add($"{defender.Name} blocked all damage!");
            }
        }

        private static void ResolveDeath(Actor defender)
        {
            if (defender is Player)
            {
                Game.MessageLog.Add($"{defender.Name} was killed. YOU HAVE TO TRY AGAIN, TUCKER!");
            }
            else if (defender is Droid droid)
            {
                Game.WastelandMap.RemoveDroid(droid);
                Game.MessageLog.Add($"{defender.Name} died and dropped {droid.EnergyCell} energy cells!");
            }
        }

        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
        }

        public void ActivateDroids()
        {
            IScheduleable scheduleable = Game.SchedulingSystem.Get();

            if (scheduleable is Player)
            {
                IsPlayerTurn = true;
                Game.SchedulingSystem.Add(Game.Player);
            }
            else if (scheduleable is Droid droid)
            {
                droid.PerformAction(this);
                Game.SchedulingSystem.Add(droid);
                ActivateDroids();
            }
        }

        public void MoveDroid(Droid droid, ICell targetCell)
        {
            if (targetCell == null)
                return;

            if (!Game.WastelandMap.IsWalkable(targetCell.X, targetCell.Y))
                return;

            Game.WastelandMap.SetIsWalkable(droid.X, droid.Y, true);

            droid.X = targetCell.X;
            droid.Y = targetCell.Y;

            Game.WastelandMap.SetIsWalkable(droid.X, droid.Y, false);
        }
    }
}