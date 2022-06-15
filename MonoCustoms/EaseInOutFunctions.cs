namespace MonoCustoms
{
    public static class EaseInOutFunctions
    {
        #region Sine
        public static double EaseInSine(double x)
        {
            return 1 - Math.Cos((x * Math.PI) / 2);
        }

        public static double EaseOutSine(double x)
        {
            return Math.Sin((x * Math.PI) / 2);
        }

        public static double EaseInOutSine(double x)
        {
            return -(Math.Cos(Math.PI * x) - 1) / 2;
        }
        #endregion

        #region Cubic
        public static double EaseInCubic(double x)
        {
            return x * x * x;
        }

        public static double EaseOutCubic(double x)
        {
            return 1 - Math.Pow(1 - x, 3);
        }

        public static double EaseInOutCubic(double x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2;
        }
        #endregion

        #region Quint
        public static double EaseInQuint(double x)
        {
            return x * x * x * x * x;
        }

        public static double EaseOutQuint(double x)
        {
            return 1 - Math.Pow(1 - x, 5);
        }

        public static double EaseInOutQuint(double x)
        {
            return x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2;
        }
        #endregion

        #region Circ
        public static double EaseInCirc(double x)
        {
            return 1 - Math.Sqrt(1 - Math.Pow(x, 2));
        }

        public static double EaseOutCirc(double x)
        {
            return Math.Sqrt(1 - Math.Pow(x - 1, 2));
        }

        public static double EaseInOutCirc(double x)
        {
            return x < 0.5
                ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2
                : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
        }
        #endregion

        #region Elastic
        public static double EaseInElastic(double x)
        {
            const double c4 = (2 * Math.PI) / 3;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * c4);
        }

        public static double EaseOutElastic(double x)
        {
            const double c4 = (2 * Math.PI) / 3;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1;
        }

        public static double EaseInOutElastic(double x)
        {
            const double c5 = (2 * Math.PI) / 4.5;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : x < 0.5
              ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2
              : (Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 + 1;
        }
        #endregion

        #region Quad
        public static double EaseInQuad(double x)
        {
            return x * x;
        }

        public static double EaseOutQuad(double x)
        {
            return 1 - (1 - x) * (1 - x);
        }

        public static double EaseInOutQuad(double x)
        {
            return x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2;
        }
        #endregion
        public static double EaseOutBack(double x)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1.0;
            return 1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2);
        }

        public static double EaseOutBounce(double x)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x -= 1.5 / d1) * x + 0.75;
            }
            else if (x < 2.5 / d1)
            {
                return n1 * (x -= 2.25 / d1) * x + 0.9375;
            }
            else
            {
                return n1 * (x -= 2.625 / d1) * x + 0.984375;
            }
        }
    }
}
