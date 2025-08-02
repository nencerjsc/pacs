using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NencerApi.Modules.PacsServer.Service;

namespace NencerApi.Modules.PacsServer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DicomListenerController : ControllerBase
    {
        private readonly DicomListenerManager _listenerManager;

        public DicomListenerController(DicomListenerManager listenerManager)
        {
            _listenerManager = listenerManager;
        }

        [HttpPost("start")]
        public IActionResult StartListener()
        {
            _listenerManager.Start();
            return Ok(new { status = "started", running = _listenerManager.IsRunning });
        }

        [HttpPost("stop")]
        public IActionResult StopListener()
        {
            _listenerManager.Stop();
            return Ok(new { status = "stopped", running = _listenerManager.IsRunning });
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new { running = _listenerManager.IsRunning });
        }
    }
}
