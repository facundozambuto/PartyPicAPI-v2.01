using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Logger;
using PartyPic.Models.Common;
using System;
using System.Collections.Generic;
namespace PartyPic.Controllers
{
    
    public class PartyPicControllerBase : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;

        public PartyPicControllerBase(IMapper mapper, IConfiguration config, ILoggerManager logger)
        {
            _mapper = mapper;
            _config = config;
            _logger = logger;
        }

        protected ActionResult ExecuteMethod<T, TApiOutput, TOutput>(Func<TOutput> method, Action validate = null)
            where TApiOutput : ApiResponse
        {
            return ExecuteAction<T, ActionResult>(() =>
            {
                validate?.Invoke();
                var coreData = method();
                var apiResponse = _mapper.Map<TApiOutput>(coreData);
                apiResponse.Success = true;
                return Ok(apiResponse);
            });
        }

        private ActionResult ExecuteAction<T, TOutput>(Func<TOutput> action)
            where TOutput : ActionResult
        {
            try
            {
                var result = action();
                return result;
            }
            catch (Exception ex)
            {
                return ProcessError<T>(ex, ex.Message);
            }
        }

        protected ActionResult ExecuteMethod<T>(Action method)
        {
            return ExecuteAction<T, ActionResult>(() =>
            {
                method();
                var response = new ApiResponse
                {
                    Success = true
                };
                return Ok(response);
            });
        }

        protected ActionResult ProcessError<T>(Exception exception, string errorMessage)
        {
            _logger.LogError(errorMessage, exception);

            var errorCode = exception.GetType().Name;

            var response = new ApiResponse
            {
                Errors = new List<Error>
                {
                    new Error(errorCode)
                }
            };

            if (_config.GetValue<bool>("ShowDetailedErrorsInResponse"))
            {
                response.Errors.Add(new Error(exception.Message));
                response.Errors.Add(new Error(exception.StackTrace));
            }

            return BadRequest(response);
        }
    }
}
