using Microsoft.AspNetCore.Components;

namespace Nubetico.Frontend.Helpers
{
    public static class RenderFragmentHandler
    {
        public static RenderFragment RenderByType(Type tipoComponente)
        {
            return builder =>
            {
                builder.OpenComponent(0, tipoComponente);
                builder.CloseComponent();
            };
        }

        public static RenderFragment RenderByType(Type tipoComponente, Dictionary<string, object> parametros)
        {
            return builder =>
            {
                builder.OpenComponent(0, tipoComponente);

                int i = 1;
                foreach (var param in parametros)
                {
                    builder.AddAttribute(i++, param.Key, param.Value);
                }

                builder.CloseComponent();
            };
        }
    }
}
