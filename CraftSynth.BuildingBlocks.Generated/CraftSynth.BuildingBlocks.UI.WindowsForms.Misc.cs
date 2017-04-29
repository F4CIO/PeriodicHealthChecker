using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CraftSynth.BuildingBlocks.UI.WindowsForms
{
	public static class Misc
	{
		/// <summary>
		/// Example: C:\App1
		/// </summary>
		public static string ApplicationPhysicalPath
		{
			get
			{
				return Path.GetDirectoryName(Application.ExecutablePath);
			}

		}

		/// <summary>
		/// Example: C:\App1\app1.exe
		/// </summary>
		public static string ApplicationPhysicalExeFilePath
		{
			get
			{
				return Application.ExecutablePath;
			}
		}

		/// <summary>
		/// Example: C:\App1\app1
		/// </summary>
		public static string ApplicationPhysicalExeFilePathWithoutExtension
		{
			get
			{
				//.Location not tested!!
				string r = ApplicationPhysicalExeFilePath;
				r = Path.ChangeExtension(r, string.Empty);
				r = r.TrimEnd('.');
				return r;
			}
		}

		/// <summary>
		/// Recursively searches for control with specified type and name
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="startingControl"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static T FindControl<T>(Control startingControl, string name) where T : Control
		{
			// this is null by default
			T found = default(T);

			int controlCount = startingControl.Controls.Count;

			if (controlCount > 0)
			{
				for (int i = 0; i < controlCount; i++)
				{
					Control activeControl = startingControl.Controls[i];
					if (activeControl is T)
					{
						found = startingControl.Controls[i] as T;
						if (string.Compare(name, found.Name, true) == 0) break;
						else found = null;
					}
					else
					{
						found = FindControl<T>(activeControl, name);
						if (found != null) break;
					}
				}
			}
			return found;
		}

		public static Point LocationInScreenCoordinates(this Control c)
		{
			return c.PointToScreen(new Point(0, 0));
		}

		public static bool CaptionBarIntersectsAnyScreen(this Form form)
		{
			Rectangle captionBar = new Rectangle(form.Left, form.Top, form.Width, SystemInformation.CaptionHeight);
			bool captionBarIntersectsAnyScreen = false;
			foreach (Screen screen in Screen.AllScreens)
			{
				if (captionBar.IntersectsWith(screen.WorkingArea))
				{
					captionBarIntersectsAnyScreen = true;
					break;
				}
			}
			return captionBarIntersectsAnyScreen;
		}

	}
}
