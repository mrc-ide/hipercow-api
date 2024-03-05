namespace Hipercow_api
{
    using Microsoft.Hpc.Scheduler.Properties;

    public static class Utils
    {
        public static string HPCString(StoreProperty sp)
        {
            return sp.Value.ToString() ?? string.Empty;
        }

        public static int HPCInt(StoreProperty sp) 
        {
            return int.Parse(HPCString(sp));
        }
    }
}
