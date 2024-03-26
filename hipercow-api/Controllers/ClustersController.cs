namespace Hipercow_api.Controllers
{
    using Hipercow_api.Tools;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClustersController : ControllerBase
    {
        private IClusterInfoQuery clusterInfoQuery = new ClusterInfoQuery();

        public void MockClusterInfoQuery(IClusterInfoQuery inject)
        {
            this.clusterInfoQuery = inject;
        }

        [HttpGet]
        public List<string> Get()
        {
            return DideConstants.GetDideClusters();
        }

        [HttpGet("{cluster}")]
        public IActionResult Get(string cluster)
        {
            ClusterInfo? info = this.clusterInfoQuery.GetClusterInfo(cluster);
            if (info != null)
            {
                return this.Ok(info);
            }

            return this.NotFound();
        }
    }
}
