using PedidosWeb.Core.Detalles;
using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPedidoRepositorio, PedidoRepositorioEnMemoria>();
builder.Services.AddSingleton<ListadoPedidosServicio>();
builder.Services.AddSingleton<ILineaPedidoRepositorio, LineaPedidoRepositorioEnMemoria>();
builder.Services.AddSingleton<DetallePedidoServicio>();

var app = builder.Build();

// Para comprobar que la API está arriba.
app.MapGet("/salud", () => Results.Ok(new { estado = "ok" }));

// Detalle de un pedido por id.
app.MapGet("/api/pedidos/{id:int}", (int id, IPedidoRepositorio repositorio) =>
    repositorio.ObtenerPorId(id) is { } pedido
        ? Results.Ok(pedido)
        : Results.NotFound());

// HU-02 · Detalle de un pedido con cliente, líneas y total (specs/HU-02-detalle-pedido/spec.md).
app.MapGet("/api/pedidos/{id:int}/detalle", (int id, DetallePedidoServicio servicio) =>
    servicio.Obtener(id) is { } detalle
        ? Results.Ok(detalle)
        : Results.NotFound());

// HU-01 · Listar pedidos con filtros (specs/HU-01-listar-pedidos/spec.md).
// Los parámetros entran como texto para que los errores de formato salgan en ProblemDetails con "errors".
app.MapGet("/api/pedidos", (
    string? clienteId,
    string? estado,
    string? desde,
    string? hasta,
    string? pagina,
    ListadoPedidosServicio servicio) =>
{
    var resultado = servicio.Listar(clienteId, estado, desde, hasta, pagina);

    return resultado.Pagina is { } paginaPedidos
        ? Results.Ok(paginaPedidos)
        : Results.ValidationProblem(resultado.Errores);
});

app.Run();

// Permite crear tests de integración con WebApplicationFactory<Program> más adelante.
public partial class Program { }
