using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OreoPOS.Core.DataLayer;
using OreoPOS.Core.EntityLayer;
using OreoPOS.Responses;
using OreoPOS.ViewModels;
using AutoMapper;
using System.Data.Entity;
namespace OreoPOS.Controllers
{
    [Produces("application/json")]
    [Route("api/Area")]
    public class AreaController : Controller
    {
        private readonly IAreaRepository _areaRepository;
        public AreaController(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }
        protected override void Dispose(Boolean disposing)
        {
            _areaRepository?.Dispose();

            base.Dispose(disposing);
        }

        // GET Areaion/Area
        /// <summary>
        /// Retrieves a list of Areas
        /// </summary>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="name">Name</param>
        /// <returns>List response</returns>
        [HttpGet]
        [Route("Area")]
        public async Task<IActionResult> GetAreas(Int32? pageSize = 10, Int32? pageNumber = 1, String name = null)
        {
            var response = new ListModelResponse<AreaViewModel>();

            try
            {
                response.PageSize = (Int32)pageSize;
                response.PageNumber = (Int32)pageNumber;

                response.Model = await _areaRepository
                        .GetAreas(response.PageSize, response.PageNumber, name)
                        .Select(item => Mapper.Map<AreaViewModel>(item))
                        .ToListAsync();

                response.Message = String.Format("Total of records: {0}", response.Model.Count());
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        // GET Areaion/Area/5
        /// <summary>
        /// Retrieves a specific Area by id
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Single response</returns>
        [HttpGet]
        [Route("Area/{id}")]
        public async Task<IActionResult> GetAreaAsync(Int32 id)
        {
            var response = new SingleModelResponse<AreaViewModel>();

            try
            {
                var entity = await _areaRepository.GetAreaAsync(new Area { Id = id });

                response.Model = Mapper.Map<AreaViewModel>(entity); //entity?.ToViewModel();
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        // POST Areaion/Area/
        /// <summary>
        /// Creates a new Area on Areaion catalog
        /// </summary>
        /// <param name="request">Area entry</param>
        /// <returns>Single response</returns>
        [HttpPost]
        [Route("Area")]
        public async Task<IActionResult> PostAreaAsync([FromBody]AreaViewModel request)
        {
            var response = new SingleModelResponse<AreaViewModel>();

            try
            {
                var entity = await _areaRepository.AddAreaAsync(Mapper.Map<Area>(request));

                response.Model = Mapper.Map<AreaViewModel>(entity);// entity?.ToViewModel();
                response.Message = "The data was saved successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.ToString();
            }

            return response.ToHttpResponse();
        }

        // PUT Areaion/Area/5
        /// <summary>
        /// Updates an existing Area
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <param name="request">Area entry</param>
        /// <returns>Single response</returns>
        [HttpPut]
        [Route("Area/{id}")]
        public async Task<IActionResult> PutAreaAsync(Int32 id, [FromBody]AreaViewModel request)
        {
            var response = new SingleModelResponse<AreaViewModel>();

            try
            {
                var entity = await _areaRepository.UpdateAreaAsync(Mapper.Map<Area>(request));//request.ToEntity());

                response.Model = Mapper.Map<AreaViewModel>(entity);// entity?.ToViewModel();
                response.Message = "The record was updated successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        // DELETE Areaion/Area/5
        /// <summary>
        /// Delete an existing Area
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Single response</returns>
        [HttpDelete]
        [Route("Area/{id}")]
        public async Task<IActionResult> DeleteAreaAsync(Int32 id)
        {
            var response = new SingleModelResponse<AreaViewModel>();

            try
            {
                var entity = await _areaRepository.DeleteAreaAsync(new Area { Id = id });

                response.Model = Mapper.Map<AreaViewModel>(entity); //entity?.ToViewModel();
                response.Message = "The record was deleted successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }
    }
}