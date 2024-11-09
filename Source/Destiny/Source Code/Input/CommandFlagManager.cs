using System.Collections.Generic;

namespace Destiny
{
    /// <summary>
    /// Command flag manager class.
    /// </summary>
    public class CommandFlagManager
    {
        private Dictionary<string, bool> m_Flags = new Dictionary<string, bool>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public CommandFlagManager(CommandManager p_CommandManager)
        {
            p_CommandManager.RegisterCommand("flags", this.ShowAllFlags);
            p_CommandManager.RegisterCommand("flag", this.SetFlagValue);
        }

        /// <summary>
        /// Register a new flag.
        /// </summary>
        public void RegisterFlag(string p_Name, bool p_DefaultValue)
        {
            if (!m_Flags.ContainsKey(p_Name))
            {
                m_Flags[p_Name] = p_DefaultValue;
            }
        }

        /// <summary>
        /// Unregister a flag.
        /// </summary>
        public void UnregisterFlag(string p_Name)
        {
            m_Flags.Remove(p_Name);
        }

        /// <summary>
        /// Get flag value.
        /// </summary>
        public bool Flag(string p_Name)
        {
            bool result = false;
            if (m_Flags.ContainsKey(p_Name))
            {
                result = m_Flags[p_Name];
            }
            return result;
        }

        /// <summary>
        /// Show all flags.
        /// </summary>
        /// <param name="p_Arguments">Arguments.</param>
        /// <param name="p_CommandResult">Command result.</param>
        private void ShowAllFlags(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            int i = 0;
            foreach (string flagName in m_Flags.Keys)
            {
                i++;
                p_CommandResult.AddOutputLine(string.Format(
                    "{0}: {1} ({2})",
                    i,
                    flagName,
                    m_Flags[flagName] ? "true" : "false"
                    ));
            }
        }

        /// <summary>
        /// Set a flag value.
        /// </summary>
        /// <param name="p_Arguments">Arguments.</param>
        /// <param name="p_CommandResult">Command result.</param>
        private void SetFlagValue(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            if (p_Arguments == null || p_Arguments.Length < 2)
            {
                p_CommandResult.AddOutputLine("Usage: 'flag <flag_number> <flag_value>'");
            }
            else
            {
                int flagNumber = 0;
                IntHelper.TryParse(p_Arguments[0], out flagNumber);
                if (flagNumber == 0 || flagNumber > m_Flags.Count)
                {
                    p_CommandResult.AddOutputLine(string.Format(
                        "'{0}' is not a valid flag number",
                        p_Arguments[0]
                        ));
                }
                else
                {
                    // Consider anything other than 0 to be true
                    int flagValue = 1;
                    IntHelper.TryParse(p_Arguments[1], out flagValue);

                    int i = 0;
                    foreach (string flagName in m_Flags.Keys)
                    {
                        i++;
                        if (i == flagNumber)
                        {
                            m_Flags[flagName] = (flagValue != 0);
                            break;
                        }
                    }
                }
            }
        }
    }
}
