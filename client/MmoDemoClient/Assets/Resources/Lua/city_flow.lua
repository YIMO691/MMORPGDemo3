-- city_flow.lua
-- Phase 1 empty city flow. Displays role name, level, gold.

local CityFlow = {}
local _network = nil
local _ui = nil

function CityFlow.init(network_bridge, ui_bridge)
    _network = network_bridge
    _ui = ui_bridge
end

function CityFlow.on_enter(role_id)
    _ui.show_loading(true)

    _network:EnterCityAsync(
        role_id,
        function(result)
            CityFlow._on_enter_city(result)
        end,
        function(error_msg)
            _ui.show_error("Failed to enter city: " .. error_msg)
            _ui.show_loading(false)
        end
    )
end

function CityFlow._on_enter_city(result)
    _ui.show_loading(false)

    if result.code == 0 and result.role then
        local role = result.role
        _ui.set_city_display(
            role.name,    -- character name
            role.level,   -- level
            role.gold     -- gold
        )
    else
        _ui.show_error(result.message or "Enter city failed")
    end
end

return CityFlow
