namespace Hipercow_api.Tools
{
    public interface IClusterInfoQuery
    {
        public ClusterInfo? GetClusterInfo(string cluster);
    }
}
