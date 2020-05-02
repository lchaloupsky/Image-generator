using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Helpers
{
    /// <summary>
    /// Class for helping with enum operation
    /// </summary>
    class EnumHelper
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
