using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Models.Response;
using DWorldProject.Models.ViewModel;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using DWorldProject.Utils.ErrorCodes;
using Microsoft.Extensions.Logging;

namespace DWorldProject.Services
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SectionService(ISectionRepository sectionRepository, ITopicRepository topicRepository,
            IMapper mapper, ILogger<SectionService> logger)
        {
            _sectionRepository = sectionRepository;
            _topicRepository = topicRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public ServiceResult<List<SectionResponseModel>> Get()
        {
            var serviceResult = new ServiceResult<List<SectionResponseModel>>();

            var sections = _sectionRepository.GetAll().Select(t => _mapper.Map<SectionResponseModel>(t)).ToList();
            if (sections.Count() == 0)
            {
                serviceResult.ErrorCode = (int)BaseErrorCodes.NotFound;
                throw new Exception("Exception@Section/Get");
            }

            serviceResult.Data = sections;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

        public ServiceResult<bool> Add(SectionModel model)
        {
            var serviceResult = new ServiceResult<bool>();

            var isSectionExists = _sectionRepository.FindBy(x => x.IsActive && !x.IsDeleted && x.Name.ToLower() == model.Name.ToLower()).Any();
            if (isSectionExists)
            {
                throw new Exception("Section already exists!");
            }

            var section = _mapper.Map<Section>(model);
            if (section == null)
            {
                throw new Exception("Section is null!");
            }

            _sectionRepository.AddWithCommit(section);
            serviceResult.Data = true;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

        public ServiceResult<bool> Edit(SectionModel model)
        {
            var serviceResult = new ServiceResult<bool>();

            if (model.Id == null)
            {
                throw new Exception("Section Id is null!");
            }
            var topicCount = _topicRepository.FindBy(x => x.IsActive && !x.IsDeleted && x.SectionId == model.Id).Count();
            if (topicCount > 0)
            {
                throw new Exception("Section cannot be edited! Remove Topics first.");
            }
            var section = _sectionRepository.GetSingle(x => x.IsActive && !x.IsDeleted && x.Id == model.Id);
            if (section == null)
            {
                throw new Exception("Section not found!");
            }

            section.Name = model.Name;
            section.UpdatedDate = DateTime.Now;
            _sectionRepository.UpdateWithCommit(section);
            serviceResult.Data = true;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

        public ServiceResult<bool> Delete(int id)
        {
            var serviceResult = new ServiceResult<bool>();
            
            var topicCount = _topicRepository.FindBy(x => x.IsActive && !x.IsDeleted && x.SectionId == id).Count();
            if (topicCount > 0)
            {
                throw new Exception("Section cannot be deleted! Remove Topics first.");
            }
            var section = _sectionRepository.GetSingle(x => x.IsActive && !x.IsDeleted && x.Id == id);
            if (section == null)
            {
                throw new Exception("Section not found!");
            }

            section.DeletedDate = DateTime.Now;
            section.IsActive = false;
            section.IsDeleted = true;
            _sectionRepository.UpdateWithCommit(section);
            serviceResult.Data = true;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }
    }
}
