using AutoMapper;
using GameTracker.Application.DTOs;
using GameTracker.Domain.Entities;

namespace GameTracker.Application.Mapping
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<CreateGameDto, Game>();
            CreateMap<UpdateGameDto, Game>();

            CreateMap<Game, GameDto>();
            CreateMap<Developer, DeveloperDto>();
            CreateMap<Genre, GenreDto>();
            CreateMap<GamePlatform, GamePlatformDto>();
        }
    }
}
