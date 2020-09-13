using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace Larnaca.Build.Tools
{
    /// <summary>
    /// Read a list of lines from the file and return it as an item list
    /// </summary>
    public class ReadLinesFromFile : Task
    {
        public string File { get; set; }
        [Output]
        public string[] Lines { get; set; }
        public override bool Execute()
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"{nameof(ReadLinesFromFile)} task running", "Larnaca Framework 🌴", nameof(ReadLinesFromFile), MessageImportance.High));

            if (System.IO.File.Exists(File))
            {
                Lines = System.IO.File.ReadAllLines(File);
            }
            else
            {
                Lines = new string[0];
            }

            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Contents: " + string.Join(Environment.NewLine, Lines), "Larnaca Framework 🌴", nameof(ReadLinesFromFile), MessageImportance.Low));

            return true;
        }
    }
}
