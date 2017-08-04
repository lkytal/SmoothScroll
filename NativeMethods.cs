using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmoothScroll
{
	class NativeMethods
	{
		public enum MouseEventSource
		{
			/// <summary>
			/// Events raised by the mouse
			/// </summary>
			Mouse,

			/// <summary>
			/// Events raised by a stylus
			/// </summary>
			Pen,

			/// <summary>
			/// Events raised by touching the screen
			/// </summary>
			Touch
		}

		/// <summary>
		/// Gets the extra information for the mouse event.
		/// </summary>
		/// <returns>The extra information provided by Windows API</returns>
		[DllImport("user32.dll")]
		private static extern uint GetMessageExtraInfo();

		/// <summary>
		/// Determines what input device triggered the mouse event.
		/// </summary>
		/// <returns>
		/// A result indicating whether the last mouse event was triggered
		/// by a touch, pen or the mouse.
		/// </returns>
		public static MouseEventSource GetMouseEventSource()
		{
			var extra = GetMessageExtraInfo();
			bool isTouchOrPen = ((extra & 0xFFFFFF00) == 0xFF515700);

			if (!isTouchOrPen)
			{
				return MouseEventSource.Mouse;
			}

			bool isTouch = ((extra & 0x00000080) == 0x00000080);

			return isTouch ? MouseEventSource.Touch : MouseEventSource.Pen;
		}

		public static bool IsMouseEvent()
		{
			return GetMouseEventSource() == MouseEventSource.Mouse;
		}
	}
}
