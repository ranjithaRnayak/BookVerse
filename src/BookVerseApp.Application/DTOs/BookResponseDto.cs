using System;

namespace BookVerseApp.Application.DTOs;

public class BookResponseDto : BookBaseDto
{
    public Guid Id { get; set; }
}