namespace User.Application.Dtos;

public record UpdateUserProfileDto(
    int? Weight,
    string? Gender,
    string? ActivityLevel,
    string? FitnessGoal
);
