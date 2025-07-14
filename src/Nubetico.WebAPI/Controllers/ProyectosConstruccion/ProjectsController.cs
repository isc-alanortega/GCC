using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalProveedores;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.Projects;
using Nubetico.WebAPI.Filters;
using FluentValidation;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [ApiController]
    [Authorize]
    [Route("api/v1/proyectosconstruccion/proyectos")]
    [TypeFilter(typeof(ExceptionFilter))]
    public class ProjectsController : Controller
    {
        #region GET
        /// <summary>
        /// Retrieves the form data required.
        /// </summary>
        /// <param name="projectService">The service used to fetch the project form options.</param>
        /// <returns>Returns a response containing project form data.</returns>
        [HttpGet("form")]
        [Produces("application/json")]
        //[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ProjectFormDataDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetProjectsFormData([FromServices] ProjectsService projectService)
        {
            var response = await projectService.FetchProjectFormOptionsAsync();
            if (!response.Success || response.Result == null)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(statusCode: StatusCodes.Status400BadRequest, message: response.Message ?? "OCurrio un problema al recupararlos datos de la lista"));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<ProjectFormDataDto>(StatusCodes.Status200OK, data: response.Result));
        }

        /// <summary>
        ///  Retrieves project data filtered by a specific project GUID.
        ///  This endpoint fetches details of a project based on the provided GUID.
        /// </summary>
        /// <param name="projectService">The service used to fetch the filtered project data.</param>
        /// <param name="projectGuid">The GUID of the project to be retrieved.</param>
        /// <returns>Returns the project data if found, or a 400 Bad Request if the project is not found.</returns>
        /// <response code="200">Returns the project data successfully filtered by the project GUID.</response>
        /// <response code="400">Returns an error message if the project is not found or invalid.</response>
        /// <response code="500">Returns a generic error message in case of an internal server error.</response>
        [HttpGet("filter")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ProjectDataDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetFilterProject(
            [FromServices] ProjectsService projectService,
            [FromQuery] Guid projectGuid
        )
        {           
            var response = await projectService.GetOneFilterProjectAsync(projectGuid);
            if (!response.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: "Shared.Texts.ProjectNotFound"));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<ProjectDataDto>(StatusCodes.Status200OK, data: response.Result));
        }
        #endregion

        #region POST
        /// <summary>
        /// Retrieves a paginated list of projects based on the specified filter parameters.
        /// This endpoint allows clients to fetch projects in a paginated format, applying filters such as 
        /// project name, business unit, subdivision, and status.
        /// </summary>
        /// <param name="request">The request body containing the pagination details and filter parameters 
        /// (e.g., limit, offset, filters).</param>
        /// <returns>
        /// A paginated response containing a list of projects. The response will include the total number of records, 
        /// the filtered records, and data in a paginated format. 
        /// The response will return a 400 BadRequest if the parameters are invalid or if the operation fails.
        /// </returns>
        [HttpPost("paginado")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<ProyectsGridDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetPaginatedProjects(
            [FromServices] ProjectsService projectService,
            [FromBody] ProjectsPaginatedRequestDto request
        )
        {
            if (request.Limit < 0 || request.OffSet < 0)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: "Shared.Texts.BadLimits"));

            var response = await projectService.GetProjectsFiltersAsync(request);
            if (response == null || !response.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: response?.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<PaginatedListDto<ProyectsGridDto>>(StatusCodes.Status200OK, data: response.Result));
        }

        /// <summary>
        /// Adds a new project to the system. This endpoint receives the project data, validates it, 
        /// and creates a new project. If validation fails or an error occurs during the project creation, 
        /// an appropriate error response will be returned.
        /// </summary>
        /// <param name="validator">The validator used to validate the project data.</param>
        /// <param name="projectService">The service responsible for adding the project to the database.</param>
        /// <param name="request">The project data to be added. This includes details like project name, 
        /// business unit, and other relevant information.</param>
        /// <returns>
        /// Returns a 201 Created status with the newly created project data if successful. 
        /// Returns a 400 BadRequest if validation fails or if there is any error during project creation. 
        /// In case of a server error, a 500 InternalServerError will be returned.
        /// </returns> 
        /// <response code="201">Created status with the newly created project data if successful.</response>
        /// <response code="400">BadRequest if validation fails or if there is any error during project creation.</response>
        /// <response code="500">In case of a server error, a 500 InternalServerError will be returned.</response> 
        [HttpPost("add")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<PaginatedListDto<ObraDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<ProjectDataDto?>))]
        public async Task<IActionResult> PostAddProject(
            [FromServices] IValidator<ProjectDataDto> validator,
            [FromServices] ProjectsService projectService,
            [FromBody] ProjectDataDto request
        )
        {
            var validate = await validator.ValidateAsync(request);
            if (!validate.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response(StatusCodes.Status400BadRequest, validate.Errors));

            var response = await projectService.AddProjectAsync(request);
            if(!response!.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: response.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<ProjectDataDto?>(StatusCodes.Status200OK, data: response.Result));
        }
        #endregion

        #region PATCH
        /// <summary>
        /// Updates an existing project in the system. This endpoint receives the updated project data, validates it, 
        /// and attempts to update the project in the system. If validation fails or there is an error during the 
        /// update process, an appropriate error response will be returned.
        /// </summary>
        /// <param name="validator">The validator used to validate the updated project data.</param>
        /// <param name="projectService">The service responsible for updating the project in the database.</param>
        /// <param name="request">The updated project data to be applied. This includes details such as project name, 
        /// status, business unit, and other updated fields.</param>
        /// <returns>
        /// Returns a 200 OK status with the updated project data if successful. 
        /// Returns a 400 BadRequest if validation fails or if there is any error during the update process. 
        /// In case of a server error, a 500 InternalServerError will be returned.
        /// </returns>
        [HttpPatch("update")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ProjectDataDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PatchUpdateProject(
            [FromServices] IValidator<ProjectDataDto> validator,
            [FromServices] ProjectsService projectService,
            [FromBody] ProjectDataDto request
        )
        {
            var validate = await validator.ValidateAsync(request);
            if (!validate.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response(StatusCodes.Status400BadRequest, validate.Errors));

            var response = await projectService.UpdateProjectAsync(request);
            if(!response.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: response.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, data: response));
        }

        /// <summary>
        /// Marks a project as deleted by updating its status. This endpoint receives the project data,
        /// validates it, and performs a "soft delete" by updating the project's status. If validation fails 
        /// or there is an error during the update process, an appropriate error response will be returned.
        /// </summary>
        /// <param name="validator">The validator used to validate the project data before deletion.</param>
        /// <param name="projectService">The service responsible for updating the project status in the database to mark it as deleted.</param>
        /// <param name="request">The project data containing the project ID and other relevant details to identify the project to be deleted.</param>
        /// <returns>
        /// Returns a 200 OK status if the project deletion (soft delete) was successful. 
        /// Returns a 400 BadRequest if the validation fails or if any issues occur during the deletion process.
        /// In case of a server error, a 500 InternalServerError will be returned.
        /// </returns>
        [HttpPatch("delte")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<ObraDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PatchDeleteProject(
            [FromServices] IValidator<ProjectDataDto> validator,
            [FromServices] ProjectsService projectService,
            [FromQuery] Guid projectGuid
        )
        {
            var response = await projectService.DeleteProjectAsync(projectGuid);
            if(!response.Success)
                return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, data: response));
        }
        #endregion
    }
}
