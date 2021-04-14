using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DWorldProject.Enums;
using DWorldProject.Models.Response;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using DWorldProject.Utils.ErrorCodes;
using Microsoft.Extensions.Logging;

namespace DWorldProject.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TopicService(ITopicRepository topicRepository, IMapper mapper, ILogger<TopicService> logger)
        {
            _topicRepository = topicRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public ServiceResult<List<TopicResponseModel>> Get()
        {
            var serviceResult = new ServiceResult<List<TopicResponseModel>>();
            try
            {
                var topics = _topicRepository.GetAll().Select(t => _mapper.Map<TopicResponseModel>(t)).ToList();
                if (topics.Count() == 0)
                {
                    serviceResult.ErrorCode = (int)TopicErrorCodes.NoTopicFound;
                    throw new Exception("Exception@Topic/GetTopics");
                }

                serviceResult.Data = topics;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = e.Message;
            }

            return serviceResult;
        }

        public ServiceResult<List<string>> GetSections()
        {
            var serviceResult = new ServiceResult<List<string>>();
            try
            {
                var sections =Enum.GetNames(typeof(Section)).ToList();

                serviceResult.Data = sections;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = e.Message;
            }

            return serviceResult;
        }

        public ServiceResult<TopicResponseModel> GetById(int id)
        {
            var serviceResult = new ServiceResult<TopicResponseModel>();
            try
            {
                var topic = _mapper.Map<TopicResponseModel>(_topicRepository.GetSingle(t => t.IsActive && !t.IsDeleted && t.Id == id));
                if (topic == null)
                {
                    serviceResult.ErrorCode = (int)TopicErrorCodes.NoTopicFound;
                    throw new Exception("Exception@Topic/GetTopicById");
                }

                serviceResult.Data = topic;
            }
            catch (Exception e)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = e.Message;
            }

            return serviceResult;
        }
    }
}
