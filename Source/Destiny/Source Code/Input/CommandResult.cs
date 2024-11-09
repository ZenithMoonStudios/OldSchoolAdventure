using System.Collections.Generic;

namespace Destiny
{
    public class CommandResult
    {
        public string Command { get; private set; }
        public StringList OutputLines { get; private set; }

        public CommandResult(string p_Command)
        {
            this.Command = p_Command;
            this.OutputLines = new StringList();
        }

        public CommandResult(string p_Command, string p_Output) : this(p_Command)
        {
            this.AddOutputLine(p_Output);
        }

        public void AddOutputLine(string p_Text)
        {
            this.OutputLines.Add(p_Text);
        }
    }

    public class CommandResultList : List<CommandResult> { }
}
