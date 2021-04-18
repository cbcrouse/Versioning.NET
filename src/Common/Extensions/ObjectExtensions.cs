using System.Reflection;

namespace Common.Extensions
{
    /// <summary>
    /// Provides extended functionality for <see cref="object"/>s.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the value of an object's property.
        /// </summary>
        /// <param name="o">The object to get the property value from.</param>
        /// <param name="propertyName">The name of the property on the object.</param>
        public static object GetPropertyValue(this object o, string propertyName)
        {
            object objValue = string.Empty;

            PropertyInfo? propertyInfo = o.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
                objValue = propertyInfo.GetValue(o, null) ?? string.Empty;

            return objValue;
        }
    }
}
