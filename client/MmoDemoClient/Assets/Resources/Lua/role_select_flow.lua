-- role_select_flow.lua
-- Phase 1 role selection flow. Called by RoleSelectView UI.

local RoleSelectFlow = {}
local _network = nil
local _ui = nil

function RoleSelectFlow.init(network_bridge, ui_bridge)
    _network = network_bridge
    _ui = ui_bridge
end

function RoleSelectFlow.on_enter()
    _ui.show_loading(true)
    _network:GetRoleListAsync(
        function(result)
            RoleSelectFlow._on_role_list(result)
        end,
        function(error_msg)
            _ui.show_error("Failed to load roles: " .. error_msg)
            _ui.show_loading(false)
        end
    )
end

function RoleSelectFlow._on_role_list(result)
    _ui.show_loading(false)

    if result.code ~= 0 then
        _ui.show_error(result.message or "Unknown error")
        return
    end

    local roles = result.roles or {}
    if #roles == 0 then
        _ui.show_create_role_panel()
    else
        _ui.show_role_list(roles)
    end
end

function RoleSelectFlow.on_create_role(name, class_id)
    if not name or string.len(name) < 1 or string.len(name) > 12 then
        _ui.show_error("Name must be 1-12 characters")
        return
    end

    _ui.show_loading(true)
    _network:CreateRoleAsync(
        name,
        class_id,
        function(result)
            RoleSelectFlow._on_create_role(result)
        end,
        function(error_msg)
            _ui.show_error("Failed to create role: " .. error_msg)
            _ui.show_loading(false)
        end
    )
end

function RoleSelectFlow._on_create_role(result)
    _ui.show_loading(false)

    if result.code == 0 and result.role then
        -- Auto-select and enter city
        RoleSelectFlow.on_select_role(result.role.roleId)
    else
        _ui.show_error(result.message or "Create role failed")
    end
end

function RoleSelectFlow.on_select_role(role_id)
    _ui.show_loading(true)
    _network:SelectRoleAsync(
        role_id,
        function(result)
            RoleSelectFlow._on_select_role(result)
        end,
        function(error_msg)
            _ui.show_error("Failed to select role: " .. error_msg)
            _ui.show_loading(false)
        end
    )
end

function RoleSelectFlow._on_select_role(result)
    _ui.show_loading(false)

    if result.code == 0 then
        _ui.show_city()
    else
        _ui.show_error(result.message or "Select role failed")
    end
end

return RoleSelectFlow
