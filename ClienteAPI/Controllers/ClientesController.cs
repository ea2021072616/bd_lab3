using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClienteAPI.Data; // Asegúrate de que esta ruta sea correcta
using ClienteAPI.Models; // Asegúrate de que esta ruta sea correcta

[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly BdClientesContext _context;

    public ClientesController(BdClientesContext context)
    {
        _context = context;
    }

    // GET: api/clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
    {
        return await _context.Clientes.ToListAsync();
    }

    // POST: api/clientes
    [HttpPost]
    public async Task<ActionResult<Cliente>> CreateCliente(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetClientes), new { id = cliente.Id }, cliente);
    }

}
