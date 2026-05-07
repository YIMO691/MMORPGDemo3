-- City flow: C# CityView calls into this module.
-- Hotfix: adjust display format or welcome message without recompiling.

local M = {}

function M.get_welcome_message(roleName, level, gold)
  return "Welcome to the city, " .. roleName .. "! (Lua)"
end

function M.format_gold(amount)
  return tostring(amount) .. " G"
end

return M
