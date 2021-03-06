using AutoMapper;

namespace HtmlRaportGenerator.Tools.Mapper
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile)
            => profile.CreateMap(typeof(T), GetType());
    }
}
