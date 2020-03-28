using System;
using Microsoft.AspNetCore.Mvc;
using PropLogicSolver;

namespace TruthTableSolver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TruthTableController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get([FromQuery]string expr)
        {
            if (expr == null)
            {
                return BadRequest("Request was empty.");
            }

            try
            {
                var solver = new PropLogicSolver.TruthTableSolver(new TruthExpression(expr));

                return solver.SolveToString();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            

        }










    }
}