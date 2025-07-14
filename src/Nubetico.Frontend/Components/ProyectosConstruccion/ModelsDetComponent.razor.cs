using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Components.Core.Dialogs;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core.Constans;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Models;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ModelsDetComponent
    {
        #region PARAMETER
        [Parameter] public ModelDto ModelData { get; set; }
        #endregion

        #region INJECTIONS
        [Inject] private ModelService? modelService { get; set; }
        [Inject] private IStringLocalizer<SharedResources> Localizer { get; set; }
        #endregion

        #region PROPERTIES
        private RadzenTemplateForm<ModelDto> ModelForm { get; set; }
        private RadzenDataGrid<InsumosModelos> SuppliesGrid { get; set; }
        private RadzenDataGrid<ModelSuppliesSimplify> SuppliesSimplifyGrid { get; set; }

        private LoadingDialogComponent _loading;
        private RadzenUpload ExcelUpload { get; set; }
        private bool IsFormInProgress { get; set; } = false;
        private bool IsSaving { get; set; } = false;
        #endregion

        #region METHODS        
        #region LIFECYCLE BLAZOR METHODS
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                if (this.EstadoControl == TipoEstadoControl.Alta) ValidateForm();
                if (ModelData.ModelSupplies.Any()) GroupSuppliesOnDataGrid();
            }

            base.OnAfterRender(firstRender);
        }
        #endregion

        #region BUTTONS HEADER METHODS
        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var baseMenu = MenuItemsFactory.GetBaseMenuItems(Localizer);
            var displayedMenu = new List<RadzenMenuItem>();

            void AgregarMenuSiExiste(string comando, EventCallback<MenuItemEventArgs> onClick)
            {
                var menu = baseMenu.FirstOrDefault(m => m.Attributes != null
                    && m.Attributes.TryGetValue("comando", out var comandoValue)
                    && comandoValue.ToString() == comando);

                if (menu != null)
                {
                    menu.Click = onClick;
                    displayedMenu.Add(menu);
                }
            }

            switch (this.EstadoControl)
            {
                case TipoEstadoControl.Alta:
                    AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSubmit));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
                case TipoEstadoControl.Edicion:
                    AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit));
                    AgregarMenuSiExiste(BaseMenuCommands.CANCEL, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickCancel));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
                default:
                    AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;

            }

            return displayedMenu;
        }
        #endregion

        #region EVENTS
        private async void OnClickSubmit()
        {
            if (!ValidateForm() || IsSaving) return;

            IsSaving = true;
            _loading.Show(message: Localizer["Shared.Texts.SavingModelData"]);

            bool succces = await HandleSaveAsync();

            IsSaving = false;
            _loading.Hide();
            if (!succces) return;

            UpdateTab(state: TipoEstadoControl.Lectura);
        }

        private void OnClickEdit()
        {
            if (this.EstadoControl == TipoEstadoControl.Edicion) return;
            UpdateTab(state: TipoEstadoControl.Edicion);
        }

        private async void OnClickCancel()
        {
            UpdateTab(state: TipoEstadoControl.Lectura);

            if (this.EstadoControl != TipoEstadoControl.Edicion) return;

            var response = await modelService!.GetModelByIdAsync(ModelData.ModelId!.Value);
            if (!response!.Success || response.Data is null) return;

            ModelData = response.Data;
        }

        private void OnClickClose() => this.CerrarTabNubetico();

        private async Task OnFileChange(UploadChangeEventArgs args)
        {
            if (args.Files == null || !args.Files.Any())
            {
                // Enviar mensaje de error?
                return;
            }

            try
            {
                if (IsFormInProgress) return;

                IsFormInProgress = true;
                _loading.Show(message: Localizer["Shared.Texts.ExtractingExcelData"]);

                var file = args.Files.First();
                var uploadResult = await documentsService.PostLoadExcel<IEnumerable<InsumosModelos>>(file.Source, LoadExcelContants.PC_MODEL_EXCEL);

                // Show error notification if the result was unsuccessful
                if (uploadResult == null || uploadResult.Data == null)
                {
                    ShowErrorNotification("Excel.Error.Uncontrolled");
                    IsFormInProgress = false;
                    _loading.Hide();
                    return;
                }

                if (!uploadResult.Success)
                {
                    string errorMessage = !string.IsNullOrEmpty(uploadResult.Data.Exception)
                        ? "Excel.Error.Uncontrolled"
                        : uploadResult.Data.Error_Code ?? "Excel.Error.Uncontrolled";

                    ShowErrorNotification(errorMessage, uploadResult.Data.Aditional_Error);
                    IsFormInProgress = false;
                    _loading.Hide();
                    return;
                }

                // Load the supplies to the grid
                ModelData.ModelSupplies = uploadResult.Data.Result.OrderBy(item => item.Supply);
                ShowInfoNotification("Excel.Import.Successfull");

                GroupSuppliesOnDataGrid();
                _loading.Hide();
            }
            catch (Exception ex)
            {
                ShowErrorNotification("Excel.Error.Uncontrolled");
            }
            finally
            {
                IsFormInProgress = false;
            }
        }

        #endregion

        #region FORM METHODS
        private bool IsDisabled() => EstadoControl == TipoEstadoControl.Lectura;

        private bool ValidateForm() => ModelForm.EditContext.Validate();

        private void OnGroupRowRender(GroupRowRenderEventArgs args) 
        {
            if (args.FirstRender) // Solo expande en el primer render
            {
                args.Expanded = true;
            }
        }

        private async Task<bool> HandleSaveAsync()
        {
            var response = await (this.EstadoControl switch
            {
                TipoEstadoControl.Alta => modelService!.PostAddModelAsync(ModelData),
                //TipoEstadoControl.Edicion => modelService.PutUpdateModel(ModelData),
                _ => Task.FromResult<BaseResponseDto<ModelDto?>>(null)
            });

            if (response.StatusCode > 300)
            {
                string? message = response.StatusCode > 400 ? Localizer!["Shared.Text.UnknowError"] : response.Message;
                ShowErrorNotification(Localizer!["Shared.Text.ProblemOcurred"], message);
                return false;
            }

            ShowInfoNotification(Localizer!["Shared.Text.SaveSucces"]);
            return true;
        }

        private IEnumerable<ModelSuppliesSimplify> GetTableSuppliesSum() =>
         !ModelData!.ModelSupplies.Any() ? [] :
         ModelData.ModelSupplies
             .GroupBy(item => new { item.Type, item.Supply, item.Unit }) // Agrupar por Type y Supply
             .Select(group => new ModelSuppliesSimplify()
             {
                 Type = group.Key.Type,
                 Supply = group.Key.Supply,
                 Unit = group.Key.Unit,
                 Volume = group.Sum(item => item.Volume),
                 Amount = group.Sum(item => item.Amount),
                 Price = group.Sum(item => item.Price)
             })
            .OrderBy(item => item.Supply);

        private async Task GroupSuppliesSimplifyAsync()
        {
            var supplies = GetTableSuppliesSum();
            if (!supplies.Any()) return;

            SuppliesSimplifyGrid.Groups.Clear();
            SuppliesSimplifyGrid.Groups.Add(new GroupDescriptor() { Property = "Type", SortOrder = SortOrder.Ascending, Title = Localizer["Shared.Textos.Tipo"].Value.ToUpper() });
            StateHasChanged();
            await SuppliesSimplifyGrid.Reload();
        }



        private void UpdateTab(TipoEstadoControl state)
        {
            this.EstadoControl = state;
            SetNombreTabNubetico($"{Localizer!["Shared.Text.Supply"]} [{ModelData!.Name}]");
            this.TriggerMenuUpdate();
            StateHasChanged();
        }

        private async void GroupSuppliesOnDataGrid()
        {
            SuppliesGrid.Groups.Clear();

            SuppliesGrid.Groups.Add(new GroupDescriptor() { Property = "Group", SortOrder = SortOrder.Ascending, Title = Localizer["Models.Text.Group"].Value.ToUpper() });
            SuppliesGrid.Groups.Add(new GroupDescriptor() { Property = "Category", SortOrder = SortOrder.Ascending, Title = Localizer["Models.Text.Category"].Value.ToUpper() });
            SuppliesGrid.Groups.Add(new GroupDescriptor() { Property = "Unit_Price", SortOrder = SortOrder.Ascending, Title = Localizer["Models.Text.PU"].Value.ToUpper() });

            StateHasChanged();
            await SuppliesGrid.Reload();

            await GroupSuppliesSimplifyAsync();
        }
        #endregion

        #region NOTIFICATION
        private void ShowInfoNotification(string message) => Notify(Localizer["Subdivisions.Notify.Info"], Localizer[message], NotificationSeverity.Info);

        private void ShowErrorNotification(string message, string? additionalMessage = null)
        {
            string detail = string.IsNullOrEmpty(additionalMessage)
                    ? Localizer[message]
                    : string.Concat($"{Localizer[message]}\n", $"{Localizer["Excel.Text.Row"]} {additionalMessage}.");

            Notify("Error", detail, NotificationSeverity.Error);
        }

        private void Notify(string summary, string? detail, NotificationSeverity severity) => notificationService.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = detail,
            Duration = 6500,
        });

        #endregion
        #endregion
    }
}
