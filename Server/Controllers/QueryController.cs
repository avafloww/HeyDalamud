﻿using System.Text;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Chat;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class QueryController : ControllerBase
{
    private readonly Conversation _conversation;

    public QueryController(Conversation conversation)
    {
        _conversation = conversation;
    }

    [HttpPost]
    public async Task<QueryResponsePayload> RespondToQuery(QueryRequestPayload payload)
    {
        try {
            // build context string
            var context = new StringBuilder();

            if (payload.IsInGame) {
                context.Append("User Name: ");
                context.Append(payload.CharacterFirstName);
                context.Append('\n');
                
                if (payload.ActiveDutyName != null) {
                    context.Append("Context: in duty: ");
                    context.Append(payload.ActiveDutyName);
                    context.Append('\n');
                } else if (payload.ActiveAreaName != null) {
                    context.Append("Context: in area: ");
                    context.Append(payload.ActiveAreaName);
                    context.Append('\n');
                }
            } else {
                context.Append("User is not logged in\n");
            }
            
            context.Append("Query: ");
            context.Append(payload.Query);

            _conversation.AppendUserInput(context.ToString());
            var response = await _conversation.GetResponseFromChatbot();

            return new QueryResponsePayload
            {
                Success = true,
                Response = response
            };
        } catch (Exception ex) {
            return new QueryResponsePayload
            {
                Success = false,
                Response = "Uh oh, looks like something went wrong. Try again later!"
            };
        }
    }

    public class QueryRequestPayload
    {
        public string Query { get; set; }

        public bool IsInGame { get; set; }
        public string? CharacterFirstName { get; set; }
        public string? ActiveDutyName { get; set; }
        public string? ActiveAreaName { get; set; }
    }

    public class QueryResponsePayload
    {
        public bool Success { get; set; }
        public string Response { get; set; }
        public string? Command { get; set; }
    }
}
