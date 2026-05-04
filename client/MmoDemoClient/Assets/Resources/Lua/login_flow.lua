-- login_flow.lua
-- Phase 1 guest login flow. Called by LoginView UI.

local LoginFlow = {}
local _network = nil
local _ui = nil

function LoginFlow.init(network_bridge, ui_bridge)
    _network = network_bridge
    _ui = ui_bridge
end

function LoginFlow.on_login_click()
    local device_id = UnityEngine.SystemInfo.deviceUniqueIdentifier
    local platform = "editor"  -- replaced at build time for android/windows
    local app_version = "0.1.0"

    _ui.set_login_button_enabled(false)
    _ui.show_loading(true)

    _network:GuestLoginAsync(
        device_id,
        platform,
        app_version,
        function(result)  -- success callback
            LoginFlow._on_login_response(result)
        end,
        function(error_msg)  -- error callback
            _ui.show_error("Login failed: " .. error_msg)
            _ui.set_login_button_enabled(true)
            _ui.show_loading(false)
        end
    )
end

function LoginFlow._on_login_response(result)
    _ui.show_loading(false)

    if result.code == 0 then
        _ui.show_info("Welcome! Player: " .. result.playerId)
        _ui.show_role_select()
    else
        _ui.show_error("Login error: " .. (result.message or "unknown"))
        _ui.set_login_button_enabled(true)
    end
end

return LoginFlow
