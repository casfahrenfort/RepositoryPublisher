﻿using System;
using System.Diagnostics;

public static class ShellHelper
{
    public static string Bash(string filename, string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = cmd,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
}
