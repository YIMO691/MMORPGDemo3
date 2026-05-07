-- Role select flow: C# RoleSelectView calls into this module.
-- Hotfix: adjust validation rules or flow without recompiling.

local M = {}

function M.validate_name(name)
  if name == nil or #name < 1 or #name > 12 then
    return false, "Name must be 1-12 characters"
  end
  return true, nil
end

function M.on_role_selected(roleId, roleName, level)
  return "city" -- next view
end

return M
