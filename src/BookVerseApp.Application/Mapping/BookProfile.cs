using AutoMapper;
using BookVerseApp.Application.DTOs;
using BookVerseApp.Domain.Entities;

namespace BookVerseApp.Application.Mapping;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookDto>().ReverseMap();
    }
}
