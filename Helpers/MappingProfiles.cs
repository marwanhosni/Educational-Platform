using AutoMapper;
using Educational_Platform.Core.Dtos.Auth;
using Educational_Platform.Core.Dtos.Groups;
using Educational_Platform.Core.Dtos.Registrations;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.EnumGrade;

namespace Educational_Platform.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<RegisterStudentDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<RegisterStudentDto, StudentProfile>()
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade))
            .ForMember(dest => dest.StudentType, opt => opt.MapFrom(src => src.StudentType))
            .ForMember(dest => dest.ParentPhone, opt => opt.MapFrom(src => src.ParentPhone))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StudentStatus.Pending));

        CreateMap<ApplicationUser, AuthResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? ""))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        //Groups

        CreateMap<CreateGroupDto, Group>();
        CreateMap<UpdateGroupDto, Group>();
        CreateMap<Group, GroupResponseDto>()
            .ForMember(dest => dest.CurrentStudentsCount, opt => opt.MapFrom(src => src.Students.Count(s => s.Status == StudentStatus.Approved)));
        CreateMap<Group, GroupDetailsDto>()
            .ForMember(dest => dest.CurrentStudentsCount, opt => opt.MapFrom(src => src.Students.Count(s => s.Status == StudentStatus.Approved)))
            .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.Students));
        CreateMap<StudentProfile, GroupStudentDto>()
            .ForMember(dest => dest.StudentProfileId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber ?? ""))
            .ForMember(dest => dest.ParentPhone, opt => opt.MapFrom(src => src.ParentPhone))
            .ForMember(dest => dest.ApprovedAt, opt => opt.MapFrom(src => src.ApprovedAt));

        // Pending
        CreateMap<StudentProfile, PendingStudentDto>()
            .ForMember(dest => dest.StudentProfileId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email ?? ""))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber ?? ""))
            .ForMember(dest => dest.ParentPhone, opt => opt.MapFrom(src => src.ParentPhone))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}
