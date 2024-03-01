using Microsoft.Hpc.Scheduler.Properties;
using Microsoft.Identity.Client;

namespace hipercow_api
{
    public static class Utils
    {
        public static String HPCString(StoreProperty sp)
        {
            return sp.Value.ToString();
        }

        public static int HPCInt(StoreProperty sp) {
            return int.Parse(HPCString(sp));
        }
    }
}
