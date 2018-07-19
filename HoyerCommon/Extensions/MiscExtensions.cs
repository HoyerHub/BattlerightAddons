using System.Linq;
using BattleRight.Core.Math;

namespace Hoyer.Common.Extensions
{
    public static class MiscExtensions
    {
        public static bool Includes(this int[] array, int target)
        {
            return array.Any(o => o == target);
        }
        public static bool Includes(this string[] array, string target)
        {
            return array.Any(o => o == target);
        }
        public static bool Includes(this float[] array, float target)
        {
            return array.Any(o => o == target);
        }
        public static bool Includes(this Vector2[] array, Vector2 target)
        {
            return array.Any(o => o == target);
        }
    }
}