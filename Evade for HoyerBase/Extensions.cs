using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using Hoyer.Base;
using Hoyer.Base.Data.Abilites;

namespace Hoyer.Evade
{
    public static class Extensions
    {
        public static Vector2 GetJumpPos(this DodgeAbilityInfo data)
        {
            if (MenuHandler.JumpMode == 1)
            {
                return GetBestJumpPosition(data.Range, (int) (data.Range*5));
            }

            return Main.MouseWorldPos;
        }

        private static Vector2 GetBestJumpPosition(float range, int pointsToConsider)
        {
            var allies = EntitiesManager.LocalTeam.Where(x => !x.IsLocalPlayer && !x.Living.IsDead);
            var characters = allies as Character[] ?? allies.ToArray();
            var alliesInRange = characters
                .Where(x => x.Distance(LocalPlayer.Instance) <= range)
                .OrderByDescending(x => x.Distance(LocalPlayer.Instance))
                .ToArray();
            var alliesNotInRange = characters
                .Except(alliesInRange)
                .OrderBy(x => x.Distance(LocalPlayer.Instance));

            
                    foreach (var ally in alliesInRange)
                    {
                        if (EntitiesManager.EnemyTeam.Count(e=>!e.Living.IsDead && e.Distance(ally) < 4.5f) == 0)
                        {
                            return ally.MapObject.Position;
                        }
                    }

                    foreach (var ally in alliesNotInRange)
                    {
                        if (Math.Abs(LocalPlayer.Instance.Distance(ally) - range) <= 4.5f)
                        {
                            if (EntitiesManager.EnemyTeam.Count(e => !e.Living.IsDead && e.Distance(ally) < 4.5f) == 0)
                            {
                                return ally.MapObject.Position;
                            }
                        }
                    }

                    //No directly safe ally, lets find spots in our circumference
                    var possibleSafeSpots = new List<Vector2>();

                    var sectorAngle = 2 * Math.PI / pointsToConsider;
                    for (var i = 0; i < pointsToConsider; i++)
                    {
                        var angleIteration = sectorAngle * i;

                        var point = new Vector2
                        {
                            X = (int)(LocalPlayer.Instance.MapObject.Position.X + range * Math.Cos(angleIteration)),
                            Y = (int)(LocalPlayer.Instance.MapObject.Position.Y + range * Math.Sin(angleIteration))
                        };

                        possibleSafeSpots.Add(point);
                    }

                    //No ally is safe, let's just find a safe spot in the circumference that is closest to the ally who in turn is closest to our jump distance

                    var orderedByEdgeDistance = characters.OrderBy(x => Math.Abs(LocalPlayer.Instance.Distance(x) - range));

                    foreach (var ally in orderedByEdgeDistance)
                    {
                        var orderedPoints = possibleSafeSpots.OrderBy(x => x.Distance(ally));
                        foreach (var point in orderedPoints)
                        {
                            if (EntitiesManager.EnemyTeam.Count(e => !e.Living.IsDead && e.Distance(ally) < 4.5f) == 0)
                            {
                                return point;
                            }
                        }
                    }

            //If all else fails, just return mousepos
            return Main.MouseWorldPos;
        }
    }
}