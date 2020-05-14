using System;

namespace ImagePositioner.Helpers
{
    /// <summary>
    /// Class for helping with enum operation
    /// </summary>
    internal class EnumHelper
    {
        /// <summary>
        /// Gets random enum value
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Random enum value</returns>
        public T GetRandomEnum<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(new Random().Next(values.Length));
        }
    }
}
