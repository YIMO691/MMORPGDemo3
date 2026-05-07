using MmoDemo.Contracts;

namespace MmoDemo.Application;

public interface IRoleService
{
    GetRoleListResponse GetRoleList(GetRoleListRequest request);
    CreateRoleResponse CreateRole(CreateRoleRequest request);
    SelectRoleResponse SelectRole(SelectRoleRequest request);
}
