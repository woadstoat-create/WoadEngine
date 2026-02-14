// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace WoadEngine.Diagnostics;

#region Logger
/// <summary>
/// Minimal debug-only file logger.
/// </summary>
/// <remarks>
/// - All logging methods are decorated with <see cref="ConditionalAttribute"/> so they are
///   compiled out when the DEBUG symbol is not defined.
/// - Call <see cref="Init"/> once on startup and <see cref="Shutdown"/> on exit.
/// </remarks>
public static class Logger
{
    private static readonly object _lock = new();
    private static StreamWriter? _writer;
    private static bool _initialised;

    /// <summary>
    /// True if the logger is currently initialised and able to write.
    /// In Release builds, this may be true but calls are still compiled out.
    /// </summary>
    public static bool IsInitialised
    {
        get { lock (_lock) return _initialised; }
    }

    /// <summary>
    /// Initialises the logger and opens a log file for writing (debug builds only).
    /// </summary>
    /// <param name="logDirectory">
    /// Directory to place logs in. If null/empty, uses "Logs" under the app base directory.
    /// </param>
    /// <param name="fileNamePrefix">Prefix for the log file name.</param>
    /// <param name="append">If true, appends to the latest log file instead of creating a new one.</param>
    [Conditional("DEBUG")]
    public static void Init(string? logDirectory = null, string fileNamePrefix = "woadengine", bool append = false)
    {
        lock (_lock)
        {
            if (_initialised)
                return;

            var dir = string.IsNullOrWhiteSpace(logDirectory)
                ? Path.Combine(AppContext.BaseDirectory, "Logs")
                : logDirectory;

            Directory.CreateDirectory(dir);

            string fileName = append
                ? $"{fileNamePrefix}.log"
                : $"{fileNamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.log";

            var path = Path.Combine(dir, fileName);

            _writer = new StreamWriter(path, append: append, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
            {
                AutoFlush = true
            };

            _initialised = true;

            // Header
            WriteLine($"=== Log Start {DateTime.Now:O} ===");
            WriteLine($"Process: {Environment.ProcessPath}");
            WriteLine($"OS: {Environment.OSVersion}");
            WriteLine($"Framework: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
            WriteLine("");
        }
    }

    /// <summary>
    /// Writes an informational log entry (debug builds only).
    /// </summary>
    [Conditional("DEBUG")]
    public static void Info(string message, 
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        Write("INFO", message, member, file, line);
    }

    /// <summary>
    /// Writes a warning log entry (debug builds only).
    /// </summary>
    [Conditional("DEBUG")]
    public static void Warn(string message,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        Write("WARN", message, member, file, line);
    }

    /// <summary>
    /// Writes an error log entry (debug builds only).
    /// </summary>
    [Conditional("DEBUG")]
    public static void Error(string message,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        Write("ERROR", message, member, file, line);
    }

    /// <summary>
    /// Writes an exception log entry (debug builds only).
    /// </summary>
    [Conditional("DEBUG")]
    public static void Exception(Exception ex, string? message = null,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        var msg = message is null ? ex.ToString() : $"{message}\n{ex}";
        Write("EX", msg, member, file, line);
    }

    /// <summary>
    /// Shuts down the logger and closes the log file (debug builds only).
    /// </summary>
    [Conditional("DEBUG")]
    public static void Shutdown()
    {
        lock (_lock)
        {
            if (!_initialised)
                return;

            try
            {
                WriteLine($"=== Log end {DateTime.Now:O} ===");
            }
            catch { /* ignore */ }

            _writer?.Dispose();
            _writer = null;
            _initialised = false;
        }
    }

    #region Internals

    [Conditional("DEBUG")]
    private static void Write(string level, string message, string member, string file, int line)
    {
        lock (_lock)
        {
            if (!_initialised || _writer is null)
                return;

            string shortFile = Path.GetFileName(file);
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");

            _writer.WriteLine($"{timestamp} [{level}] {shortFile}:{line} {member}() - {message}");
        }
    }

    [Conditional("DEBUG")]
    private static void WriteLine(string message)
    {
        lock (_lock)
        {
            if (!_initialised || _writer is null)
                return;

            _writer.WriteLine(message);
        }
    }

    #endregion
}
#endregion