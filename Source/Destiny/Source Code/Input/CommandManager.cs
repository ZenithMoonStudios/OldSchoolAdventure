using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Destiny
{
    /// <summary>
    /// Command manager class.
    /// </summary>
    public class CommandManager
    {
        private CommandFlagManager m_FlagManager = null;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static CommandManager()
        {
            InitializeCommandKeys();
        }

        public string CurrentCommand { get; private set; }
        private CommandResultList m_History = new CommandResultList();
        public CommandResultList History { get { return m_History; } }

        /// <summary>
        /// Command handlers.
        /// </summary>
        public delegate void CommandHandler(string[] p_Arguments, ref CommandResult p_CommandResult);
        private Dictionary<string, CommandHandler> m_CommandNameToHandlers = new Dictionary<string, CommandHandler>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public CommandManager()
        {
            this.CurrentCommand = string.Empty;
            this.RegisterCommand("commands", this.ShowValidCommands);
            this.RegisterCommand("help", this.ShowValidCommands);
            m_FlagManager = new CommandFlagManager(this);
        }

        #region Command handlers

        /// <summary>
        /// Show all valid commands.
        /// </summary>
        /// <param name="p_Arguments">Arguments.</param>
        /// <param name="p_CommandResult">Command result.</param>
        private void ShowValidCommands(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            foreach (string commandName in m_CommandNameToHandlers.Keys)
            {
                p_CommandResult.AddOutputLine(commandName);
            }
        }

        #endregion

        /// <summary>
        /// Register a command.
        /// </summary>
        /// <param name="p_Name">Name of the command.</param>
        /// <param name="p_Handler">Command handler.</param>
        public void RegisterCommand(string p_Name, CommandHandler p_Handler)
        {
            m_CommandNameToHandlers[p_Name] = p_Handler;
        }

        /// <summary>
        /// Unregister a command.
        /// </summary>
        /// <param name="p_Name">Name of the command.</param>
        /// <param name="p_Handler">Command handler.</param>
        public void UnregisterCommand(string p_Name)
        {
            if (m_CommandNameToHandlers.ContainsKey(p_Name))
            {
                m_CommandNameToHandlers.Remove(p_Name);
            }
        }

        /// <summary>
        /// Register a flag.
        /// </summary>
        public void RegisterFlag(string p_Name, bool p_DefaultValue)
        {
            m_FlagManager.RegisterFlag(p_Name, p_DefaultValue);
        }

        /// <summary>
        /// Unregister a flag.
        /// </summary>
        public void UnregisterFlag(string p_Name)
        {
            m_FlagManager.UnregisterFlag(p_Name);
        }

        /// <summary>
        /// Get flag value.
        /// </summary>
        public bool Flag(string p_Name)
        {
            return m_FlagManager.Flag(p_Name);
        }

        /// <summary>
        /// Process the current command.
        /// </summary>
        public void ProcessCommand()
        {
            string command = this.CurrentCommand.Trim();
            if (command != string.Empty)
            {
                // Separate command into name and arguments
                string commandName;
                string[] commandArguments = null;
                int spaceIndex = command.IndexOf(' ');
                if (spaceIndex >= 0)
                {
                    commandName = command.Substring(0, spaceIndex);
                    commandArguments = command.Substring(spaceIndex + 1).Split(' ');
                }
                else
                {
                    commandName = command;
                }
                if (m_CommandNameToHandlers.ContainsKey(commandName.ToLower()))
                {
                    // Execute command
                    CommandResult commandResult = new CommandResult(this.CurrentCommand);
                    m_CommandNameToHandlers[commandName.ToLower()](commandArguments, ref commandResult);
                    m_History.Add(commandResult);
                }
                else
                {
                    // Command does not exist - output error to console
                    CommandResult commandResult = new CommandResult(
                        this.CurrentCommand,
                        string.Format("'{0}' is not a valid command.", commandName)
                        );
                    //this.ShowCommands(null, ref commandResult);
                    m_History.Add(commandResult);
                }
            }
            this.CurrentCommand = string.Empty;
        }

        /// <summary>
        /// Handle input.
        /// </summary>
        /// <param name="p_InputState">Input state.</param>
        public void HandleInput(InputState p_InputState)
        {
            KeysList keys = p_InputState.GetNewKeyPresses();
            foreach (Keys key in keys)
            {
                if (key == Keys.Enter)
                {
                    this.ProcessCommand();
                }
                else if (key == Keys.Escape)
                {
                    this.CurrentCommand = string.Empty;
                }
                else if (key == Keys.Back)
                {
                    if (this.CurrentCommand != null && this.CurrentCommand.Length > 0)
                    {
                        this.CurrentCommand = this.CurrentCommand.Substring(0, this.CurrentCommand.Length - 1);
                    }
                }
                else
                {
                    bool isShiftKeyDown = p_InputState.IsShiftKeyDown();
                    if (IsValidCommandKey(key))
                    {
                        this.CurrentCommand += GetCharFromCommandKey(key, isShiftKeyDown);
                    }
                }
            }
        }

        #region Command keys

        private static Dictionary<Keys, char> s_CommandKeysToChars = new Dictionary<Keys, char>();
        private static Dictionary<Keys, char> s_CommandKeysWithShiftToChars = new Dictionary<Keys, char>();

        private static void InitializeCommandKeys()
        {
            s_CommandKeysToChars.Add(Keys.A, 'a');
            s_CommandKeysToChars.Add(Keys.B, 'b');
            s_CommandKeysToChars.Add(Keys.C, 'c');
            s_CommandKeysToChars.Add(Keys.D, 'd');
            s_CommandKeysToChars.Add(Keys.E, 'e');
            s_CommandKeysToChars.Add(Keys.F, 'f');
            s_CommandKeysToChars.Add(Keys.G, 'g');
            s_CommandKeysToChars.Add(Keys.H, 'h');
            s_CommandKeysToChars.Add(Keys.I, 'i');
            s_CommandKeysToChars.Add(Keys.J, 'j');
            s_CommandKeysToChars.Add(Keys.K, 'k');
            s_CommandKeysToChars.Add(Keys.L, 'l');
            s_CommandKeysToChars.Add(Keys.M, 'm');
            s_CommandKeysToChars.Add(Keys.N, 'n');
            s_CommandKeysToChars.Add(Keys.O, 'o');
            s_CommandKeysToChars.Add(Keys.P, 'p');
            s_CommandKeysToChars.Add(Keys.Q, 'q');
            s_CommandKeysToChars.Add(Keys.R, 'r');
            s_CommandKeysToChars.Add(Keys.S, 's');
            s_CommandKeysToChars.Add(Keys.T, 't');
            s_CommandKeysToChars.Add(Keys.U, 'u');
            s_CommandKeysToChars.Add(Keys.V, 'v');
            s_CommandKeysToChars.Add(Keys.W, 'w');
            s_CommandKeysToChars.Add(Keys.X, 'x');
            s_CommandKeysToChars.Add(Keys.Y, 'y');
            s_CommandKeysToChars.Add(Keys.Z, 'z');
            s_CommandKeysToChars.Add(Keys.D0, '0');
            s_CommandKeysToChars.Add(Keys.D1, '1');
            s_CommandKeysToChars.Add(Keys.D2, '2');
            s_CommandKeysToChars.Add(Keys.D3, '3');
            s_CommandKeysToChars.Add(Keys.D4, '4');
            s_CommandKeysToChars.Add(Keys.D5, '5');
            s_CommandKeysToChars.Add(Keys.D6, '6');
            s_CommandKeysToChars.Add(Keys.D7, '7');
            s_CommandKeysToChars.Add(Keys.D8, '8');
            s_CommandKeysToChars.Add(Keys.D9, '9');
            s_CommandKeysToChars.Add(Keys.NumPad0, '0');
            s_CommandKeysToChars.Add(Keys.NumPad1, '1');
            s_CommandKeysToChars.Add(Keys.NumPad2, '2');
            s_CommandKeysToChars.Add(Keys.NumPad3, '3');
            s_CommandKeysToChars.Add(Keys.NumPad4, '4');
            s_CommandKeysToChars.Add(Keys.NumPad5, '5');
            s_CommandKeysToChars.Add(Keys.NumPad6, '6');
            s_CommandKeysToChars.Add(Keys.NumPad7, '7');
            s_CommandKeysToChars.Add(Keys.NumPad8, '8');
            s_CommandKeysToChars.Add(Keys.NumPad9, '9');
            s_CommandKeysToChars.Add(Keys.Space, ' ');

            s_CommandKeysWithShiftToChars.Add(Keys.A, 'A');
            s_CommandKeysWithShiftToChars.Add(Keys.B, 'B');
            s_CommandKeysWithShiftToChars.Add(Keys.C, 'C');
            s_CommandKeysWithShiftToChars.Add(Keys.D, 'D');
            s_CommandKeysWithShiftToChars.Add(Keys.E, 'E');
            s_CommandKeysWithShiftToChars.Add(Keys.F, 'F');
            s_CommandKeysWithShiftToChars.Add(Keys.G, 'G');
            s_CommandKeysWithShiftToChars.Add(Keys.H, 'H');
            s_CommandKeysWithShiftToChars.Add(Keys.I, 'I');
            s_CommandKeysWithShiftToChars.Add(Keys.J, 'J');
            s_CommandKeysWithShiftToChars.Add(Keys.K, 'K');
            s_CommandKeysWithShiftToChars.Add(Keys.L, 'L');
            s_CommandKeysWithShiftToChars.Add(Keys.M, 'M');
            s_CommandKeysWithShiftToChars.Add(Keys.N, 'N');
            s_CommandKeysWithShiftToChars.Add(Keys.O, 'O');
            s_CommandKeysWithShiftToChars.Add(Keys.P, 'P');
            s_CommandKeysWithShiftToChars.Add(Keys.Q, 'Q');
            s_CommandKeysWithShiftToChars.Add(Keys.R, 'R');
            s_CommandKeysWithShiftToChars.Add(Keys.S, 'S');
            s_CommandKeysWithShiftToChars.Add(Keys.T, 'T');
            s_CommandKeysWithShiftToChars.Add(Keys.U, 'U');
            s_CommandKeysWithShiftToChars.Add(Keys.V, 'V');
            s_CommandKeysWithShiftToChars.Add(Keys.W, 'W');
            s_CommandKeysWithShiftToChars.Add(Keys.X, 'X');
            s_CommandKeysWithShiftToChars.Add(Keys.Y, 'Y');
            s_CommandKeysWithShiftToChars.Add(Keys.Z, 'Z');
        }

        private static bool IsValidCommandKey(Keys p_Key)
        {
            return s_CommandKeysToChars.ContainsKey(p_Key);
        }

        private static char GetCharFromCommandKey(Keys p_Key, bool p_IsShiftKeyDown)
        {
            char result;
            if (p_IsShiftKeyDown && s_CommandKeysWithShiftToChars.ContainsKey(p_Key))
            {
                result = s_CommandKeysWithShiftToChars[p_Key];
            }
            else
            {
                result = s_CommandKeysToChars[p_Key];
            }
            return result;
        }

        #endregion

        #region Singleton instance

        private static CommandManager s_Instance;

        public static CommandManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new CommandManager();
                }
                return s_Instance;
            }
        }

        #endregion
    }
}
