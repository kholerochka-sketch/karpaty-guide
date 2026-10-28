using AutoMapper;
using Karpaty.Api.DBCommunication.Documents;
using Karpaty.Api.Models;
using Karpaty.Api.DTOs;
using Karpaty.Api.DTOs.Place;
namespace Karpaty.Api.Mapping
{
    public class PlaceProfile : Profile
    {
        public PlaceProfile() { 
        CreateMap<PlaceDocument, Place>().ReverseMap();

        CreateMap<CreatePlaceDTO, Place>();

            CreateMap<UpdatePlaceDTO, Place>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
