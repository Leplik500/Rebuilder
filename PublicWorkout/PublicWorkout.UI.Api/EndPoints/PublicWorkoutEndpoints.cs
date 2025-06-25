using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Pepegov.MicroserviceFramework.AspNetCore.WebApplicationDefinition;
using Pepegov.MicroserviceFramework.Definition;
using Pepegov.MicroserviceFramework.Definition.Context;
using PublicWorkout.Application.Dtos;
using PublicWorkout.Application.Services.Interfaces;
using PublicWorkout.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;

namespace PublicWorkout.UI.Api.EndPoints;

public class PublicWorkoutEndPoints : ApplicationDefinition
{
    public override Task ConfigureApplicationAsync(
        IDefinitionApplicationContext context
    )
    {
        var app = context.Parse<WebDefinitionApplicationContext>().WebApplication;
        var group = app.MapGroup("/public/workouts")
            .WithOpenApi()
            .WithTags("Public Workouts");

        // 3.1 Просмотр каталога
        group
            .MapGet(
                string.Empty,
                async (
                    [FromQuery] string? type,
                    [FromQuery] int? durationMin,
                    [FromQuery] int? durationMax,
                    [FromQuery] string? sortBy,
                    [FromQuery] string? sortOrder,
                    [FromQuery] int page,
                    [FromQuery] int pageSize,
                    IPublicWorkoutService workoutService
                ) =>
                    await GetWorkouts(
                        type,
                        durationMin,
                        durationMax,
                        sortBy,
                        sortOrder,
                        workoutService,
                        page,
                        pageSize
                    )
            )
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary =
                    "Retrieves a list of public workouts with optional filtering and sorting.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "type",
                        In = ParameterLocation.Query,
                        Description = "Filter by workout type.",
                        Required = false,
                        Schema = new OpenApiSchema { Type = "string" },
                    },
                    new OpenApiParameter
                    {
                        Name = "durationMin",
                        In = ParameterLocation.Query,
                        Description = "Minimum duration in seconds for filtering.",
                        Required = false,
                        Schema = new OpenApiSchema { Type = "integer" },
                    },
                    new OpenApiParameter
                    {
                        Name = "durationMax",
                        In = ParameterLocation.Query,
                        Description = "Maximum duration in seconds for filtering.",
                        Required = false,
                        Schema = new OpenApiSchema { Type = "integer" },
                    },
                    new OpenApiParameter
                    {
                        Name = "sortBy",
                        In = ParameterLocation.Query,
                        Description = "Sort by field (likes_count, copies_count).",
                        Required = false,
                        Schema = new OpenApiSchema { Type = "string" },
                    },
                    new OpenApiParameter
                    {
                        Name = "sortOrder",
                        In = ParameterLocation.Query,
                        Description = "Sort order (asc, desc).",
                        Required = false,
                        Schema = new OpenApiSchema { Type = "string" },
                    },
                    new OpenApiParameter
                    {
                        Name = "page",
                        In = ParameterLocation.Query,
                        Description = "Page number for pagination.",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "integer",
                            Default = new Microsoft.OpenApi.Any.OpenApiInteger(1),
                        },
                    },
                    new OpenApiParameter
                    {
                        Name = "pageSize",
                        In = ParameterLocation.Query,
                        Description = "Number of items per page.",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "integer",
                            Default = new Microsoft.OpenApi.Any.OpenApiInteger(20),
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description =
                                "List of public workouts retrieved successfully.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema
                                            {
                                                Reference = new OpenApiReference
                                                {
                                                    Type = ReferenceType.Schema,
                                                    Id = "PublicWorkoutDto",
                                                },
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description = "Invalid filter or sort parameters.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        group
            .MapGet("{publicWorkoutId:guid}", GetWorkoutDetails)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary =
                    "Retrieves detailed information about a specific public workout.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description =
                            "The ID of the public workout to retrieve details for.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description =
                                "Public workout details retrieved successfully.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "PublicWorkoutDetailDto",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        // 3.2 Лайки и копирование
        group
            .MapPost("{publicWorkoutId:guid}/like", LikeWorkout)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Likes a specific public workout.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description = "The ID of the public workout to like.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "Workout liked successfully.",
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description =
                                "Failed to like workout due to invalid data or already liked.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        group
            .MapDelete("{publicWorkoutId:guid}/like", UnlikeWorkout)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Unlikes a specific public workout.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description = "The ID of the public workout to unlike.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "Workout unliked successfully.",
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description =
                                "Failed to unlike workout due to invalid data or not liked.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        group
            .MapPost("{publicWorkoutId:guid}/copy", CopyWorkout)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary =
                    "Copies a specific public workout to the user's private workouts.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description = "The ID of the public workout to copy.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description =
                                "Workout copied successfully to private workouts.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "PrivateWorkoutDto",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description =
                                "Failed to copy workout due to invalid data or already copied.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        group
            .MapPost("sync", SyncPublicWorkout)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Synchronizes a private workout to the public catalog.",
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "application/json",
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.Schema,
                                        Id = "SyncPublicWorkoutDto",
                                    },
                                },
                            }
                        },
                    },
                    Description = "The data of the private workout to synchronize.",
                    Required = true,
                },
                Responses = new OpenApiResponses
                {
                    {
                        "201",
                        new OpenApiResponse
                        {
                            Description = "Public workout created successfully.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "PublicWorkoutDto",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "Public workout updated successfully.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "PublicWorkoutDto",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description = "Invalid workout data.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "401",
                        new OpenApiResponse { Description = "Unauthorized access." }
                    },
                    {
                        "403",
                        new OpenApiResponse
                        {
                            Description =
                                "Forbidden. Insufficient permissions to publish the workout.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        group
            .MapGet("{publicWorkoutId:guid}/copies", GetWorkoutCopies)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary =
                    "Retrieves a list of user IDs who copied the specific public workout.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description =
                            "The ID of the public workout to retrieve copies for.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description =
                                "List of user IDs who copied the workout retrieved successfully.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Format = "uuid",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "403",
                        new OpenApiResponse
                        {
                            Description =
                                "Access denied. Only the owner can view copies.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        // 3.3 Комментарии
        group
            .MapGet(
                "{publicWorkoutId:guid}/comments",
                async (
                    Guid publicWorkoutId,
                    [FromQuery] Guid? parentCommentId,
                    [FromQuery] int page,
                    [FromQuery] int pageSize,
                    ICommentService commentService
                ) =>
                    await GetWorkoutComments(
                        publicWorkoutId,
                        parentCommentId,
                        commentService,
                        page,
                        pageSize
                    )
            )
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary =
                    "Retrieves a list of comments for a specific public workout.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description =
                            "The ID of the public workout to retrieve comments for.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                    new OpenApiParameter
                    {
                        Name = "parentCommentId",
                        In = ParameterLocation.Query,
                        Description =
                            "Optional filter for comments under a specific parent comment.",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                    new OpenApiParameter
                    {
                        Name = "page",
                        In = ParameterLocation.Query,
                        Description = "Page number for pagination.",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "integer",
                            Default = new Microsoft.OpenApi.Any.OpenApiInteger(1),
                        },
                    },
                    new OpenApiParameter
                    {
                        Name = "pageSize",
                        In = ParameterLocation.Query,
                        Description = "Number of items per page.",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "integer",
                            Default = new Microsoft.OpenApi.Any.OpenApiInteger(20),
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "List of comments retrieved successfully.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema
                                            {
                                                Reference = new OpenApiReference
                                                {
                                                    Type = ReferenceType.Schema,
                                                    Id = "CommentDto",
                                                },
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        group
            .MapPost("{publicWorkoutId:guid}/comments", AddWorkoutComment)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Adds a new comment to a specific public workout.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description =
                            "The ID of the public workout to add a comment to.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "application/json",
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.Schema,
                                        Id = "AddCommentDto",
                                    },
                                },
                            }
                        },
                    },
                    Description = "The comment data to add.",
                    Required = true,
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "Comment added successfully.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "CommentDto",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description = "Invalid comment data.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        group
            .MapPatch(
                "{publicWorkoutId:guid}/comments/{commentId:guid}",
                UpdateWorkoutComment
            )
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary =
                    "Updates an existing comment for a specific public workout.",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description =
                            "The ID of the public workout containing the comment.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                    new()
                    {
                        Name = "commentId",
                        In = ParameterLocation.Path,
                        Description = "The ID of the comment to update.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "application/json",
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.Schema,
                                        Id = "UpdateCommentDto",
                                    },
                                },
                            }
                        },
                    },
                    Description = "The updated comment data.",
                    Required = true,
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "Comment updated successfully.",
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description = "Invalid update data.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "403",
                        new OpenApiResponse
                        {
                            Description =
                                "Access denied. Only the author can update the comment.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Comment or public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });
        ;

        group
            .MapDelete(
                "{publicWorkoutId:guid}/comments/{commentId:guid}",
                DeleteWorkoutComment
            )
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Deletes a specific comment for a public workout.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "publicWorkoutId",
                        In = ParameterLocation.Path,
                        Description =
                            "The ID of the public workout containing the comment.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                    new OpenApiParameter
                    {
                        Name = "commentId",
                        In = ParameterLocation.Path,
                        Description = "The ID of the comment to delete.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "Comment deleted successfully.",
                        }
                    },
                    {
                        "403",
                        new OpenApiResponse
                        {
                            Description =
                                "Access denied. Only the author or moderator can delete the comment.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Comment or public workout not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        // 3.4 Лайки комментариев
        var commentGroup = app.MapGroup("/public/comments")
            .WithOpenApi()
            .WithTags("Public Workout Comments");
        commentGroup
            .MapPost("{commentId:guid}/like", LikeComment)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Likes a specific comment.",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "commentId",
                        In = ParameterLocation.Path,
                        Description = "The ID of the comment to like.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "Comment liked successfully.",
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description =
                                "Failed to like comment due to invalid data or already liked.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Comment not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        commentGroup
            .MapDelete("{commentId:guid}/like", UnlikeComment)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Unlikes a specific comment.",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "commentId",
                        In = ParameterLocation.Path,
                        Description = "The ID of the comment to unlike.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                        },
                    },
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "Comment unliked successfully.",
                        }
                    },
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description =
                                "Failed to unlike comment due to invalid data or not liked.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                    {
                        "404",
                        new OpenApiResponse
                        {
                            Description = "Comment not found.",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "ErrorResponse",
                                            },
                                        },
                                    }
                                },
                            },
                        }
                    },
                },
            });

        return base.ConfigureApplicationAsync(context);
    }

    // Остальные методы остаются без изменений
    [SwaggerResponse(
        200,
        "List of public workouts retrieved successfully.",
        typeof(List<PublicWorkoutDto>)
    )]
    [SwaggerResponse(
        400,
        "Invalid filter or sort parameters.",
        typeof(ErrorResponse)
    )]
    private static async Task<IResult> GetWorkouts(
        [FromQuery] string? type,
        [FromQuery] int? durationMin,
        [FromQuery] int? durationMax,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        IPublicWorkoutService workoutService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var result = await workoutService.GetWorkoutsAsync(
            type,
            durationMin,
            durationMax,
            sortBy,
            sortOrder,
            page,
            pageSize
        );
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(
        200,
        "Public workout details retrieved successfully.",
        typeof(PublicWorkoutDetailDto)
    )]
    [SwaggerResponse(404, "Public workout not found.", typeof(ErrorResponse))]
    private static async Task<IResult> GetWorkoutDetails(
        Guid publicWorkoutId,
        IPublicWorkoutService workoutService
    )
    {
        var result = await workoutService.GetWorkoutDetailsAsync(publicWorkoutId);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Errors);
    }

    [SwaggerResponse(200, "Workout liked successfully.")]
    [SwaggerResponse(
        400,
        "Failed to like workout due to invalid data or already liked.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(404, "Public workout not found.", typeof(ErrorResponse))]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> LikeWorkout(
        Guid publicWorkoutId,
        IPublicWorkoutService workoutService
    )
    {
        var result = await workoutService.LikeWorkoutAsync(publicWorkoutId);
        return result.IsSuccess ? Results.Ok()
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(200, "Workout unliked successfully.")]
    [SwaggerResponse(
        400,
        "Failed to unlike workout due to invalid data or not liked.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(404, "Public workout not found.", typeof(ErrorResponse))]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> UnlikeWorkout(
        Guid publicWorkoutId,
        IPublicWorkoutService workoutService
    )
    {
        var result = await workoutService.UnlikeWorkoutAsync(publicWorkoutId);
        return result.IsSuccess ? Results.Ok()
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(
        200,
        "Workout copied successfully to private workouts.",
        typeof(PrivateWorkoutDto)
    )]
    [SwaggerResponse(
        400,
        "Failed to copy workout due to invalid data or already copied.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(404, "Public workout not found.", typeof(ErrorResponse))]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> CopyWorkout(
        Guid publicWorkoutId,
        IPublicWorkoutService workoutService
    )
    {
        var result = await workoutService.CopyWorkoutAsync(publicWorkoutId);
        return result.IsSuccess ? Results.Ok(result.Value)
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(
        200,
        "List of users who copied the workout retrieved successfully.",
        typeof(List<UserDto>)
    )]
    [SwaggerResponse(
        403,
        "Access denied. Only the owner can view copies.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(404, "Public workout not found.", typeof(ErrorResponse))]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> GetWorkoutCopies(
        Guid publicWorkoutId,
        IPublicWorkoutService workoutService
    )
    {
        var result = await workoutService.GetWorkoutCopiesAsync(publicWorkoutId);
        return result.IsSuccess ? Results.Ok(result.Value)
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : Results.Forbid();
    }

    [SwaggerResponse(
        200,
        "List of comments retrieved successfully.",
        typeof(List<CommentDto>)
    )]
    [SwaggerResponse(404, "Public workout not found.", typeof(ErrorResponse))]
    private static async Task<IResult> GetWorkoutComments(
        Guid publicWorkoutId,
        [FromQuery] Guid? parentCommentId,
        ICommentService commentService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var result = await commentService.GetCommentsAsync(
            publicWorkoutId,
            parentCommentId,
            page,
            pageSize
        );
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Errors);
    }

    [SwaggerResponse(200, "Comment added successfully.", typeof(CommentDto))]
    [SwaggerResponse(400, "Invalid comment data.", typeof(ErrorResponse))]
    [SwaggerResponse(404, "Public workout not found.", typeof(ErrorResponse))]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> AddWorkoutComment(
        Guid publicWorkoutId,
        HttpContext context,
        ICommentService commentService
    )
    {
        var commentDto = await context.Request.ReadFromJsonAsync<AddCommentDto>();
        if (commentDto == null)
        {
            return Results.BadRequest(
                new { Errors = new[] { "Invalid comment data" } }
            );
        }

        var result = await commentService.AddCommentAsync(
            publicWorkoutId,
            commentDto
        );
        return result.IsSuccess ? Results.Ok(result.Value)
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(200, "Comment updated successfully.")]
    [SwaggerResponse(400, "Invalid update data.", typeof(ErrorResponse))]
    [SwaggerResponse(
        403,
        "Access denied. Only the author can update the comment.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        404,
        "Comment or public workout not found.",
        typeof(ErrorResponse)
    )]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> UpdateWorkoutComment(
        Guid publicWorkoutId,
        Guid commentId,
        HttpContext context,
        ICommentService commentService
    )
    {
        var updateDto = await context.Request.ReadFromJsonAsync<UpdateCommentDto>();
        if (updateDto == null)
        {
            return Results.BadRequest(
                new { Errors = new[] { "Invalid update data" } }
            );
        }

        var result = await commentService.UpdateCommentAsync(
            publicWorkoutId,
            commentId,
            updateDto
        );
        return result.IsSuccess ? Results.Ok()
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : result.Errors.Any(e => e.Message.Contains("access denied"))
                ? Results.Forbid()
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(200, "Comment deleted successfully.")]
    [SwaggerResponse(
        403,
        "Access denied. Only the author or moderator can delete the comment.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        404,
        "Comment or public workout not found.",
        typeof(ErrorResponse)
    )]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> DeleteWorkoutComment(
        Guid publicWorkoutId,
        Guid commentId,
        ICommentService commentService
    )
    {
        var result = await commentService.DeleteCommentAsync(
            publicWorkoutId,
            commentId
        );
        return result.IsSuccess ? Results.Ok()
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : result.Errors.Any(e => e.Message.Contains("access denied"))
                ? Results.Forbid()
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(200, "Comment liked successfully.")]
    [SwaggerResponse(
        400,
        "Failed to like comment due to invalid data or already liked.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(404, "Comment not found.", typeof(ErrorResponse))]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> LikeComment(
        Guid commentId,
        ICommentService commentService
    )
    {
        var result = await commentService.LikeCommentAsync(commentId);
        return result.IsSuccess ? Results.Ok()
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(200, "Comment unliked successfully.")]
    [SwaggerResponse(
        400,
        "Failed to unlike comment due to invalid data or not liked.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(404, "Comment not found.", typeof(ErrorResponse))]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> UnlikeComment(
        Guid commentId,
        ICommentService commentService
    )
    {
        var result = await commentService.UnlikeCommentAsync(commentId);
        return result.IsSuccess ? Results.Ok()
            : result.Errors.Any(e => e.Message.Contains("not found"))
                ? Results.NotFound(result.Errors)
            : Results.BadRequest(result.Errors);
    }

    [SwaggerResponse(
        201,
        "Public workout created successfully.",
        typeof(PublicWorkoutDto)
    )]
    [SwaggerResponse(
        200,
        "Public workout updated successfully.",
        typeof(PublicWorkoutDto)
    )]
    [SwaggerResponse(400, "Invalid workout data.", typeof(ErrorResponse))]
    [SwaggerResponse(401, "Unauthorized access.")]
    [SwaggerResponse(
        403,
        "Forbidden. Insufficient permissions to publish the workout.",
        typeof(ErrorResponse)
    )]
    [Authorize(AuthenticationSchemes = AuthData.AuthenticationSchemes)]
    private static async Task<IResult> SyncPublicWorkout(
        HttpContext context,
        IPublicWorkoutService workoutService
    )
    {
        var syncDto =
            await context.Request.ReadFromJsonAsync<SyncPublicWorkoutDto>();
        if (syncDto == null)
        {
            return Results.BadRequest(
                new { Errors = new[] { "Invalid workout data" } }
            );
        }

        var result = await workoutService.SyncPublicWorkoutAsync(syncDto);
        return result.IsSuccess
            ? result.Value.IsNew
                ? Results.Created($"/public/workouts/{result.Value}", result.Value)
                : Results.Ok(result.Value)
            : result.Errors.Any(e => e.Message.Contains("forbidden"))
                ? Results.Forbid()
                : Results.BadRequest(result.Errors);
    }
}
