using Microsoft.AspNetCore.Mvc;

namespace hipercow_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClustersController : ControllerBase
    {
        private readonly ILogger<ClustersController> _logger;

        public ClustersController(ILogger<ClustersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<String> Get()
        {
            return DideConstants.DideClusters;
        }

        [HttpGet("{cluster}")]
        public ClusterInfo Get(String cluster)
        {
            return ClusterInfoQuery.getClusterInfo(cluster);
        }
    }
}
