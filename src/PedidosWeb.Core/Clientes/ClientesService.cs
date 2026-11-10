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
        Cliente? cliente = null;
        for (int i = 0; i < _clientes.Count; i++)
        {
            if (_clientes[i].Id == id) { cliente = _clientes[i]; break; }
        }
        if (cliente == null) return "";
        string rut = cliente.Rut.Replace(".", "").Replace(" ", "").ToUpper();
        string zona = "";
        if (cliente.Region == "RM") zona = "Zona centro";
        else if (cliente.Region == "V") zona = "Zona centro";
        else if (cliente.Region == "VIII") zona = "Zona sur";
        else if (cliente.Region == "II") zona = "Zona norte";
        else zona = "Sin zona";
        string estado = "";
        if (cliente.Activo == true) estado = "Activo"; else estado = "Inactivo";
        return cliente.Nombre + " | " + rut + " | " + zona + " | " + estado;
    }

    // si existe y está activo
    public bool EstaActivo(string id)
    {
        Cliente? cliente = null;
        for (int i = 0; i < _clientes.Count; i++)
        {
            if (_clientes[i].Id == id) { cliente = _clientes[i]; break; }
        }
        if (cliente == null) return false;
        if (cliente.Activo == true) return true; else return false;
    }
}
