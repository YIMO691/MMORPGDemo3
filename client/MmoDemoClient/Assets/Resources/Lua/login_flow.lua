-- Login flow: C# GameLauncher calls into this module.
-- Hotfix: modify UI flow without recompiling C#.

local M = {}

function M.on_login_success(playerId)
  -- Return the next view to show. Change to "city" to skip role select for debugging.
  return "role_select"
end

function M.on_login_error(code, message)
  return "login" -- stay on login
end

return M
