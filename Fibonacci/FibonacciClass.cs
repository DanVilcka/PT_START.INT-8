using System;
using System.Collections.Concurrent;

namespace Fibonacci
{
    public class FibonacciClass
    {
        /// <summary>
        /// Функиция для вычисления следующего значения. Я решил написать вычисление через заполнение памяти, что даёт возможность моментального вычисления, однако следующим шагом улучшения моей программы, я бы выбрал кэширование значений вычислений.
        /// </summary>
        /// <param name="current"></param> - передается значение.
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static FibonnaciValue CalculateNext(FibonnaciValue current)
        {
            int n = current.N + 1;
            if (n == 0) return new FibonnaciValue(current.Id, 0, 0);
            int[] dp = new int[n + 1];

            //base cases
            dp[0] = 0;
            dp[1] = 1;

            for (int i = 2; i <= n; i++)
            {

                dp[i] = dp[i - 1] + dp[i - 2];
                if (dp[i] < 0)
                {
                    throw new Exception($"Fibonacci {i}th results an invalid calculation because it is too long to be stored in an integer variable");
                }
            }

            return new FibonnaciValue(current.Id, n, dp[n]);;
        }
    }
}