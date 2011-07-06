using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Reflection;
using System.IO;


namespace XRouter.Gui
{
	static class ConfigurationControlManager
	{
		private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static UserControl LoadUserControlFormFile(string filePath, string ConfigurationControlFullName)
		{
			Assembly assembly = Assembly.LoadFile(Path.Combine(BinPath, filePath));
			Type type = assembly.GetType(ConfigurationControlFullName, true);

			var constructor = type.GetConstructor(new Type[] { });
			var userControlObject = constructor.Invoke(new object[] { });


			return userControlObject as UserControl;
		}
	}
}
