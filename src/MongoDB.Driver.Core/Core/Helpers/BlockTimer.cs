using System;
using System.Diagnostics;

namespace MongoDB.Driver.Core
{
    /// <summary>
    /// Used for timing operations.
    /// </summary>
    public sealed class BlockTimer : IDisposable
    {
        Stopwatch _sw;
        string _description;
        Action<string> _logger = description => Debug.WriteLine(description);
        long _subTimeMs;
        string _lastSubRoutine;

        /// <summary>
        /// Shorthand for new BlockTimer
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static BlockTimer For(string description)
        {
            return new BlockTimer(description);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        BlockTimer(string description, Action<string> logger = null)
        {
            _description = description;
            if (logger != null)
                _logger = logger;
            _subTimeMs = 0;
            _lastSubRoutine = string.Empty;
            _sw = Stopwatch.StartNew();
        }

        /// <summary>
        /// Sets logger
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public BlockTimer LogUsing(Action<string> logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public BlockTimer IncludeStart(string description = null)
        {
            var startDescription = string.IsNullOrWhiteSpace(description) ? _description : description;
            _logger($"[{DateTime.Now.ToString("M/d/yyyy hh:mm:ss")}] ------ Started: {startDescription} ------");
            return this;
        }

        /// <summary>
        /// Add to description
        /// </summary>
        /// <param name="description"></param>
        public void Append(string description)
        {
            _description += $" {description}";
        }

        /// <summary>
        /// Subroutine time.
        /// </summary>
        /// <param name="finishedSubRoutine"></param>
        /// <param name="nextSubRoutine"></param>
        public void SubTime(string finishedSubRoutine, string nextSubRoutine)
        {
            _subTimeMs = _sw.ElapsedMilliseconds - _subTimeMs;
            _logger($"{finishedSubRoutine} in {_subTimeMs} ms");
            _lastSubRoutine = nextSubRoutine;
        }

        /// <summary>
        /// On complete...
        /// </summary>
        public void Dispose()
        {
            _sw.Stop();
            if (_subTimeMs > 0)
                _logger($"{_lastSubRoutine} in {_sw.ElapsedMilliseconds - _subTimeMs} ms.");
            _logger($"========== Finished: {_description} in {_sw.ElapsedMilliseconds} ms. ==========");
            _logger(string.Empty);
        }
    }

}
