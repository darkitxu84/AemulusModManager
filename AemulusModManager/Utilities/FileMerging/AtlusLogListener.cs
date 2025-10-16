using AtlusScriptLibrary.Common.Logging;
using System.Text.RegularExpressions;

namespace AemulusModManager.Utilities.FileMerging
{
    internal class AtlusLogListener : LogListener
    {

        internal AtlusLogListener(LogLevel logLevel) : base(logLevel) { }

        protected override void OnLogCore(object sender, LogEventArgs e)
        {
            // Ignore message script name conflicts as they're rarely a real problem
            if (!Regex.IsMatch(e.Message, "Compiler generated constant for MessageScript dialog .+ conflicts with another variable"))
                Utilities.ParallelLogger.Log($"[{e.Level.ToString().ToUpper()}] {e.Message}");
        }
    }
}
