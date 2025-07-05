namespace GameServerManager.BAL
{
    /// <summary>
    /// Attribute to map a property to an INI setting name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IniSettingAttribute : Attribute
    {
        public string IniName { get; }
        public bool IsStaticValue { get; }
        public string StaticValue { get; }

        /// <summary>
        /// Maps a property to an INI setting name
        /// </summary>
        /// <param name="iniName">The name of the setting in the INI file</param>
        public IniSettingAttribute(string iniName)
        {
            IniName = iniName;
            IsStaticValue = false;
            StaticValue = string.Empty;
        }

        /// <summary>
        /// Creates a static INI setting that doesn't map to a property
        /// </summary>
        /// <param name="iniName">The name of the setting in the INI file</param>
        /// <param name="staticValue">The static value to use</param>
        public IniSettingAttribute(string iniName, string staticValue)
        {
            IniName = iniName;
            IsStaticValue = true;
            StaticValue = staticValue;
        }
    }
}
