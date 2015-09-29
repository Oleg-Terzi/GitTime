using System;

namespace GitTime.Web.Infrastructure.Helpers
{
    public static class NumberHelper
    {
        #region Constants

        public const Decimal DecimalNotKnown = Decimal.MinValue;
        public const Double DoubleNotKnown = Double.MinValue;
        public const Int16 Int16NotKnown = Int16.MinValue;
        public const Int32 Int32NotKnown = Int32.MinValue;
        public const Int64 Int64NotKnown = Int64.MinValue;
        public const Single SingleNotKnown = Single.MinValue;

        public const Decimal DecimalNotAvailable = Decimal.MinValue + 1;
        public const Double DoubleNotAvailable = Double.MinValue + 1;
        public const Int16 Int16NotAvailable = Int16.MinValue + 1;
        public const Int32 Int32NotAvailable = Int32.MinValue + 1;
        public const Int64 Int64NotAvailable = Int64.MinValue + 1;
        public const Single SingleNotAvailable = Single.MinValue + 1;

        public const Decimal DecimalNotApplicable = Decimal.MinValue + 2;
        public const Double DoubleNotApplicable = Double.MinValue + 2;
        public const Int16 Int16NotApplicable = Int16.MinValue + 2;
        public const Int32 Int32NotApplicable = Int32.MinValue + 2;
        public const Int64 Int64NotApplicable = Int64.MinValue + 2;
        public const Single SingleNotApplicable = Single.MinValue + 2;

        #endregion

        #region Division

        /// <summary>
        ///     Returns zero if the denominator is zero.
        /// </summary>
        public static Double Divide(Double numerator, Double denominator)
        {
            Double x = numerator;
            Double y = denominator;

            if (Math.Abs(y - 0) > Double.Epsilon)
                return x/y;

            return 0;
        }

        #endregion

        #region Average

        public static Double Average(Int32[] numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException("numbers");

            if (numbers.Length == 0)
                throw new ArgumentException("Cannot calculate the average for an empty array");

            Double sum = 0.0;
            foreach (Int32 number in numbers)
                sum += number;

            return sum/Convert.ToDouble(numbers.Length);
        }

        public static Double Average(Double[] numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException("numbers");

            if (numbers.Length == 0)
                throw new ArgumentException("Cannot calculate the average for an empty array");

            Double sum = 0.0;
            foreach (Double number in numbers)
                sum += number;

            return sum/Convert.ToDouble(numbers.Length);
        }

        #endregion

        #region Standard deviation

        public static Double StandardDeviation(Double[] numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException("numbers");

            if (numbers.Length == 0)
                throw new ArgumentException("Cannot calculate the standard deviation for an empty array");

            if (numbers.Length == 1)
                throw new ArgumentException("Cannot calculate the standard deviation for an array with only one element");

            Double sum = 0.0;
            Double sumOfSquares = 0.0;
            foreach (Double number in numbers)
            {
                sum += number;
                sumOfSquares += Math.Pow(number, 2);
            }
            Double topSum = (numbers.Length*sumOfSquares) - (Math.Pow(sum, 2));
            Double n = numbers.Length;
            return Math.Sqrt(topSum/(n*(n - 1)));
        }

        public static Double StandardDeviation(Double[,] numbers, Int32 columnIndex)
        {
            if (numbers == null)
                throw new ArgumentNullException("numbers");

            Int32 length = numbers.GetLength(0);
            var row = new Double[length];
            for (Int32 i = 0; i < length; i++)
                row[i] = numbers[i, columnIndex];

            return StandardDeviation(row);
        }

        #endregion

        #region Correlation

        public static Double Correlation(Double[] array1, Double[] array2)
        {
            if (array1 == null)
                throw new ArgumentNullException("array1");

            if (array2 == null)
                throw new ArgumentNullException("array2");

            if (array1.Length == 0 || array2.Length == 0)
                throw new ArgumentException("Cannot calculate the correlation for an empty array");

            if (array1.Length == 1 || array2.Length == 0)
                throw new ArgumentException("Cannot calculate the correlation for an array with only one element");

            if (array1.Length != array2.Length)
                throw new ArgumentException("Cannot calculate the correlation for arrays with different lengths");

            Double denominator = (array1.Length - 1)*StandardDeviation(array1)*StandardDeviation(array2);
            Double sumxy = 0;
            for (Int32 i = 0; i < array1.Length; i++)
            {
                sumxy += array1[i]*array2[i];
            }
            Double numerator = sumxy - array1.Length*Average(array1)*Average(array2);
            return numerator/denominator;
        }

        #endregion
    }
}