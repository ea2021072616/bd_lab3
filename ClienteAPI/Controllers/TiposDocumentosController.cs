using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClienteAPI.Data;
using ClienteAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class TiposDocumentosController : ControllerBase
{
    private readonly BdClientesContext _context;

    public TiposDocumentosController(BdClientesContext context)
    {
        _context = context;
    }

    // GET: api/tiposdocumentos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TiposDocumentos>>> GetTiposDocumentos()
    {
        return await _context.TiposDocumentos.ToListAsync();
    }

    // GET: api/tiposdocumentos/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TiposDocumentos>> GetTipoDocumento(int id)
    {
        var tipoDocumento = await _context.TiposDocumentos.FindAsync(id);

        if (tipoDocumento == null)
        {
            return NotFound(); // Devuelve un 404 si no se encuentra el tipo de documento
        }

        return tipoDocumento; // Devuelve el tipo de documento encontrado
    }


}
