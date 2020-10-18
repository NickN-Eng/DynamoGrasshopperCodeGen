using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    /// <summary>
    /// Base class for scanning Items in a Collection, and outputing a list of errors to the console.
    /// </summary>
    public abstract class CheckerBase<TCollection, TItem> where TItem : ICodeItem
    {
        public Dictionary<TItem, List<string>> ErrorMessages;
        public int ErrorCount;

        public CheckerBase(App app)
        {
            App = app;
        }

        protected App App;

        public bool Parse(TCollection collection)
        {
            ErrorMessages = new Dictionary<TItem, List<string>>();
            ErrorCount = 0;

            return ParseImplementation(collection);
        }

        protected abstract bool ParseImplementation(TCollection collection);

        /// <summary>
        /// Adds an error, using the CurrentFunction as the default function
        /// </summary>
        /// <param name="message"></param>

        protected void AddError(string message, TItem function)
        {
            ErrorCount++;
            if (ErrorMessages.ContainsKey(function))
                ErrorMessages[function].Add(message);
            else
                ErrorMessages[function] = new List<string>() { message };
        }

        public void WriteToConsole()
        {
            Console.WriteLine($"{typeof(TItem).Name}(s) have a total of {ErrorCount} errors.");
            if (ErrorMessages.Count > 0) Console.WriteLine();

            foreach (var kvp in ErrorMessages)
            {
                Console.WriteLine($"<{kvp.Key.Name}> at {kvp.Key.CodeDocument.FilePath}");
                foreach (var message in kvp.Value)
                {
                    Console.WriteLine(message);
                }
                Console.WriteLine();
            }
        }
    }
}
