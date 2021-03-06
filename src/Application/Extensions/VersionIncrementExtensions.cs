using Domain.Enumerations;
using System;

namespace Application.Extensions
{
    /// <summary>
    /// Provides extended functionality for <see cref="VersionIncrement"/>.
    /// </summary>
    public static class VersionIncrementExtensions
    {
        /// <summary>
        /// Lowers the increment. (e.g. Major -> Minor)
        /// </summary>
        /// <param name="increment">Represents various version increments based on semver.org.</param>
        public static VersionIncrement Lower(this VersionIncrement increment)
        {
            switch (increment)
            {
                case VersionIncrement.Unknown:
                    increment = VersionIncrement.None;
                    break;
                case VersionIncrement.None:
                case VersionIncrement.Patch:
                    break;
                case VersionIncrement.Minor:
                    increment = VersionIncrement.Patch;
                    break;
                case VersionIncrement.Major:
                    increment = VersionIncrement.Minor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(increment), increment, null);
            }

            return increment;
        }
    }
}
