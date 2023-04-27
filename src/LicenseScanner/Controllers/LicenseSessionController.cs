using Microsoft.AspNetCore.Mvc;

namespace LicenseScanner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LicenseSessionController : ControllerBase
    {
        private readonly ILogger<LicenseSessionController> _logger;
        private readonly LicenseContext _context;

        public LicenseSessionController(ILogger<LicenseSessionController> logger, LicenseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<LicenseSession>> Get()
        {
            return _context.Sessions.Where(s => s.EndTime == null);
        }

        [HttpPut]
        public async Task<IActionResult> Put(LicenseType licenseType, LicenseAction action, string Username)
        {
            switch (action)
            {
                case LicenseAction.Acquire:
                    LicenseSession session = new LicenseSession()
                    {
                        Type = licenseType,
                        StartTime = DateTime.Now,
                        Username = Username
                    };
                    _context.Sessions.Add(session);
                    break;

                case LicenseAction.Release:
                    var activeSession = _context.Sessions.FirstOrDefault(s => s.Username == Username && s.EndTime == null);
                    if (activeSession == null)
                    {
                        _logger.LogError("No active session found for {0} on {1}", Username, licenseType);
                        return NotFound();
                    }
                    activeSession.EndTime = DateTime.Now;
                    break;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}