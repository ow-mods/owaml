using System.Collections.Generic;
using System.Linq;

namespace CMOWA
{
	//From https://github.com/amazingalek/owml/blob/master/src/OWML.Launcher/ArgumentHelper.cs
	public class ArgumentHelper
	{
		public string[] Arguments
		{
			get
			{
				return _arguments.ToArray();
			}
		}
		public ArgumentHelper(string[] args)
		{
			_arguments = args.ToList();
		}
		public string GetArgument(string name)
		{
			int index = _arguments.IndexOf("-" + name);
			bool flag = index == -1 || index >= _arguments.Count - 1;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = _arguments[index + 1];
			}
			return result;
		}
		public bool HasArgument(string name)
		{
			return GetArgument(name) != null;
		}
		public void RemoveArgument(string argument)
		{
			bool flag = HasArgument(argument);
			if (flag)
			{
				int index = _arguments.IndexOf("-" + argument);
				_arguments.RemoveRange(index, 2);
			}
		}
		private readonly List<string> _arguments;
	}
}
