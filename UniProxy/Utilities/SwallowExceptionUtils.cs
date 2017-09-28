using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniProxy.Utilities
{
    [DebuggerDisplay("Kills exceptions")]
    [DebuggerNonUserCode]
    internal class SwallowExceptionUtils
    {
        public static bool TryExec(Action input)
        {
            try
            {
                input();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static async Task<bool> TryExec(Func<Task> input)
        {
            try
            {
                await input();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}