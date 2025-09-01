namespace UserManagementApi.DTO
{
    public record FunctionDto(int Id, string Code, string DisplayName);
    public record ModuleDto(int Id, string Name, string Area, string Controller, string Action, List<FunctionDto> Functions);
    public record CategoryDto(int Id, string Name, List<ModuleDto> Modules);
    public record UserPermissionsDto(int UserId, string UserName, List<CategoryDto> Categories);
}
