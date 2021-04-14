using System;
using System.Collections.Generic;
using DWorldProject.Data.Entities;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;

namespace DWorldProject.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("EnableCors")]
    //[Authorize]
    [AllowAnonymous]
    public class LogController : BaseController
    {
        public readonly IElasticSearchService _elasticSearchService;
        public LogController(IConfiguration configuration, IElasticSearchService elasticSearchService) : base(configuration)
        {
            _elasticSearchService = elasticSearchService;
        }

        [HttpPost("[action]")]
        public IActionResult PutElasticIndex()
        {
            _elasticSearchService.CheckIndex("log");
            //var response = elasticClient.Search<Log>(i => i
            //    .Query(q => q.MatchAll())
            //    .PostFilter(f => f.Range(r => r.Field(fi => fi.Id).GreaterThan(1)))
            //);
            //List<Log> items = new List<Log>();
            //foreach (var item in response.Documents)
            //    items.Add(item);
            //return items;
            return Ok();
        }
    }
}
