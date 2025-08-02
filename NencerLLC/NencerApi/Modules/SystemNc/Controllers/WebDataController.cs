using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.SystemNc.Model;
using NencerApi.Modules.SystemNc.Model.DTO;
using NencerCore;
namespace NencerApi.Modules.SystemNc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WebDataController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("List")]
        public async Task<BaseResponse<dynamic>> ListWebData(string? Type)
        {


            var list = _context.WebDatas.Where(x => (string.IsNullOrEmpty(Type) || x.Type == Type))
            .GroupBy(w => w.Type)
            .ToDictionary(
            g => g.Key,
            g => g.Select(w => new
            {
                Code = w.Code,
                Name = w.Name
            }).ToList()
        );
            return new BaseResponse<dynamic>("200", "SUCCESS", list);
        }

    }
}
