using System;
using System.Collections.Generic;
using System.Text;

namespace LDSTest.src.utils
{
    public static class Docker
    {
        public static Boolean RunningInContainer()
        {
            string inDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

            if (inDocker != null && inDocker.Equals("true")) return true;
            else return false;
        }
    }
}
