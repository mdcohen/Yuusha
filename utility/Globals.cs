using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuusha
{
    public static class Globals
    {
        // Control unique names.
        public static string GAMEINPUTTEXTBOX = "GameInputTextBox";
        public static string GAMESCROLLABLETEXTBOX = "GameTextScrollableTextBox";
        public static string CONFINPUTTEXTBOX = "ConfInputTextBox";
        public static string CONFSCROLLABLETEXTBOX = "ConfScrollableTextBox";

        public const int MAX_EXP_LEVEL = 40;
        public const short EXP_LEVEL_3 = 1600;
        public const long EXP_LEVEL_20 = 209715200;

        public static int GetExpLevel(long exp)
        {
            if (exp > EXP_LEVEL_20) return GetExpLevelPostLevel20(exp);

            long low = EXP_LEVEL_3;
            long high = EXP_LEVEL_3 * 2;

            for (int a = 3; a <= MAX_EXP_LEVEL; a++)
            {
                if (exp >= low && exp < high)
                    return a;

                low = high;

                high = high * 2;
            }
            return 3;
        }

        public static long GetExperienceRequiredForLevel(int level)
        {
            long experience = EXP_LEVEL_3;

            for (int a = 3, b = 0, c = 1; a <= MAX_EXP_LEVEL; a++)
            {
                if (a == level)
                    return experience;

                if (a <= 20)
                {
                    experience = experience * 2;
                }
                else
                {
                    experience = experience + (EXP_LEVEL_20 * c);
                    b++;

                    if (b == 2)
                    {
                        c++;
                        b = 0;
                    }
                }
            }

            return experience;
        }

        public static int GetExpLevelPostLevel20(long exp)
        {
            long experienceCurve = EXP_LEVEL_20;
            long low = experienceCurve;
            long high = experienceCurve * 2;

            for (int a = 20, b = 0, c = 1; a <= MAX_EXP_LEVEL; a++)
            {
                if (b == 2)
                {
                    c++;
                    b = 0;
                }
                experienceCurve = EXP_LEVEL_20 * c;
                b++;

                if (exp >= low && exp < high)
                    return a;

                low = high;

                high = high + experienceCurve;
            }
            return 20;
        }
    }
}
