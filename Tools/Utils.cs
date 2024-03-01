namespace Hipercow_api
{
    using Microsoft.Hpc.Scheduler.Properties;

    public static class Utils
    {
        public static String HPCString(StoreProperty sp)
        {
            return sp.Value.ToString();
        }

        public static int HPCInt(StoreProperty sp) 
        {
            return int.Parse(HPCString(sp));
        }
    }
}
