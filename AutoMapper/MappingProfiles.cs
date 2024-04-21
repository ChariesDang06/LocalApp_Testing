using AutoMapper;
using LocalEdu_App.Dto;
using LocalEdu_App.Models;

namespace LocalEdu_App.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<AppUser, AppUserDto>();
            CreateMap<AppUserDto, AppUser>();
            CreateMap<Topic, TopicDto>();
            CreateMap<TopicDto, Topic>();
            CreateMap<Part, PartDto>();
            CreateMap<PartDto, Part>();
            CreateMap<PartType, PartTypeDto>();
            CreateMap<PartTypeDto, PartType>();
            CreateMap<UserProgress, UserProgressDto>();
            CreateMap<UserProgressDto, UserProgress>();
        }
    }
}
