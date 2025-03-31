﻿using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemBriefDto
{
    public int Id { get; init; }
    public int ListId { get; init; }
    public string? Title { get; init; }
    public string? Note { get; init; }
    public DateTime? Reminder { get; set; }
    public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.None;
    public bool Done { get; init; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TodoItem, TodoItemBriefDto>();
        }
    }
}
