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
        public static string[] ROBERT_COMMANDS = RobertCommand.commands;
        public static string ROBERT_ANSWER = RobertCommand.answer;
        public static string[] GAG_COMMANDS = _9gagCommand.commands;
        public static string[] GAG_ANSWERS(int index)
        {
            string[] returnArray = { _9gagCommand.answer(index), "1" };
            return returnArray;
        }
        public static bool isCommand(string[] commands, string lookupCommand)
        {
            return commands.Contains(lookupCommand);
        }
        public static int getIndex(string[] commands, string lookupCommand)
        {
            
            int index;
            for (index = 0; index < commands.Length; index++)
            {
                if (commands[index] == lookupCommand)
                    return index;
            }
             return -1;
        }
    }
}
