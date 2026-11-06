using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    // Streak Data from the Original Game:
    // 2 card
        // Round 2: +4
        // Round 3: +6
        // Round 4: +7
        // Round 6: +10
        // Round 7: +12
        // Round 9: +15
        // Round 12: +19
        // Round 13: +21
    // 3 card
        // Round 3: +13
        // Round 4: +17
        // Round 5: +20
        // Round 7: +27
        // Round 8: +31
        // Round 10: +37
        // Round 13: +48
    // 4 card
        // Round 4: +29
        // Round 6: +41
        // Round 9: +59
    // 5 card
        // Round 5: +55
        // Round 10: +102
        
        // Points go up by 5 each round, starting at 10 points in round 1
        public static int GetBasePoints(int round)
        {
            return 5 * (round + 1);
        }

        public static int GetStreakPoints(int round, int streak)
        {
            if (streak < 2)
            {
                return 0;
            }
            // Generate the multiplier by taking 1.3 ^ (streak - 1) and then subtracting 1 again.
            // For the final bonus points, we multiply the multiplier times the base points for the given round
            double multiplier = System.Math.Pow(1.3, streak - 1) - 1.0;
            // Adding the tiny epsilon stops floating point errors from converting 6.0 into 5.9999 or whatever
            return (int)System.Math.Floor(GetBasePoints(round) * multiplier + 1e-6);
        }

        public static int GetTotalPoints(int round, int streak)
        {
            return GetBasePoints(round) + GetStreakPoints(round, streak);
        }
        
}
