using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DracosBot
{
    public class Command 
    {

        public static string PING_COMMAND = PingCommand.command;
        public static string PING_ANSWER = PingCommand.answer;
        public static string[] ROBERT_COMMAND = RobertCommand.commands;
        public static string ROBERT_ANSWER = RobertCommand.answer;
        public static string[] GAG_COMMAND = { "!9gag-random", "!9gag-post" };

        public static bool isCommand(string[] commands, string lookupCommand)
        {
            return commands.Contains(lookupCommand);
        }
    }
}
