using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    public static class ConsoleHelper
    {
        public static bool GetYesOrNo(string QuestionMessage)
        {
            Console.Write($"{QuestionMessage} [y*/n]:");
            while (true)
            {
                var inputKey = Console.ReadKey().Key;
                if (inputKey == ConsoleKey.Y || inputKey == ConsoleKey.Enter)
                    return true;
                else if (inputKey == ConsoleKey.N)
                    return false;
                else
                    Console.WriteLine("\tInvalid key. Please enter [y]es (default) or [n]o.:");
            }
        }


        public static int GetNumberInRange(int minNo, int maxNo, string QuestionMessage)
        {
            Console.Write($"{QuestionMessage} [{minNo}]to[{maxNo}]:");
            while (true)
            {
                string inputStr = Console.ReadLine();
                if (int.TryParse(inputStr, out int inputNo))
                {
                    if (inputNo >= minNo && inputNo <= maxNo)
                        return inputNo;
                }
                Console.WriteLine($"\tInvalid input. Please enter a number between [{minNo}] and [{maxNo}]:");
            }
        }

        /// <summary>
        /// Creates console text of the form below, and retrieves the index of the selected option:
        /// [1] Option 1 Text
        /// [2] Option 2 Text
        /// [3] Option 3 Text
        /// Select an option. (QuestionMessage) [1]to[3]:_
        /// </summary>
        public static int GetOptionNumber(IList<string> options, string QuestionMessage = "Select an option.")
        {
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {options[i]}");
            }
            return GetNumberInRange(1, options.Count, QuestionMessage) - 1;
        }

        /// <summary>
        /// Creates console text of the form below, and retrieves the index of the selected option:
        /// [1] Option 1 Text
        /// [2] Option 2 Text
        /// [3] Option 3 Text
        /// Select an option. (QuestionMessage) [1]to[3]:_
        /// </summary>
        public static void DoActionByOptionNumber(IList<(string optionText, Action action)> optionActionPairs, string QuestionMessage = "Select an option.")
        {
            for (int i = 0; i < optionActionPairs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {optionActionPairs[i].optionText}");
            }
            int inputNo = GetNumberInRange(1, optionActionPairs.Count, QuestionMessage) - 1;
            optionActionPairs[inputNo].action.Invoke();
        }
    }
}
