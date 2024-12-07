
using AutoMapper;
using OS.Data.Dtos;
using OS.Data.Models;

namespace OS.Services.Mappers
{
    public class DtoMapper : IDtoMapper
    {
        private Mapper _mapper;
        public DtoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Track, TracksDto>().ForMember(x => x.ArtistIds, opt => opt.MapFrom(x => x.Artists.Select(x => x.Id))).ForMember(x => x.AlbumId, opt => opt.MapFrom(el => el.Album.Id)).ReverseMap();
                cfg.CreateMap<Artist, ArtistsDto>().ReverseMap();
                cfg.CreateMap<Album, AlbumsDto>().ForMember(x => x.ArtistId, opt => opt.MapFrom(el => el.Artist.Id)).ReverseMap();
                cfg.CreateMap<Album, AlbumDto>().ReverseMap();
                cfg.CreateMap<Track, TracksDto>().ReverseMap();
                cfg.CreateMap<Artist, ArtistDto>().ReverseMap();

            });
            _mapper = new Mapper(config);

        }
        public T Map<T>(object source)
        {
            return _mapper.Map<T>(source);
        }
    }
}
