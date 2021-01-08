namespace Domain.Enumerations
{
    /// <summary>
    /// Represents various version increments based on semver.org.
    /// </summary>
    public enum VersionIncrement
    {
        /// <summary>
        /// Indicates a version increment could not be determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates the version will not be incremented.
        /// </summary>
        None,

        /// <summary>
        /// Indicates the version increment is Patch.
        /// </summary>
        Patch,

        /// <summary>
        /// Indicates the version increment is Minor.
        /// </summary>
        Minor,

        /// <summary>
        /// Indicates the version increment is Major.
        /// </summary>
        Major

    }
}
