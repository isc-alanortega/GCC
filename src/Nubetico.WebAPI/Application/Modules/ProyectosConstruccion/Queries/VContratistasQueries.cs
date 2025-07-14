using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Queries
{
    public static class VContratistasQueries
    {
        public static IQueryable<ElementsDropdownForm> GetvContratistasElementsDropdown(this IQueryable<vContratistas> vContratistas)
            => vContratistas
                .Where(item => item.Habilitado)
                .Select(item => new ElementsDropdownForm()
                {
                    Id = item.Id_Contratista,
                    Name = $"{item.Nombres} {item.PrimerApellido}"
                });
    }
}
