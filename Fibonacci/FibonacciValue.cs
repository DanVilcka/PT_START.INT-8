using System;

namespace Fibonacci
{
    /// <summary>
    /// Опредение свойств числа.
    /// </summary>
    /// <param name="Id"></param> - для сравнения запроса и результата.
    /// <param name="N"></param> - номер в последовательности.
    /// <param name="Value"></param> - значение числа.
    public record FibonnaciValue(Guid Id, int N, int Value);
}