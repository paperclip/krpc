using System;
using UnityEngine;

namespace KRPC.Utils
{
    public static class Logger
    {
        private static bool shouldLogToUnity = false;

        public static bool ShouldLogToUnity {
            private get { return shouldLogToUnity; }
            set { shouldLogToUnity = value; }
        }

        internal static void WriteLine (string line)
        {
            if (ShouldLogToUnity)
                WriteLineToUnityLog ("[kRPC] " + line);
            else
                Console.WriteLine ("[kRPC] " + line);
        }

        private static void WriteLineToUnityLog (string line)
        {
            Debug.Log (line);
        }
    }
}

