using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Dialogs.ProyectosConstruccion;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class SubdivisionsDetComponent : NbBaseComponent
	{
		[Parameter] 
		public string? SubdivisionGuid { get; set; }
		[Parameter] 
		public SubdivisionsCatComponent? ParentCatalog {  get; set; }

		private RadzenTemplateForm<SubdivisionsDto> GeneralSubdivisionForm { get; set; }
		private SubdivisionsDto SubdivisionData { get; set; } = new SubdivisionsDto();
		private RadzenDataGrid<SubdivisionStage>? GridStages { get; set; }
		private RadzenDataGrid<SubdivisionBlock>? GridBlocks { get; set; }
		private string LogoPreview { get; set; }
		private bool IsLoading { get; set; }
		private int Count { get; set; }
		private int SelectedTabIndex = 0;
		private IList<SubdivisionStage> SelectedStage { get; set; } = new List<SubdivisionStage>();
		private IList<SubdivisionBlock> SelectedBlock { get; set; } = new List<SubdivisionBlock>();

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			await RefreshDetail();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender && EstadoControl == TipoEstadoControl.Alta)
			{
				GeneralSubdivisionForm.EditContext.Validate();
			}
		}

		protected override List<RadzenMenuItem> GetMenuItems()
		{
			var baseMenu = MenuItemsFactory.GetBaseMenuItems(Localizer);
			var displayedMenu = new List<RadzenMenuItem>();

			foreach (var option in baseMenu)
			{
				bool command = option.Attributes.TryGetValue("comando", out var commandValue);

				switch (commandValue)
				{
					case "editar":
						// Show only if the status is read
						if (EstadoControl == TipoEstadoControl.Lectura)
						{
							// Edit the subdivision
							option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit);
							displayedMenu.Add(option);
						}
						break;
					case "cerrar":
						option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose);
						displayedMenu.Add(option);
						break;
					case "guardar":
						// Show only if the status is different from read
						if (EstadoControl != TipoEstadoControl.Lectura)
						{
							option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSubmit);
							displayedMenu.Add(option);
						}
						break;
					case "cancelar":
						// Show only if the status is edit
						if (EstadoControl == TipoEstadoControl.Edicion)
						{
							option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickCancel);
							displayedMenu.Add(option);
						}
						break;
				}
			}

			return displayedMenu;
		}

		private void OnTabChange(int tabIndex)
		{
			SelectedTabIndex = tabIndex;

			TriggerMenuUpdate();
		}

		private void OnErrorLogo(UploadErrorEventArgs args)
		{
			ShowErrorNotification("Subdivisions.Error.UploadImage");
		}

		#region METHODS ONCLICK
		private void OnClickEdit()
		{
			EstadoControl = TipoEstadoControl.Edicion;
			SetNombreTabNubetico($"{Localizer["Subdivisions.Text.Subdivision"]} [{SubdivisionData.Subdivision}]");
			TriggerMenuUpdate();
		}

		private void OnClickClose()
		{
			CerrarTabNubetico();
		}

		private async void OnClickSubmit()
		{
			if (!GeneralSubdivisionForm.EditContext.Validate())
			{
				return;
			}

			var submitResponse = EstadoControl == TipoEstadoControl.Alta
				? await subdivisionsService.PostSubdivision(SubdivisionData)
				: await subdivisionsService.UpdateSubdivision(SubdivisionData);

			if (submitResponse != null && (submitResponse.StatusCode == 201 || submitResponse.StatusCode == 200))
			{
				if (submitResponse.StatusCode == 201 && !string.IsNullOrEmpty(submitResponse.Message))
				{
					SubdivisionGuid = submitResponse.Message;
					await RefreshDetail();
				}
				ShowInfoNotification("Subdivisions.Saved.Subdivision");

				EstadoControl = TipoEstadoControl.Lectura;
				SetNombreTabNubetico($"{Localizer["Subdivisions.Text.Subdivision"]} [{SubdivisionData.Subdivision}]");
			}
			else
			{
				ShowErrorNotification("Subdivisions.Error.SaveSubdivision");
			}

			// Refresh the Subdivisions Catalog
			if (ParentCatalog != null)
			{
				var args = new LoadDataArgs { Top = 20, Skip = 0 };
				await ParentCatalog.LoadData(args);
			}

			TriggerMenuUpdate();
			StateHasChanged();
		}

		private async void OnClickCancel()
		{
			await RefreshDetail();

			EstadoControl = TipoEstadoControl.Lectura;
			SetNombreTabNubetico($"{Localizer["Subdivisions.Text.Subdivision"]} [{SubdivisionData.Subdivision}]");
			TriggerMenuUpdate();
			StateHasChanged();
		}

		private void OnClickAddLogo()
		{

		}

		private void OnClickDeleteLogo()
		{
			
		}

		private async void OnClickAddStage()
		{
			var dialogResult = await dialogService.OpenAsync<DetailSubdivisionDialogComponent>
			(
				$"{Localizer["Subdivisions.Text.New"]} {Localizer["Subdivisions.Text.Stage"]}",
				new Dictionary<string, object>()
				{
					{ "EstadoControl", TipoEstadoControl.Alta },
					{ "InvalidNames", SubdivisionData.Stages == null ? Enumerable.Empty<string>() : SubdivisionData.Stages.Select(s => s.Stage) }
				}
			);

			if (dialogResult == null)
			{
				return;
			}
			
			// Create the new Stage
			var result = dialogResult as GeneralDetailSubdivision;
			int lastSequence = SubdivisionData.Stages != null && SubdivisionData.Stages.Any()
				? SubdivisionData.Stages.OrderBy(stage => stage.Sequence).Select(stage => stage.Sequence).Last()
				: 0;

			var newStage = new SubdivisionStage
			{
				ID_Subdivision = SubdivisionData.ID,
				Stage = result.Name,
				Description = result.Description,
				Sequence = lastSequence + 1
			};

			if (newStage.ID == 0)
			{
				newStage.ID = SubdivisionData.Stages == null || !SubdivisionData.Stages.Any() 
					? 1
					: SubdivisionData.Stages.OrderByDescending(s => s.ID).Select(s => s.ID).First() + 1;
			}

			// Add the new Stage to the list
			var stages = SubdivisionData.Stages != null && SubdivisionData.Stages.Any() ? SubdivisionData.Stages.ToList() : new List<SubdivisionStage>();
			stages.Add(newStage);
			SubdivisionData.Stages = stages;

			StateHasChanged();
		}

		private async void OnClickAddBlock()
		{
			if (!SelectedStage.Any())
			{
				return;
			}

			var selectedStage = SelectedStage.First();
			var dialogResult = await dialogService.OpenAsync<DetailSubdivisionDialogComponent>
			(
				$"{Localizer["Subdivisions.Text.New"]} {Localizer["Subdivisions.Text.Block"]}",
				new Dictionary<string, object>()
				{
					{ "EstadoControl", TipoEstadoControl.Alta },
					{ "InvalidNames", selectedStage.Blocks == null ? Enumerable.Empty<string>() : selectedStage.Blocks.Select(s => s.Block) },
					{ "ShowDescription", false }
				}
			);

			if (dialogResult == null)
			{
				return;
			}

			// Create the new Block
			var result = dialogResult as GeneralDetailSubdivision;
			var newBlock = new SubdivisionBlock
			{
				ID_Subdivision = SubdivisionData.ID,
				ID_Subdivision_Stage = selectedStage.ID,
				Block = result.Name
			};

			// Add the new Block to the list
			var blocks = SubdivisionData.Stages.First(stage => stage.ID == selectedStage.ID).Blocks != null && SubdivisionData.Stages.First(stage => stage.ID == selectedStage.ID).Blocks.Any()
				? SubdivisionData.Stages.First(stage => stage.ID == selectedStage.ID).Blocks.ToList()
				: new List<SubdivisionBlock>();
			blocks.Add(newBlock);
			SubdivisionData.Stages.First(stage => stage.ID == selectedStage.ID).Blocks = blocks;

			StateHasChanged();
		}

		public void OnClickUpperSequence(SubdivisionStage ClickStage)
		{
			SelectedStage = new List<SubdivisionStage> { ClickStage };
			
			var changeSequence = SubdivisionData.Stages.OrderByDescending(stage => stage.Sequence)
													   .FirstOrDefault(stage => stage.Sequence < SelectedStage.First().Sequence);
			
			if (changeSequence == null)
			{
				return;
			}

			// Change the sequences
			var selectedStage = SelectedStage.First();
			var tempSequence = selectedStage.Sequence;

			selectedStage.Sequence = changeSequence.Sequence;
			changeSequence.Sequence = tempSequence;

			// Order Stages
			SubdivisionData.Stages = SubdivisionData.Stages.OrderBy(stage => stage.Sequence);

			StateHasChanged();
		}

		public void OnClickDownSequence(SubdivisionStage ClickStage)
		{
			SelectedStage = new List<SubdivisionStage> { ClickStage };
			
			var changeSequence = SubdivisionData.Stages.OrderBy(stage => stage.Sequence).FirstOrDefault(stage => stage.Sequence > SelectedStage.First().Sequence);

			if (changeSequence == null)
			{
				return;
			}

			// Change the sequences
			var selectedStage = SelectedStage.First();
			var tempSequence = selectedStage.Sequence;

			selectedStage.Sequence = changeSequence.Sequence;
			changeSequence.Sequence = tempSequence;

			// Order Stages
			SubdivisionData.Stages = SubdivisionData.Stages.OrderBy(stage => stage.Sequence);

			StateHasChanged();
		}

		private async void OnClickEditStage(SubdivisionStage ClickStage)
		{
			SelectedStage = new List<SubdivisionStage> { ClickStage };

			var selectedStage = SelectedStage.First();
			var dialogResult = await dialogService.OpenAsync<DetailSubdivisionDialogComponent>
			(
				$"{Localizer["Shared.Comandos.Editar"]} {Localizer["Subdivisions.Text.Stage"]}",
				new Dictionary<string, object>()
				{
					{ "EstadoControl", TipoEstadoControl.Edicion },
					{ "DetailData", new GeneralDetailSubdivision
						{
							Name = selectedStage.Stage,
							Description = selectedStage.Description
						}
					},
					{ "InvalidNames",  SubdivisionData.Stages == null
						? Enumerable.Empty<string>()
						: SubdivisionData.Stages.Where(s => s.Stage != selectedStage.Stage).Select(s => s.Stage)
					}
				}
			);

			if (dialogResult == null)
			{
				return;
			}

			// Get the edited Stage
			var result = dialogResult as GeneralDetailSubdivision;
			selectedStage.Stage = result.Name;
			selectedStage.Description = result.Description;

			StateHasChanged();
		}

		private async void OnClickEditBlock(SubdivisionBlock ClickBlock)
		{
			if (!SelectedStage.Any())
			{
				return;
			}

			SelectedBlock = new List<SubdivisionBlock> { ClickBlock };

			var selectedBlock = SelectedBlock.First();
			var dialogResult = await dialogService.OpenAsync<DetailSubdivisionDialogComponent>
			(
				$"{Localizer["Shared.Comandos.Editar"]} {Localizer["Subdivisions.Text.Block"]}",
				new Dictionary<string, object>()
				{
					{ "EstadoControl", TipoEstadoControl.Edicion },
					{ "DetailData", new GeneralDetailSubdivision { Name = selectedBlock.Block } },
					{ "InvalidNames",  SelectedStage.First().Blocks == null
						? Enumerable.Empty<string>()
						: SelectedStage.First().Blocks.Where(b => b.Block != selectedBlock.Block).Select(b => b.Block)
					},
					{ "ShowDescription", false }
				}
			);

			if (dialogResult == null)
			{
				return;
			}

			// Get the edited Stage
			var result = dialogResult as GeneralDetailSubdivision;
			selectedBlock.Block = result.Name;

			StateHasChanged();
		}

		private async void OnClickRemoveStage(SubdivisionStage ClickStage)
		{
			SelectedStage = new List<SubdivisionStage> { ClickStage };

			string message = (SelectedStage.First().Blocks != null && SelectedStage.First().Blocks.Any())
				? $"{Localizer["Shared.Text.WarningRemoveBlockQuestion"]} \n {Localizer["Subdivisions.Warning.RemoveBlocks"]}"
				: Localizer["Shared.Text.WarningRemoveBlockQuestion"];

            // Show a warning if the stage has blocks
            bool dialogResult = await ShowWarningDialog(message) ?? false;
            if (!dialogResult)
            {
                return;
            }


			bool isStageInLot = await CheckStageInLotsAsync();
			if (isStageInLot) return;
            

            var stages = SubdivisionData.Stages.ToList();
			stages.Remove(SelectedStage.First());
			SubdivisionData.Stages = stages;
			SelectedStage = new List<SubdivisionStage>();

			StateHasChanged();
		}

		private async Task<bool> CheckStageInLotsAsync()
		{
			if (this.EstadoControl != TipoEstadoControl.Edicion) return false;

            var response = await lotService.CheckStageInLotsByIdAsync(SelectedStage.First().ID);
            string checkStageInLotMessage = (response.Data is null || !response.Success || response.StatusCode > 300 || response.Data is null)
                ? "Ocurrio un error desconocido, contacte con soperte"
                : response.Data ?? false
                    ? "No es posible eliminar la etapa, esta ya se encuentra en un lote"
                    : string.Empty;

			bool isStageInLot = checkStageInLotMessage != string.Empty;
			if(isStageInLot)
            {
                ShowErrorNotification(checkStageInLotMessage);
            }

			return isStageInLot;
        }

		private async void OnClickRemoveBlock(SubdivisionBlock ClickBlock)
		{
			SelectedBlock = new List<SubdivisionBlock> { ClickBlock };
            bool dialogResult = await ShowWarningDialog("Shared.Text.WarningRemoveManzanas") ?? false;
            if (!dialogResult)
            {
                return;
            }

            bool isBlockInLot = await CheckBlockInLotsAsync();
            if (isBlockInLot) return;

            var blocks = SubdivisionData.Stages.First(stage => stage.ID == SelectedBlock.First().ID_Subdivision_Stage).Blocks.ToList();
			blocks.Remove(SelectedBlock.First());
			SubdivisionData.Stages.First(stage => stage.ID == SelectedBlock.First().ID_Subdivision_Stage).Blocks = blocks;
			SelectedBlock = new List<SubdivisionBlock>();

			StateHasChanged();
		}

        private async Task<bool> CheckBlockInLotsAsync()
        {
            if (this.EstadoControl != TipoEstadoControl.Edicion) return false;

            var response = await lotService.CheckBlockInLotsByIdAsync(SelectedBlock.First().ID);
            string checkStageInLotMessage = (response is null || response?.Data is null || !response.Success || response.StatusCode > 300 || response.Data is null)
                ? "Ocurrio un error desconocido, contacte con soperte"
                : response.Data ?? false
                    ? "No es posible eliminar la manzana, esta ya se encuentra en un lote"
                    : string.Empty;

            bool isBlockInLot = checkStageInLotMessage != string.Empty;
            if (isBlockInLot)
            {
                ShowErrorNotification(checkStageInLotMessage);
            }

            return isBlockInLot;
        }
        #endregion

        private void OnCompleteUpload(UploadCompleteEventArgs? value)
		{
			var newvalue = value;
		}

		private IEnumerable<SubdivisionBlock>? GetSelectedStageBlocks()
		{
			if (SubdivisionData == null || !SelectedStage.Any())
			{
				return Enumerable.Empty<SubdivisionBlock>();
			}

			return SubdivisionData.Stages.First(stage => stage.ID == SelectedStage.First().ID).Blocks;
		}

		private bool GetDisabled()
		{
			return EstadoControl == TipoEstadoControl.Lectura;
		}

		private bool GetAddBlockDisabled()
		{
			return !(SelectedStage.Any() && EstadoControl != TipoEstadoControl.Lectura);
		}

		private async Task RefreshDetail()
		{
			if (string.IsNullOrEmpty(SubdivisionGuid))
			{
				return;
			}

			var subdivisionGetResponse = await subdivisionsService.GetSubdivisionByGuid(SubdivisionGuid);
			if (subdivisionGetResponse == null || !subdivisionGetResponse.Success)
			{
				CerrarTabNubetico();
				return;
			}

			SubdivisionData = JsonConvert.DeserializeObject<SubdivisionsDto>(subdivisionGetResponse.Data.ToString());
			SelectedStage = new List<SubdivisionStage>();
			SelectedBlock = new List<SubdivisionBlock>();
		}

		private bool ValidateValues()
		{
			if (SubdivisionData == null)
			{
				return false;
			}

			if (string.IsNullOrEmpty(SubdivisionData.Subdivision))
			{
				return false;
			}

			if (SubdivisionData.PostalCode == 0)
			{
				return false;
			}

			return true;
		}

		private void ShowInfoNotification(string message)
		{
			var notification = new NotificationMessage
			{
				Severity = NotificationSeverity.Info,
				Summary = Localizer["Subdivisions.Notify.Info"],
				Detail = Localizer[message],
				Duration = 3500
			};

			notificationService.Notify(notification);
		}

		private void ShowErrorNotification(string message)
		{
			var notification = new NotificationMessage
			{
				Severity = NotificationSeverity.Error,
				Summary = "Error",
				Detail = Localizer[message],
				Duration = 3500
			};

			notificationService.Notify(notification);
		}

		private async Task<bool?> ShowWarningDialog(string message)
		{
			var dialogResult = await dialogService.Confirm
			(
                Localizer[message],
				Localizer["Subdivisions.Dialog.Warning"],
				new ConfirmOptions() { OkButtonText = Localizer["Shared.Botones.Aceptar"], CancelButtonText = Localizer["Shared.Botones.Cancelar"] }
			);

			return dialogResult;
		}
	}
}
