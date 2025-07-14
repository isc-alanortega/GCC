using Microsoft.Extensions.Localization;
using Radzen.Blazor;
using System.Globalization;

namespace Nubetico.Frontend.Models.Static.Core
{
    public class MenuItemsFactory
    {
        /// <summary>
        /// Función sobrecargada que retorna los menus con el servicio localizer como parámetro
        /// </summary>
        /// <param name="localizer"></param>
        /// <returns></returns>
        public static List<RadzenMenuItem> GetBaseMenuItems(IStringLocalizer<SharedResources> localizer)
        {
            return new List<RadzenMenuItem>
            {
                CreateMenuItem("abrir", localizer),
                CreateMenuItem("agregar", localizer),
                CreateMenuItem("editar", localizer),
                CreateMenuItem("borrar", localizer),
                CreateMenuItem("refrescar", localizer),
                CreateMenuItem("buscar", localizer),
                CreateMenuItem("exportar", localizer),
                CreateMenuItem("guardar", localizer),
                CreateMenuItem("cancelar", localizer),
                CreateMenuItem("cerrar", localizer)
            };
        }

        private static RadzenMenuItem CreateMenuItem(string command, IStringLocalizer<SharedResources> localizer)
        {
            return new RadzenMenuItem
            {
                Icon = char.ConvertFromUtf32(Convert.ToInt32(MenuIconDictionary[command], 16)),
                Text = localizer[$"Shared.Comandos.{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(command)}"],
                Attributes = new Dictionary<string, object>
                {
                    { "comando", command }
                }
            };
        }

        public static readonly Dictionary<string, string> MenuIconDictionary = new()
        {
            { "abrir", "f07c" }, // fa-sharp fa-light fa-folder-open
            { "agregar", "002b" }, // fa-plus
            { "editar", "f044" }, // fa-pen-to-square
            { "borrar", "f1f8" }, // fa-trash
            { "refrescar", "f021" }, // fa-arrows-rotate
            { "buscar", "f002" }, // fa-magnifying-glass
            { "exportar", "f1c3" }, // fa-file-excel
            { "guardar", "f0c7" }, // fa-sharp fa-light fa-floppy-disk
            { "cancelar", "e215" }, // fa-pencil-slash
            { "cerrar", "f00d" } // fa-sharp fa-light fa-xmark
        };
    }
}
