namespace Hipercow_api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class ClustersController : ControllerBase
    {
        private readonly ILogger<ClustersController> logger;

        public ClustersController(ILogger<ClustersController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public List<string> Get()
        {
            return DideConstants.GetDideClusters();
        }

        [HttpGet("{cluster}")]
        public ClusterInfo Get(string cluster)
        {
            return ClusterInfoQuery.GetClusterInfo(cluster);
        }
    }
}
