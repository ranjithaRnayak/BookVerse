using AutoMapper;
using BookVerseApp.Application.DTOs;
using BookVerseApp.Domain.Entities;

namespace BookVerseApp.Application.Mapping;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<BookDto, Book>()
    .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Book, BookDto>().ReverseMap();
        CreateMap<Book, BookResponseDto>();
    }
}
