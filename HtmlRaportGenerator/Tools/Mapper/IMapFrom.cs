using AutoMapper;

namespace HtmlRaportGenerator.Tools.Mapper
{
    //todo source generator for creating tests? Or Just test all mappings
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile)
            => profile.CreateMap(typeof(T), GetType());
    }
}
