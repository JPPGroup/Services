using Jpp.Projects.MailAI;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Jpp.Projects.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MailController : ControllerBase
    {
        private readonly MailDbContext _db;

        public MailController(MailDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        public async Task PutAsync(MailClassification mc)
        {
            _db.Classifications.Add(mc);
            await _db.SaveChangesAsync();
        }
    }
}
