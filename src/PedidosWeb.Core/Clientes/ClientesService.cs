namespace PedidosWeb.Core.Clientes;

// Servicio heredado: lo escribió un ex integrante del equipo, con apuro.
// Funciona y tiene 4 tests. Nadie se ha atrevido a tocarlo.
public class ClientesService
{
    private readonly List<Cliente> _clientes;

    public ClientesService() : this(ClientesDeEjemplo.Crear()) { }

    public ClientesService(List<Cliente> clientes)
    {
        _clientes = clientes;
    }

    // ficha del cliente para la pantalla
    public string ObtenerFicha(string id)
    {
        Cliente? cliente = BuscarPorId(id);
        if (cliente == null) return "";
        string rut = cliente.Rut.Replace(".", "").Replace(" ", "").ToUpper();
        string zona = cliente.Region switch
        {
            "RM" or "V" => "Zona centro",
            "VIII" => "Zona sur",
            "II" => "Zona norte",
            _ => "Sin zona"
        };
        string estado = cliente.Activo ? "Activo" : "Inactivo";
        return cliente.Nombre + " | " + rut + " | " + zona + " | " + estado;
    }

    // si existe y está activo
    public bool EstaActivo(string id)
    {
        Cliente? cliente = BuscarPorId(id);
        return cliente != null && cliente.Activo;
    }

    // el primer cliente con ese id exacto, o null si no existe
    private Cliente? BuscarPorId(string id) => _clientes.FirstOrDefault(c => c.Id == id);
}
