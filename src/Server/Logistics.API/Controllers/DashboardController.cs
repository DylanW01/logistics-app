﻿namespace Logistics.API.Controllers;

[Route("[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("getDailyGrosses")]
    [ProducesResponseType(typeof(ResponseResult<DailyGrossesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = Permissions.Employee.View)]
    public async Task<IActionResult> GetDailyGrosses([FromQuery] GetDailyGrossesQuery request)
    {
        var result = await _mediator.Send(request);

        if (result.Success)
            return Ok(result);

        return BadRequest(result.Error);
    }
    
    [HttpGet("getMonthlyGrosses")]
    [ProducesResponseType(typeof(ResponseResult<MonthlyGrossesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = Permissions.Employee.View)]
    public async Task<IActionResult> GetMonthlyGrosses([FromQuery] GetMonthlyGrossesQuery request)
    {
        var result = await _mediator.Send(request);

        if (result.Success)
            return Ok(result);

        return BadRequest(result.Error);
    }

    [HttpGet("getOverallStats")]
    [ProducesResponseType(typeof(ResponseResult<OverallStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = Permissions.Employee.View)]
    public async Task<IActionResult> GetOverallStats([FromQuery] GetOverallStatsQuery request)
    {
        var result = await _mediator.Send(request);

        if (result.Success)
            return Ok(result);

        return BadRequest(result.Error);
    }
}
