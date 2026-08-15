using Axlon.Services.Category.Seed;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Category.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitDataBaseController : ControllerBase
    {
        private readonly DBSeed dbSeed;

        public InitDataBaseController(DBSeed dbSeed)
        {
            this.dbSeed = dbSeed;
        }

        [HttpGet("InitTable")]
        public bool InitTable()
        {
            dbSeed.InitTable();
            return true;
        }

        [HttpGet("InitTableData")]
        public async Task<bool> InitTableData()
        {
            await dbSeed.InitTableData();
            return true;
        }
    }
}
