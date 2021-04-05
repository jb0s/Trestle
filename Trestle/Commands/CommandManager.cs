using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Networking;

namespace Trestle.Commands
{
    public class CommandManager
    {
        /// <summary>
        /// Dictionary of all commands and their classes.
        /// </summary>
        public Dictionary<string, (Type, MethodInfo)> Commands = new();

        public CommandManager()
        {
            InitializeCommands();
        }

        public void HandleCommand(Client client, string message)
        {
            var args = message.Substring(1).Split(' ');
            var command = args[0];

            args = args.Skip(0).ToArray();

            if (!Commands.TryGetValue(command, out var data))
            {
                client.Player.SendChat($"{ChatColor.Red}Unknown command! Do '/help' for more info.");
                return;
            }

            var (type, method) = data;
            
            var instance = (Command)Activator.CreateInstance(type);
            instance.Client = client;
            
            method.Invoke(instance, Array.Empty<object>());
        }
        
        private void InitializeCommands()
        {
            // Iterates over every type in the assembly 
            foreach(var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(Command))) 
                    continue;
                
                foreach (var method in type.GetMethods())
                {
                    var commandAttribute = method.GetCustomAttribute<CommandAttribute>(false);
                    if (commandAttribute == null) 
                        continue;
                    
                    var aliasAttribute = method.GetCustomAttribute<AliasAttribute>(false);
                    if (aliasAttribute != null)
                        foreach (var alias in aliasAttribute.Aliases)
                            Commands.Add(alias, (type, method));
                    
                    Commands.Add(commandAttribute.Command.ToLower(), (type, method));
                }
            }
        }
    }
}