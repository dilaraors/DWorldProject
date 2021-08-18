using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Models.Response;
using DWorldProject.Models.ViewModel;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using DWorldProject.Utils.ErrorCodes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWorldProject.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TopicService(ITopicRepository topicRepository, ISectionRepository sectionRepository,
            IMapper mapper, ILogger<TopicService> logger)
        {
            _topicRepository = topicRepository;
            _sectionRepository = sectionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public ServiceResult<List<TopicResponseModel>> Get()
        {
            var serviceResult = new ServiceResult<List<TopicResponseModel>>();
            try
            {
                var topics = _topicRepository.FindBy(t => t.IsActive && !t.IsDeleted).Select(t => _mapper.Map<TopicResponseModel>(t)).ToList();
                if (topics.Count() == 0)
                {
                    serviceResult.ErrorCode = (int)BaseErrorCodes.NotFound;
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

        public ServiceResult<TopicResponseModel> GetById(int id)
        {
            var serviceResult = new ServiceResult<TopicResponseModel>();
            var topic = _mapper.Map<TopicResponseModel>(_topicRepository.GetSingle(t => t.IsActive && !t.IsDeleted && t.Id == id));
            if (topic == null)
            {
                serviceResult.ErrorCode = (int)BaseErrorCodes.NotFound;
                throw new Exception("Topic not found!");
            }
            serviceResult.Data = topic;

            return serviceResult;
        }

        public ServiceResult<bool> Add(TopicModel model)
        {
            var serviceResult = new ServiceResult<bool>();
            var section = _sectionRepository.GetSingle(x => x.IsActive && !x.IsDeleted && x.Id == model.SectionId);
            if (section == null)
            {
                throw new Exception("Section not found!");
            }

            var isTopicExists = _topicRepository.FindBy(x => x.IsActive && !x.IsDeleted && x.SectionId == model.SectionId && x.Name.ToLower() == model.Name.ToLower()).Any();
            if (isTopicExists)
            {
                throw new Exception("Topic already exists!");
            }

            var topic = _mapper.Map<Topic>(model);
            if (topic == null)
            {
                throw new Exception("Topic is null!");
            }

            _topicRepository.AddWithCommit(topic);
            section.Topics.Add(topic);
            _sectionRepository.UpdateWithCommit(section);
            serviceResult.Data = true;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

        public ServiceResult<bool> Edit(TopicModel model)
        {
            var serviceResult = new ServiceResult<bool>();

            if (model.Id == null)
            {
                throw new Exception("Topic Id is null!");
            }
            var topic = _topicRepository.GetSingle(x => x.IsActive && !x.IsDeleted && x.Id ==model.Id);
            if (topic == null)
            {
                throw new Exception("Topic is null!");
            }

            topic.Name = model.Name;
            topic.UpdatedDate = DateTime.Now;
            _topicRepository.UpdateWithCommit(topic);
            serviceResult.Data = true;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

        public ServiceResult<bool> Delete(int id)
        {
            var serviceResult = new ServiceResult<bool>();

            var topic = _topicRepository.GetSingle(x => x.IsActive && !x.IsDeleted && x.Id == id);
            if (topic == null)
            {
                throw new Exception("Topic is null!");
            }

            topic.DeletedDate = DateTime.Now;
            topic.IsActive = false;
            topic.IsDeleted = true;
            _topicRepository.UpdateWithCommit(topic);
            serviceResult.Data = true;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }


        public ServiceResult<List<TopicResponseModel>> GetBySectionId(int sectionId)
        {
            var serviceResult = new ServiceResult<List<TopicResponseModel>>();
            var topicList = _mapper.Map<List<TopicResponseModel>>(_topicRepository.FindBy(t => t.IsActive && !t.IsDeleted && t.SectionId == sectionId).ToList());
            if (topicList == null)
            {
                serviceResult.ErrorCode = (int)BaseErrorCodes.NotFound;
                throw new Exception("Topic not found!");
            }
            serviceResult.Data = topicList;

            return serviceResult;
        }
    }
}
