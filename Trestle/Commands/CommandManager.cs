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

            args = args.Skip(1).ToArray();

            if (!Commands.TryGetValue(command, out var data))
            {
                client.Player.SendChat($"{ChatColor.Red}Unknown command! Type '/help' for a list of commands.");
                return;
            }

            var (type, method) = data;
            var parameters = method.GetParameters();
            
            if (args.Length != parameters.Length)
            {
                client.Player.SendChat($"{ChatColor.Red}Missing arguments! Type '/help' for a usage guide.");
                return;
            }

            var newArgs = new object[parameters.Length];
            for (int i = 0; i < newArgs.Length; i++)
            {
                try
                {
                    var @switch = new Dictionary<Type, Action> {
                        { typeof(ushort), () => newArgs[i] = ushort.Parse(args[i]) },
                        { typeof(short), () => newArgs[i] = short.Parse(args[i]) },
                        { typeof(int), () => newArgs[i] = int.Parse(args[i]) },
                        { typeof(long), () => newArgs[i] = long.Parse(args[i]) },
                        { typeof(float), () => newArgs[i] = float.Parse(args[i]) },
                        { typeof(double), () => newArgs[i] = double.Parse(args[i]) },
                        { typeof(bool), () => newArgs[i] = bool.Parse(args[i]) },
                        { typeof(string), () => newArgs[i] = args[i] }
                    };

                    @switch[parameters[i].ParameterType]();
                }
                catch {}
            }
            
            var instance = (Command)Activator.CreateInstance(type);
            instance.Client = client;
            
            method.Invoke(instance, newArgs);
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