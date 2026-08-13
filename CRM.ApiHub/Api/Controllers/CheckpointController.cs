using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.UseCases.Checkpoints;
using CRM.ApiHub.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.ApiHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckpointController : ControllerBase
{
    private readonly GetOrderCheckpointsUseCase _getCheckpointsUseCase;
    private readonly UpdateCheckpointStatusUseCase _updateStatusUseCase;
    private readonly ICheckpointRepository _repository;

    public CheckpointController(
        GetOrderCheckpointsUseCase getCheckpointsUseCase,
        UpdateCheckpointStatusUseCase updateStatusUseCase,
        ICheckpointRepository repository)
    {
        _getCheckpointsUseCase = getCheckpointsUseCase;
        _updateStatusUseCase = updateStatusUseCase;
        _repository = repository;
    }

    [HttpGet("orders/{idOrder}")]
    public async Task<IActionResult> GetOrderCheckpoints(long idOrder, CancellationToken ct)
    {
        try
        {
            var result = await _getCheckpointsUseCase.ExecuteAsync(idOrder, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener checkpoints.", details = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateCheckpointStatus(
        int id, 
        [FromBody] UpdateCheckpointStatusRequest request, 
        CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { message = "Usuario no autorizado." });
        }

        try
        {
            var success = await _updateStatusUseCase.ExecuteAsync(id, request.Status, request.Comments, userId, ct);
            if (!success)
            {
                return NotFound(new { message = $"No se encontró el checkpoint con ID {id} o falló la actualización." });
            }
            return Ok(new { message = "Estado de checkpoint actualizado exitosamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar estado del checkpoint.", details = ex.Message });
        }
    }

    [HttpPost("{id}/steps/{stepIndex}/toggle")]
    public async Task<IActionResult> ToggleStepStatus(
        int id, 
        int stepIndex, 
        [FromBody] ToggleStepStatusRequest request, 
        CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { message = "Usuario no autorizado." });
        }

        try
        {
            var success = await _repository.ToggleStepStatusAsync(id, stepIndex, request.IsCompleted, userId, ct);
            if (!success)
            {
                return BadRequest(new { message = "Error al actualizar el paso del checkpoint." });
            }
            return Ok(new { message = "Estado del paso del checkpoint actualizado exitosamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar el paso del checkpoint.", details = ex.Message });
        }
    }
}

public class UpdateCheckpointStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Comments { get; set; }
}

public class ToggleStepStatusRequest
{
    public bool IsCompleted { get; set; }
}
