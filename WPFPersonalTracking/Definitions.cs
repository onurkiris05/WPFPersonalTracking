using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPersonalTracking
{
    public static class Definitions
    {
        public static class TaskStates
        {
            public static int OnEmployee { get; set; } = 1;
            public static int Delivered { get; set; } = 2;
            public static int Approved { get; set; } = 3;
        }

        public static class PermissionStates
        {
            public static int OnEmployee { get; set; } = 1;
            public static int Approved { get; set; } = 2;
            public static int Disapproved { get; set; } = 3;
        }
    }
}
