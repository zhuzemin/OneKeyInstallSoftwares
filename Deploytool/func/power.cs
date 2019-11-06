using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;

namespace Deploytool.func
{
    class power
    {
        public static void alwaysOn()
        {
            //disable all power saving setting
            string batch = "powercfg /x -hibernate-timeout-ac 0 && powercfg /x -hibernate-timeout-dc 0 && powercfg /x -disk-timeout-ac 0 && powercfg /x -disk-timeout-dc 0 && powercfg /x -monitor-timeout-ac 0 && powercfg /x -monitor-timeout-dc 0 && Powercfg /x -standby-timeout-ac 0 && powercfg /x -standby-timeout-dc 0";
            cmd.ExecuteCommand(batch);
        }
        public static void PowerButton()
        {
            //press power button do nothing
            string batch = "powercfg -setacvalueindex SCHEME_CURRENT 4f971e89-eebd-4455-a8de-9e59040e7347 7648efa3-dd9c-4e3e-b566-50f929386280 0";
            cmd.ExecuteCommand(batch);
        }
    }
}
