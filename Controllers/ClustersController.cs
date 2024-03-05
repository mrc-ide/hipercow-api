namespace Hipercow_api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/[controller]")]
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
        public IActionResult Get(string cluster)
        {
            ClusterInfo? info = ClusterInfoQuery.GetClusterInfo(cluster);
            if (info != null)
            {
                return this.Ok(info);
            }
            return this.NotFound();
        }
    }
}
