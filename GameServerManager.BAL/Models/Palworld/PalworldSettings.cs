using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace GameServerManager.BAL
{
    public class PalworldSettings
    {
        // Server Settings
        [IniSetting("ServerName")]
        public string ServerName { get; set; } = "Default Palworld Server";
        
        [IniSetting("ServerDescription")]
        public string ServerDescription { get; set; } = "";
        
        [IniSetting("AdminPassword")]
        public string AdminPassword { get; set; } = "";
        
        [IniSetting("ServerPassword")]
        public string ServerPassword { get; set; } = "";
        
        [IniSetting("PublicIP")]
        public string PublicIP { get; set; } = "";
        
        [IniSetting("PublicPort")]
        public int PublicPort { get; set; } = 8211;
        
        [IniSetting("ServerPlayerMaxNum")]
        public int ServerPlayerMaxNum { get; set; } = 32;
        
        [IniSetting("RCONEnabled")]
        public bool RCONEnabled { get; set; } = false;
        
        [IniSetting("RCONPort")]
        public int RCONPort { get; set; } = 25575;
        
        [IniSetting("RESTAPIEnabled")]
        public bool RESTAPIEnabled { get; set; } = false;
        
        [IniSetting("RESTAPIPort")]
        public int RESTAPIPort { get; set; } = 8212;
        
        [IniSetting("Region")]
        public string Region { get; set; } = "";
        
        [IniSetting("bUseAuth")]
        public bool UseAuth { get; set; } = true;
        
        [IniSetting("BanListURL")]
        public string BanListURL { get; set; } = "https://api.palworldgame.com/api/banlist.txt";

        // In-Game Settings
        [IniSetting("bEnablePlayerToPlayerDamage")]
        public bool EnablePlayerToPlayerDamage { get; set; } = true;
        
        [IniSetting("PalEggDefaultHatchingTime")]
        public double PalEggDefaultHatchingTime { get; set; } = 72.0;
        
        [IniSetting("DropItemMaxNum")]
        public int DropItemMaxNum { get; set; } = 3000;
        
        [IniSetting("BaseCampMaxNum")]
        public int BaseCampMaxNum { get; set; } = 128;
        
        [IniSetting("DropItemAliveMaxHours")]
        public double DropItemAliveMaxHours { get; set; } = 1.0;
        
        [IniSetting("bAutoResetGuildNoOnlinePlayers")]
        public bool AutoResetGuildNoOnlinePlayers { get; set; } = false;
        
        [IniSetting("AutoResetGuildTimeNoOnlinePlayers")]
        public double AutoResetGuildTimeNoOnlinePlayers { get; set; } = 72.0;
        
        [IniSetting("WorkSpeedRate")]
        public double WorkSpeedRate { get; set; } = 1.0;
        
        [IniSetting("bIsMultiplay")]
        public bool IsMultiplay { get; set; } = false;
        
        [IniSetting("bIsPvP")]
        public bool IsPvP { get; set; } = false;
        
        [IniSetting("bCanPickupOtherGuildDeathPenaltyDrop")]
        public bool CanPickupOtherGuildDeathPenaltyDrop { get; set; } = false;
        
        [IniSetting("bEnableNonLoginPenalty")]
        public bool EnableNonLoginPenalty { get; set; } = true;
        
        [IniSetting("bEnableFastTravel")]
        public bool EnableFastTravel { get; set; } = true;
        
        [IniSetting("bIsStartLocationSelectByMap")]
        public bool IsStartLocationSelectByMap { get; set; } = true;
        
        [IniSetting("bExistPlayerAfterLogout")]
        public bool ExistPlayerAfterLogout { get; set; } = false;
        
        [IniSetting("bEnableDefenseOtherGuildPlayer")]
        public bool EnableDefenseOtherGuildPlayer { get; set; } = false;
        
        [IniSetting("bInvisibleOtherGuildBaseCampAreaFX")]
        public bool InvisibleOtherGuildBaseCampAreaFX { get; set; } = false;
        
        [IniSetting("bBuildAreaLimit")]
        public bool BuildAreaLimit { get; set; } = false;
        
        [IniSetting("CoopPlayerMaxNum")]
        public int CoopPlayerMaxNum { get; set; } = 4;
        
        [IniSetting("PalSyncDistanceFromPlayer")]
        public int PalSyncDistanceFromPlayer { get; set; } = 15000;
        
        [IniSetting("bShowPlayerList")]
        public bool EnableOnlinePlayerListInDedicatedServer { get; set; } = false;
        
        [IniSetting("bAllowGlobalPalboxExport")]
        public bool AllowGlobalPalboxExport { get; set; } = true;
        
        [IniSetting("bAllowGlobalPalboxImport")]
        public bool AllowGlobalPalboxImport { get; set; } = false;

        // Game Balance Settings
        [IniSetting("DayTimeSpeedRate")]
        public double DayTimeSpeedRate { get; set; } = 1.0;
        
        [IniSetting("NightTimeSpeedRate")]
        public double NightTimeSpeedRate { get; set; } = 1.0;
        
        [IniSetting("ExpRate")]
        public double ExpRate { get; set; } = 1.0;
        
        [IniSetting("PalCaptureRate")]
        public double PalCaptureRate { get; set; } = 1.0;
        
        [IniSetting("PalSpawnNumRate")]
        public double PalSpawnNumRate { get; set; } = 1.0;
        
        [IniSetting("PalDamageRateAttack")]
        public double PalDamageRateAttack { get; set; } = 1.0;
        
        [IniSetting("PalDamageRateDefense")]
        public double PalDamageRateDefense { get; set; } = 1.0;
        
        [IniSetting("PalStomachDecreaceRate")]
        public double PalStomachDecreaseRate { get; set; } = 1.0;
        
        [IniSetting("PalStaminaDecreaceRate")]
        public double PalStaminaDecreaseRate { get; set; } = 1.0;
        
        [IniSetting("PalAutoHPRegeneRate")]
        public double PalAutoHPRegenRate { get; set; } = 1.0;
        
        [IniSetting("PalAutoHpRegeneRateInSleep")]
        public double PalAutoHPRegenRateInSleep { get; set; } = 1.0;
        
        [IniSetting("PlayerDamageRateAttack")]
        public double PlayerDamageRateAttack { get; set; } = 1.0;
        
        [IniSetting("PlayerDamageRateDefense")]
        public double PlayerDamageRateDefense { get; set; } = 1.0;
        
        [IniSetting("PlayerStomachDecreaceRate")]
        public double PlayerStomachDecreaseRate { get; set; } = 1.0;
        
        [IniSetting("PlayerStaminaDecreaceRate")]
        public double PlayerStaminaDecreaseRate { get; set; } = 1.0;
        
        [IniSetting("PlayerAutoHPRegeneRate")]
        public double PlayerAutoHPRegenRate { get; set; } = 1.0;
        
        [IniSetting("PlayerAutoHpRegeneRateInSleep")]
        public double PlayerAutoHPRegenRateInSleep { get; set; } = 1.0;
        
        [IniSetting("BuildObjectHpRate")]
        public double BuildObjectHPRate { get; set; } = 1.0;
        
        [IniSetting("BuildObjectDamageRate")]
        public double BuildObjectDamageRate { get; set; } = 1.0;
        
        [IniSetting("BuildObjectDeteriorationDamageRate")]
        public double BuildObjectDeteriorationDamageRate { get; set; } = 1.0;
        
        [IniSetting("CollectionDropRate")]
        public double CollectionDropRate { get; set; } = 1.0;
        
        [IniSetting("CollectionObjectHpRate")]
        public double CollectionObjectHpRate { get; set; } = 1.0;
        
        [IniSetting("CollectionObjectRespawnSpeedRate")]
        public double CollectionObjectRespawnSpeedRate { get; set; } = 1.0;
        
        [IniSetting("EnemyDropItemRate")]
        public double EnemyDropItemRate { get; set; } = 1.0;
        
        [IniSetting("ItemWeightRate")]
        public double ItemWeightRate { get; set; } = 1.0;

        // Advanced Settings
        [IniSetting("bHardcore")]
        public bool HardcoreMode { get; set; } = false;
        
        [IniSetting("bPalLost")]
        public bool PalLostMode { get; set; } = false;
        
        [IniSetting("bCharacterRecreateInHardcore")]
        public bool AllowCharacterRecreateInHardcore { get; set; } = false;
        
        [IniSetting("DeathPenalty")]
        public string DeathPenalty { get; set; } = "All";
        
        [IniSetting("bEnableInvaderEnemy")]
        public bool EnableInvaderEnemy { get; set; } = true;
        
        [IniSetting("EnablePredatorBossPal")]
        public bool EnablePredatorBossPal { get; set; } = false;
        
        [IniSetting("GuildPlayerMaxNum")]
        public int GuildPlayerMaxNum { get; set; } = 20;
        
        [IniSetting("BaseCampMaxNumInGuild")]
        public int BaseCampMaxNumInGuild { get; set; } = 3;
        
        [IniSetting("BaseCampWorkerMaxNum")]
        public int BaseCampWorkerMaxNum { get; set; } = 15;
        
        [IniSetting("MaxBuildingLimitNum")]
        public int MaxBuildingLimitPerPlayer { get; set; } = 0;
        
        [IniSetting("SupplyDropSpan")]
        public int SupplyDropSpan { get; set; } = 180;
        
        [IniSetting("ChatPostLimitPerMinute")]
        public int ChatPostLimitPerMinute { get; set; } = 10;
        
        [IniSetting("EquipmentDurabilityDamageRate")]
        public double EquipmentDurabilityDamageRate { get; set; } = 1.0;
        
        [IniSetting("ItemContainerForceMarkDirtyInterval")]
        public double ItemContainerForceDirtyInterval { get; set; } = 1.0;
        
        [IniSetting("AutoSaveSpan")]
        public double AutoSaveSpan { get; set; } = 30.0;
        
        [IniSetting("ServerReplicatePawnCullDistance")]
        public double ServerReplicatePawnCullDistance { get; set; } = 15000.0;

        // Platform Settings
        [IniSetting("LogFormatType")]
        public string LogFormatType { get; set; } = "Text";
        
        [IniSetting("AllowConnectPlatform")]
        public string AllowConnectPlatform { get; set; } = "Steam";
        
        [IniSetting("CrossplayPlatforms")]
        public string CrossplayPlatforms { get; set; } = "(Steam,Xbox,PS5,Mac)";

        // Static settings that don't map to properties
        private static readonly Dictionary<string, string> StaticSettings = new()
        {
            { "Difficulty", "None" },
            { "RandomizerType", "None" },
            { "RandomizerSeed", "\"\"" },
            { "bIsRandomizerPalLevelRandom", "false" },
            { "bEnableFriendlyFire", "false" },
            { "bActiveUNKO", "false" },
            { "bEnableAimAssistPad", "true" },
            { "bEnableAimAssistKeyboard", "false" },
            { "DropItemMaxNum_UNKO", "100" },
            { "bUseBanList", "true" },
            { "bIsUseBackupSaveData", "true" }
        };

        /// <summary>
        /// Loads settings from a PalWorldSettings.ini file content
        /// </summary>
        public static PalworldSettings LoadFromIniContent(string iniContent)
        {
            var settings = new PalworldSettings();
            
            // Find the OptionSettings line and extract the parameters using proper parentheses matching
            var optionsContent = ExtractOptionSettingsContent(iniContent);
            if (string.IsNullOrEmpty(optionsContent))
                return settings;

            var parameters = ParseParameters(optionsContent);

            // Map parameters to properties
            foreach (var param in parameters)
            {
                MapParameterToProperty(settings, param.Key, param.Value);
            }

            return settings;
        }
        
        /// <summary>
        /// Extracts the content between OptionSettings=( and the matching closing parenthesis
        /// </summary>
        private static string ExtractOptionSettingsContent(string iniContent)
        {
            const string searchPattern = "OptionSettings=(";
            var startIndex = iniContent.IndexOf(searchPattern);
            if (startIndex == -1)
                return string.Empty;
            
            // Start after "OptionSettings=("
            var contentStart = startIndex + searchPattern.Length;
            var position = contentStart;
            var parenDepth = 1; // We're already inside the first opening parenthesis
            var inQuotes = false;
            
            while (position < iniContent.Length && parenDepth > 0)
            {
                var currentChar = iniContent[position];
                
                if (currentChar == '"' && (position == 0 || iniContent[position - 1] != '\\'))
                {
                    inQuotes = !inQuotes;
                }
                else if (!inQuotes)
                {
                    if (currentChar == '(')
                    {
                        parenDepth++;
                    }
                    else if (currentChar == ')')
                    {
                        parenDepth--;
                    }
                }
                
                position++;
            }
            
            if (parenDepth == 0)
            {
                // Found the matching closing parenthesis
                return iniContent.Substring(contentStart, position - contentStart - 1);
            }
            
            // No matching closing parenthesis found
            return string.Empty;
        }

        /// <summary>
        /// Loads settings from a PalWorldSettings.ini file
        /// </summary>
        public static async Task<PalworldSettings> LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return new PalworldSettings();

            var content = await File.ReadAllTextAsync(filePath);
            return LoadFromIniContent(content);
        }

        /// <summary>
        /// Saves settings to a PalWorldSettings.ini file
        /// </summary>
        public async Task SaveToFileAsync(string filePath)
        {
            var iniContent = GenerateIniContent();
            await File.WriteAllTextAsync(filePath, iniContent);
        }

        /// <summary>
        /// Generates the complete INI file content using reflection
        /// The format matches exactly what Palworld expects, with 6 decimal places for floating point values
        /// </summary>
        public string GenerateIniContent()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[/Script/Pal.PalGameWorldSettings]");
            sb.Append("OptionSettings=(");
            
            var parameters = new List<string>();
            
            // Add static settings first
            foreach (var staticSetting in StaticSettings)
            {
                parameters.Add($"{staticSetting.Key}={staticSetting.Value}");
            }
            
            // Use reflection to get all properties with IniSetting attributes
            var properties = GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<IniSettingAttribute>() != null)
                .ToList();
            
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<IniSettingAttribute>();
                if (attribute == null) continue;
                
                var value = property.GetValue(this);
                var formattedValue = FormatValueForIni(value, property.PropertyType);
                
                parameters.Add($"{attribute.IniName}={formattedValue}");
            }

            sb.Append(string.Join(",", parameters));
            sb.AppendLine(")");

            return sb.ToString();
        }
        
        /// <summary>
        /// Formats a value for INI output based on its type
        /// </summary>
        private static string FormatValueForIni(object? value, Type propertyType)
        {
            if (value == null) return "";
            
            return propertyType switch
            {
                Type t when t == typeof(bool) => value.ToString()!.ToLower(),
                Type t when t == typeof(double) || t == typeof(float) => ((double)value).ToString("F6"),
                Type t when t == typeof(int) => value.ToString()!,
                Type t when t == typeof(string) => FormatStringValueForIni(value.ToString()!),
                _ => value.ToString()!
            };
        }
        
        /// <summary>
        /// Formats a string value for INI output, handling special cases like CrossplayPlatforms
        /// </summary>
        private static string FormatStringValueForIni(string value)
        {
            // If the value already contains parentheses (like CrossplayPlatforms), don't add quotes
            if (value.StartsWith("(") && value.EndsWith(")"))
            {
                return value;
            }
            
            // Otherwise, wrap in quotes
            return $"\"{value}\"";
        }

        private static Dictionary<string, string> ParseParameters(string optionsContent)
        {
            var parameters = new Dictionary<string, string>();
            var position = 0;
            
            while (position < optionsContent.Length)
            {
                // Skip whitespace
                while (position < optionsContent.Length && char.IsWhiteSpace(optionsContent[position]))
                    position++;
                
                if (position >= optionsContent.Length)
                    break;
                
                // Find the key (parameter name)
                var keyStart = position;
                while (position < optionsContent.Length && optionsContent[position] != '=')
                    position++;
                
                if (position >= optionsContent.Length)
                    break;
                
                var key = optionsContent.Substring(keyStart, position - keyStart).Trim();
                position++; // Skip the '=' character
                
                // Find the value, handling nested parentheses
                var valueStart = position;
                var parenDepth = 0;
                var inQuotes = false;
                
                while (position < optionsContent.Length)
                {
                    var currentChar = optionsContent[position];
                    
                    if (currentChar == '"' && (position == 0 || optionsContent[position - 1] != '\\'))
                    {
                        inQuotes = !inQuotes;
                    }
                    else if (!inQuotes)
                    {
                        if (currentChar == '(')
                        {
                            parenDepth++;
                        }
                        else if (currentChar == ')')
                        {
                            parenDepth--;
                        }
                        else if (currentChar == ',' && parenDepth == 0)
                        {
                            // Found the end of this parameter
                            break;
                        }
                    }
                    
                    position++;
                }
                
                var value = optionsContent.Substring(valueStart, position - valueStart).Trim();
                
                // Remove surrounding quotes if present
                if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 1)
                {
                    value = value.Substring(1, value.Length - 2);
                }
                
                parameters[key] = value;
                
                // Skip the comma if we're not at the end
                if (position < optionsContent.Length && optionsContent[position] == ',')
                    position++;
            }
            
            return parameters;
        }

        /// <summary>
        /// Maps an INI parameter to a property using reflection
        /// </summary>
        private static void MapParameterToProperty(PalworldSettings settings, string paramName, string value)
        {
            // Use reflection to find the property with the matching IniSetting attribute
            var properties = settings.GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<IniSettingAttribute>()?.IniName == paramName)
                .ToList();
            
            if (!properties.Any()) return;
            
            var property = properties.First();
            var convertedValue = ConvertValueFromIni(value, property.PropertyType);
            
            if (convertedValue != null)
            {
                property.SetValue(settings, convertedValue);
            }
        }
        
        /// <summary>
        /// Converts a string value from INI to the appropriate type
        /// </summary>
        private static object? ConvertValueFromIni(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value)) return null;
            
            try
            {
                return targetType switch
                {
                    Type t when t == typeof(bool) => value.ToLower() == "true",
                    Type t when t == typeof(int) => int.Parse(value),
                    Type t when t == typeof(double) => double.Parse(value),
                    Type t when t == typeof(float) => float.Parse(value),
                    Type t when t == typeof(string) => value,
                    _ => Convert.ChangeType(value, targetType)
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
