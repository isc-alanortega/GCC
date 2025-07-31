using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.Dialogs.ProyectosConstruccion
{

    public partial class DetailSubdivisionDialogComponent : NbBaseComponent
    {
        [Parameter]
        public TipoEstadoControl EstadoControl { get; set; }
		[Parameter]
		public GeneralDetailSubdivision DetailData { get; set; } = new GeneralDetailSubdivision();
        [Parameter]
        public IEnumerable<string>? InvalidNames { get; set; } = Enumerable.Empty<string>();
        [Parameter]
        public bool ShowDescription { get; set; } = true;
		private RadzenTemplateForm<GeneralDetailSubdivision> DetailForm { get; set; }

		protected override async Task OnAfterRenderAsync(bool firsRender)
		{
			if (firsRender && EstadoControl == TipoEstadoControl.Alta)
			{
				DetailForm.EditContext.Validate();
			}
		}

		private void OnClickAccept()
        {
            // Validations
            if (!DetailForm.EditContext.Validate())
            {
                return;
            }

			DetailData.Name = DetailData.Name.ToUpper().Trim();
			DetailData.Description = string.IsNullOrEmpty(DetailData.Description) ? null : DetailData.Description.ToUpper().Trim();

			if (InvalidNames.Any() && InvalidNames.Contains(DetailData.Name))
            {
				var notification = new NotificationMessage
				{
					Severity = NotificationSeverity.Error,
					Summary = "Error",
					Detail = Localizer["Subdivisions.Error.InvalidName"],
					Duration = 3500
				};

				notificationService.Notify(notification);
                return;
			}

			dialogService.Close(DetailData);
		}

        private void OnClickCancel()
        {
            dialogService.Close();
        }

        private async Task<IEnumerable<KeyValuePair<int, string>>> GetSubdivisions()
        {
            var listGetResponse = await subdivisionService.GetSubdivisionsList();
            if (listGetResponse != null && listGetResponse.StatusCode == 200)
            {
                return JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<int, string>>>(listGetResponse.Data.ToString());
            }

            return Enumerable.Empty<KeyValuePair<int, string>>();
        }
    }
}
