using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Enums.Core;
using Nubetico.Shared.Dto.Core;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.Core.Shared
{
	public partial class DomicilioComponent : NbBaseComponent
	{
		[Parameter]
		public int? Address_ID { get; set; }
		[Parameter]
		public TipoEstadoControl EstadoControl { get; set; }

		public DomicilioDto AddressData = new DomicilioDto();
		private RadzenDropDown<string> StateDropDown {  get; set; }
		private RadzenDropDown<string> CityDropDown { get; set; }
		private RadzenDropDown<string> NeighborhoodDropDown { get; set; }
		private IEnumerable<KeyValuePair<string, string>> StatesList { get; set; } = Enumerable.Empty<KeyValuePair<string, string>>();
		private IEnumerable<TripletValueSAT> CitiesList { get; set; } = Enumerable.Empty<TripletValueSAT>();
		private IEnumerable<TripletValueSAT> NeighborhoodsList { get; set; } = Enumerable.Empty<TripletValueSAT>();
		private CodigoPostalDto ZipCodeData { get; set; } = new CodigoPostalDto();
		private string stateSearchText { get; set; } = "";
		private string citySearchText { get; set; } = "";

		protected override void OnInitialized()
		{
			breakpointService!.OnChange += StateHasChanged;
			base.OnInitialized();
		}

		public void Dispose()
		{
			breakpointService!.OnChange -= StateHasChanged;
		}

		protected override async Task OnInitializedAsync()
		{
			// Get States
			var resultGetResponse = await addressesService.GetEstadosListAsync();
            if (resultGetResponse != null && resultGetResponse.StatusCode == 200)
			{
				StatesList = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(resultGetResponse.Data.ToString());
			}

			resultGetResponse = await addressesService.GetColoniasListAsync();
			if (resultGetResponse != null && resultGetResponse.StatusCode == 200)
			{
				NeighborhoodsList = JsonConvert.DeserializeObject<IEnumerable<TripletValueSAT>>(resultGetResponse.Data.ToString());
			}

			await RefreshDetail();
			await base.OnInitializedAsync();
		}

		private async void OnValueChange(string? value, string field_name)
		{
			switch (field_name)
			{
				case "ZIPCODE":
					// Restore values if there is no ZIP Code
					if (string.IsNullOrEmpty(AddressData.ZipCode))
					{
						AddressData.c_State = null;
						AddressData.c_City = null;
						AddressData.c_Neighborhood = null;
						ZipCodeData = new CodigoPostalDto();

						var resultStates = await addressesService.GetEstadosListAsync();
						if (resultStates != null && resultStates.StatusCode == 200)
						{
							StatesList = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(resultStates.Data.ToString());
						}

						var resultNeighborhoods = await addressesService.GetColoniasListAsync();
						if (resultNeighborhoods != null && resultNeighborhoods.StatusCode == 200)
						{
							NeighborhoodsList = JsonConvert.DeserializeObject<IEnumerable<TripletValueSAT>>(resultNeighborhoods.Data.ToString());
						}

						CitiesList = Enumerable.Empty<TripletValueSAT>();

						StateHasChanged();
						return;
					}

					// Get the state and city by the zipcode
					var resultZipCode = await addressesService.GetCodigoPostalInfoAsync(AddressData.ZipCode);
					if (resultZipCode == null || resultZipCode.StatusCode != 200 || resultZipCode.Data == null)
					{
						AddressData.c_State = null;
						AddressData.c_City = null;
						AddressData.c_Neighborhood = null;
						ZipCodeData = new CodigoPostalDto();

						StatesList = Enumerable.Empty<KeyValuePair<string, string>>();
						CitiesList = Enumerable.Empty<TripletValueSAT>();
						NeighborhoodsList = Enumerable.Empty<TripletValueSAT>();

						StateHasChanged();
						return;
					}

					// Set the state
					ZipCodeData = JsonConvert.DeserializeObject<CodigoPostalDto>(resultZipCode.Data.ToString());
					AddressData.ZipCode = ZipCodeData.CodigoPostal;
					AddressData.c_State = ZipCodeData.c_Estado;
					StatesList = StatesList.Where(state => state.Key == ZipCodeData.c_Estado);
					
					// Get the cities filtered by the state
					var resultCities = await addressesService.GetMunicipiosListAsync(ZipCodeData.c_Estado, ZipCodeData.c_Municipio);
					if (resultCities != null && resultCities.StatusCode == 200)
					{
						CitiesList = JsonConvert.DeserializeObject<IEnumerable<TripletValueSAT>>(resultCities.Data.ToString());
					}
					AddressData.c_City = ZipCodeData.c_Municipio;

					// Get the neighborhoods filtered by the zipcode
					var resultNeighborhood = await addressesService.GetColoniasListAsync(AddressData.ZipCode);
					if (resultNeighborhood != null && resultNeighborhood.StatusCode == 200)
					{
						NeighborhoodsList = JsonConvert.DeserializeObject<IEnumerable<TripletValueSAT>>(resultNeighborhood.Data.ToString());
						
						// Set the first neighborhood
						if (NeighborhoodsList != null && NeighborhoodsList.Any())
						{
							AddressData.c_Neighborhood = NeighborhoodsList.First().ID;
						}
					}
					break;
				case "STATE":
					if (!string.IsNullOrEmpty(AddressData.ZipCode))
					{
						return;
					}

					var resultGetResponse = await addressesService.GetMunicipiosListAsync(AddressData.c_State, AddressData.c_City);
					if (resultGetResponse != null && resultGetResponse.StatusCode == 200)
					{
						CitiesList = JsonConvert.DeserializeObject<IEnumerable<TripletValueSAT>>(resultGetResponse.Data.ToString());
					}
					break;
				case "NEIGHBORHOODSEARCH":
					resultGetResponse = await addressesService.GetColoniasListAsync(AddressData.ZipCode, value);
					if (resultGetResponse != null && resultGetResponse.StatusCode == 200)
					{
						NeighborhoodsList = JsonConvert.DeserializeObject<IEnumerable<TripletValueSAT>>(resultGetResponse.Data.ToString());
					}
					break;
			}

			StateHasChanged();
		}

		private bool CustomValidate(string field_name)
		{
			switch (field_name)
			{
				case "STATE":
					// If it has Zip Code validate the State isn't empty
					return string.IsNullOrEmpty(AddressData.ZipCode) ||
						   (!string.IsNullOrEmpty(AddressData.ZipCode) && !string.IsNullOrEmpty(AddressData.c_State));
				
				case "CITY":
					// If it has Zip Code validate the City isn't empty
					return string.IsNullOrEmpty(AddressData.ZipCode) ||
						   (!string.IsNullOrEmpty(AddressData.ZipCode) && !string.IsNullOrEmpty(AddressData.c_City));

				case "NEIGHBORHOOD":
					// If it has Zip Code validate the Neighborhood isn't empty
					return string.IsNullOrEmpty(AddressData.ZipCode) ||
						   (!string.IsNullOrEmpty(AddressData.ZipCode) && !string.IsNullOrEmpty(AddressData.c_Neighborhood));
				
				default:
					return true;
			}
		}

		private bool GetDisabled(string? field_name = null)
		{
			return EstadoControl == TipoEstadoControl.Lectura;
		}

		public async Task RefreshDetail()
		{
			if (!Address_ID.HasValue)
			{
				return;
			}

			var addressGetResponse = await addressesService.GetDomicilioByID(Address_ID.Value);
			if (addressGetResponse != null && addressGetResponse.Success)
			{
				AddressData = JsonConvert.DeserializeObject<DomicilioDto>(addressGetResponse.Data.ToString());

				// Set State
				StatesList = StatesList.Where(state => state.Key == AddressData.c_State);

				// Get Cities
				var citiesGetResponse = await addressesService.GetMunicipiosListAsync(AddressData.c_State, AddressData.c_City);
				if (citiesGetResponse != null && citiesGetResponse.StatusCode == 200)
				{
					CitiesList = JsonConvert.DeserializeObject<IEnumerable<TripletValueSAT>>(citiesGetResponse.Data.ToString());
				}

				// Get Neighborhoods
				var resultGetResponse = await addressesService.GetColoniasListAsync(AddressData.ZipCode);
				if (resultGetResponse != null && resultGetResponse.StatusCode == 200)
				{
					NeighborhoodsList = JsonConvert.DeserializeObject<IEnumerable<TripletValueSAT>>(resultGetResponse.Data.ToString());
				}

				StateHasChanged();
			}
		}

		public DomicilioResult SaveAddress()
		{
			var result = new DomicilioResult { Success = false };

			// If it has no information, it doesn't return the object
			var properties = AddressData.GetType().GetProperties()
										.Where(p => p.Name != nameof(AddressData.ID) && p.Name != nameof(AddressData.IsInvoincing));

			if (properties.All(p => p.GetValue(AddressData) == null))
			{
				result.Success = true;
				return result;
			}

			// If it has information, validate the zipcode
			if (!string.IsNullOrEmpty(AddressData.ZipCode))
			{
				// Validate the length
				if (AddressData.ZipCode.Length != 5)
				{
					result.ErrorMessage = Localizer["Address.Error.LengthZipcode"];
					return result;
				}

				// Validate that the State field isn't empty
				if (string.IsNullOrEmpty(AddressData.c_State))
				{
					result.ErrorMessage = $"{Localizer["Subdivisions.Text.Required"]} {Localizer["Address.Text.State"]}";
					return result;
				}

				// Validate that the City field isn't empty
				if (string.IsNullOrEmpty(AddressData.c_City))
				{
					result.ErrorMessage = $"{Localizer["Subdivisions.Text.Required"]} {Localizer["Address.Text.City"]}";
					return result;
				}

				// Validate that the Neighborhood field isn't empty
				if (string.IsNullOrEmpty(AddressData.c_Neighborhood))
				{
					result.ErrorMessage = $"{Localizer["Subdivisions.Text.Required"]} {Localizer["Address.Text.Neighborhood"]}";
					return result;
				}
			}

			result.Success = true;
			result.Result = AddressData;
			return result;
		}

		public int GetColumnsSize(string? field_name = null)
		{
			var breakpoint = breakpointService.GetCurrentBreakpoint();
			if (field_name == "BETWEENSTREET")
			{
				switch (breakpoint)
				{
					case Breakpoint.Xs:
						return 12;
					case Breakpoint.Sm:
					case Breakpoint.Md:
						return 6;
					default:
						return 5;
				}
			}
			else
			{
				switch (breakpoint)
				{
					case Breakpoint.Xs:
						return 12;
					case Breakpoint.Sm:
					case Breakpoint.Md:
						return 6;
					default:
						return 4;
				}
			}
		}
	}
}
