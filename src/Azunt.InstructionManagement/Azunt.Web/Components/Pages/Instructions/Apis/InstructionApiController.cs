using Azunt.InstructionManagement;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Components.Pages.Instructions.Apis;

[ApiController]
[Route("api/[controller]")]
public class InstructionApiController : ControllerBase
{
    private readonly IInstructionRepository _repository;

    public InstructionApiController(IInstructionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Instruction>>> GetInstructions()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Instruction>> GetInstruction(long id)
    {
        var model = await _repository.GetByIdAsync(id);

        if (model.Id == 0)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<Instruction>> PostInstruction(Instruction model)
    {
        model.CreatedAt = DateTimeOffset.UtcNow;
        var result = await _repository.AddAsync(model);
        return CreatedAtAction(nameof(GetInstruction), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> PutInstruction(long id, Instruction model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var success = await _repository.UpdateAsync(model);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteInstruction(long id)
    {
        var success = await _repository.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
